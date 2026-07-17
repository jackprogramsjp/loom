using Loom.Core.Diagnostics;
using Loom.Core.Parsing.AST;
using Loom.Core.Text;

namespace Loom.Core.Parsing;

public sealed partial class Parser
{
    private Expression ParseExpression() => ParseBinaryLevel(0);

    private Expression ParseBinaryLevel(int level)
    {
        if (level >= BinaryPrecedenceLevel.Levels.Length)
            return ParseRange();

        var (rightAssociative, matches) = BinaryPrecedenceLevel.Levels[level];
        var left = ParseBinaryLevel(level + 1);
        while (Match(out var op, matches))
        {
            switch (op.Kind)
            {
                case SyntaxKind.AsKeyword:
                {
                    var type = ParseType();
                    left = new AsExpression(op, left, type);
                    continue;
                }
                case SyntaxKind.Question:
                {
                    var thenBranch = ParseBinaryLevel(level);
                    var colon = Expect(SyntaxKind.Colon);
                    var elseBranch = ParseBinaryLevel(level);
                    left = new TernaryOperator(op, colon, left, thenBranch, elseBranch);
                    continue;
                }
            }

            var right = ParseBinaryLevel(rightAssociative ? level : level + 1);
            var isAssignment = SyntaxFacts.IsAssignmentOperator(op.Kind);
            if (isAssignment && left is not AssignmentTarget)
            {
                _diagnostics.Error(left, InternalCodes.InvalidAssignmentTarget, "Invalid assignment target.", $"did you mean '{left} == {right}'?");
                return left;
            }

            left = isAssignment && left is AssignmentTarget target
                ? new AssignmentOperator(op, target, right)
                : new BinaryOperator(op, left, right);
        }

        return left;
    }

    private Expression ParseRange()
    {
        var expression = ParseUnary();
        if (!Match(out var dotDot, SyntaxKind.DotDot))
            return expression;

        var maximum = ParseUnary();
        return new RangeLiteral(dotDot, expression, maximum);
    }

    private InterfaceInvocation ParseInterfaceInvocation(Token keyword)
    {
        var name = new Identifier(ExpectIdentifier());
        var typeArguments = ParseTypeArguments(forInvocation: true);
        var leftBrace = Expect(SyntaxKind.LBrace);
        var initializers = new List<InterfaceInvocationInitializer>();
        if (!Match(out var rightBrace, SyntaxKind.RBrace))
        {
            initializers.AddRange(ParseDelimited(ParseInterfaceInvocationInitializer));
            rightBrace = Expect(SyntaxKind.RBrace);
        }

        var body = new InterfaceInvocationBody(leftBrace, rightBrace, initializers);
        return new InterfaceInvocation(keyword, name, typeArguments, body);
    }

    private Expression ParseUnary() =>
        Match(out var op, SyntaxFacts.IsUnaryOperator)
            ? new UnaryOperator(op, ParseUnary())
            : ParsePostfix();

    private Expression ParsePostfix()
    {
        var expression = ParsePrimary();
        while (!IsEof())
        {
            if (Current() is { Kind: SyntaxKind.LParen or SyntaxKind.ColonColonLArrow })
            {
                var typeArguments = ParseTypeArguments(forInvocation: true);
                var leftParen = Expect(SyntaxKind.LParen);
                var arguments = ParseArguments(leftParen);
                expression = new Invocation(expression, typeArguments, arguments);
            }
            else if (Match(out var leftBracket, SyntaxKind.LBracket))
            {
                var indexExpression = ParseExpression();
                var rightBracket = Expect(SyntaxKind.RBracket);
                expression = new ElementAccess(leftBracket, rightBracket, expression, indexExpression);
            }
            else if (Match(out var dot, SyntaxKind.Dot))
            {
                var name = ExpectIdentifier();
                var names = new List<DotName> { new(dot, name) };
                while (Match(out var nextDot, SyntaxKind.Dot))
                    names.Add(new DotName(nextDot, ExpectIdentifier()));

                expression = expression is Identifier identifier
                    ? new QualifiedName(identifier, names)
                    : new PropertyAccess(expression, names);
            }
            else
            {
                // TODO: postfix unary operators
                break;
            }
        }

        return expression;
    }

    private Expression ParsePrimary()
    {
        if (Match(out var matchKeyword, SyntaxKind.MatchKeyword))
            return ParseMatchExpression(matchKeyword);

        if (Match(out var newKeyword, SyntaxKind.NewKeyword))
            return ParseInterfaceInvocation(newKeyword);

        if (Match(out var leftParen, SyntaxKind.LParen))
            return ParseParenthesized(leftParen);

        if (Match(out var mutKeyword, SyntaxKind.MutKeyword) && ParseArrayLiteral(mutKeyword) is { } mutableArrayLiteral)
            return mutableArrayLiteral;

        if (ParseArrayLiteral() is { } arrayLiteral)
            return arrayLiteral;

        if (Match(out var nameOfKeyword, SyntaxKind.NameOfKeyword))
            return ParseNameOf(nameOfKeyword);

        if (Match(out var nameToken, SyntaxKind.Identifier))
            return new Identifier(nameToken);

        if (Match(out var token, SyntaxFacts.IsLiteral))
            return new Literal(token, LiteralUtility.ResolveValue(token));

        var current = Current();
        if (IsEof())
        {
            _diagnostics.Error(current, InternalCodes.UnexpectedEof, "Unexpected end of file.");
        }
        else
        {
            _diagnostics.Error(current, InternalCodes.UnexpectedToken, $"Expected expression, got {SafeTokenText(Current())}.");
            _position++;
        }

        return new NullExpression(current);
    }

    private InterfaceInvocationInitializer ParseInterfaceInvocationInitializer()
    {
        if (Match(out var name, SyntaxKind.Identifier))
        {
            if (!Match(out var colon, SyntaxKind.Colon))
                return new InterfaceInvocationShorthandPropertyInitializer(new Identifier(name));

            var expression = ParseExpression();
            return new InterfaceInvocationPropertyInitializer(name, colon, expression);
        }

        var leftBracket = Expect(SyntaxKind.LBracket, "property name or index initializer");
        var indexExpression = ParseExpression();
        var rightBracket = Expect(SyntaxKind.RBracket);
        var indexColon = Expect(SyntaxKind.Colon);
        var indexValueExpression = ParseExpression();
        return new InterfaceInvocationIndexInitializer(leftBracket, rightBracket, indexColon, indexExpression, indexValueExpression);
    }

    private Arguments ParseArguments(Token leftParen)
    {
        if (Match(out var matchedRightParen, SyntaxKind.RParen))
            return new Arguments(leftParen, matchedRightParen, []);

        var argumentList = ParseDelimited(ParseExpression);
        var rightParen = Expect(SyntaxKind.RParen);
        return new Arguments(leftParen, rightParen, argumentList);
    }

    private Parenthesized ParseParenthesized(Token leftParen)
    {
        var expression = ParseExpression();
        var rightParen = Expect(
            SyntaxKind.RParen,
            got => $"Expected ')' here to close '{leftParen.Text}' at character {leftParen.Span.Start.Character}, got {SafeTokenText(got)}."
        );

        return new Parenthesized(leftParen, rightParen, expression);
    }

    private Expression ParseNameOf(Token keyword)
    {
        var typeArguments = ParseTypeArguments<TypeName>(true, "May only get name of type when the type is a type name.");
        var leftParen = Expect(SyntaxKind.LParen);
        var expression = typeArguments == null ? ParseExpression() : null;
        var rightParen = Expect(SyntaxKind.RParen);
        if (expression is Name name)
            return new NameOf(keyword, null, leftParen, rightParen, name);

        if (typeArguments != null)
        {
            if (typeArguments.ArgumentsList.Count == 1)
                return new NameOf(keyword, typeArguments, leftParen, rightParen, null);

            _diagnostics.Error(typeArguments, InternalCodes.GenericArity, "Exactly one type parameter is allowed for 'nameof::<T>()'.");
            return new NullExpression(keyword);
        }

        _diagnostics.Error(
            typeArguments?.Span ?? expression!.Span,
            InternalCodes.InvalidNameOf,
            $"'{typeArguments?.ArgumentsList.FirstOrDefault()?.ToString() ?? expression!.ToString()}' is not a valid name."
        );

        return new NullExpression(keyword);
    }

    private ArrayLiteral? ParseArrayLiteral(Token? mutKeyword = null)
    {
        if (!Match(out var leftBracket, SyntaxKind.LBracket))
            return null;

        if (Match(out var immediateRightBracket, SyntaxKind.RBracket))
            return new ArrayLiteral(mutKeyword, leftBracket, immediateRightBracket, []);

        var expressions = ParseDelimited(ParseExpression);
        var rightBracket = Expect(SyntaxKind.RBracket);
        return new ArrayLiteral(mutKeyword, leftBracket, rightBracket, expressions);
    }

    private MatchExpression ParseMatchExpression(Token keyword)
    {
        var expression = ParseExpression();
        var leftBrace = Expect(SyntaxKind.LBrace);
        var arms = new List<MatchArm>();
        while (!IsEof() && Current() is not { Kind: SyntaxKind.RBrace })
        {
            arms.Add(ParseMatchArm());
            Match(SyntaxKind.Comma, SyntaxKind.Semicolon);
        }

        var rightBrace = Expect(SyntaxKind.RBrace);
        return new MatchExpression(keyword, expression, leftBrace, rightBrace, arms);
    }

    private MatchArm ParseMatchArm()
    {
        var pattern = ParsePattern();
        Token? when = null;
        Expression? guard = null;
        if (Match(out when, SyntaxKind.WhenKeyword))
            guard = ParseExpression();

        var arrow = Expect(SyntaxKind.Arrow);
        var body = ParseExpression();
        return new MatchArm(pattern, when, guard, arrow, body);
    }

    private Pattern ParsePattern()
    {
        var pattern = ParsePrimaryPattern();
        if (!Match(out var firstPipe, SyntaxKind.Pipe))
            return pattern;

        var patterns = new List<Pattern> { pattern };
        var pipes = new List<Token> { firstPipe };
        while (true)
        {
            patterns.Add(ParsePrimaryPattern());
            if (!Match(out var pipe, SyntaxKind.Pipe))
                break;

            pipes.Add(pipe);
        }

        return new OrPattern(pipes, patterns);
    }

    private Pattern ParsePrimaryPattern()
    {
        if (Match(out var leftBrace, SyntaxKind.LBrace))
            return ParseObjectPattern(leftBrace);

        if (Match(out var letKeyword, SyntaxKind.LetKeyword))
        {
            var letName = ExpectIdentifier("binding name");
            return new LetPattern(letKeyword, letName);
        }

        if (Match(out var identifier, SyntaxKind.Identifier))
        {
            if (identifier.Text != "_" && Current() is { Kind: SyntaxKind.WhenKeyword })
            {
                var whenPosition = _position;
                Match(out var when, SyntaxKind.WhenKeyword);
                var type = ParseType();
                if (IsTypedPatternFollower())
                {
                    ObjectPattern? objectPattern = null;
                    if (Match(out var typedLeftBrace, SyntaxKind.LBrace))
                        objectPattern = ParseObjectPattern(typedLeftBrace);

                    return new TypedPattern(identifier, when!, type, objectPattern);
                }

                _position = whenPosition;
            }

            var typeArguments = ParseTypeArguments();
            if (typeArguments != null || Current() is { Kind: SyntaxKind.LBrace })
            {
                TypeExpression type = SyntaxFacts.IsPrimitiveType(identifier.Text) && typeArguments == null
                    ? new PrimitiveType(identifier)
                    : new TypeName(identifier, typeArguments);

                ObjectPattern? objectPattern = null;
                if (Match(out var typeLeftBrace, SyntaxKind.LBrace))
                    objectPattern = ParseObjectPattern(typeLeftBrace);

                return new TypePattern(type, objectPattern);
            }

            return identifier.Text == "_"
                ? new WildcardPattern(identifier)
                : new IdentifierPattern(identifier);
        }

        if (Match(out var literal, SyntaxFacts.IsLiteral))
        {
            var minimum = new LiteralPattern(literal, LiteralUtility.ResolveValue(literal));
            if (!Match(out var dotDot, SyntaxKind.DotDot))
                return minimum;

            if (!Match(out var maximumLiteral, SyntaxFacts.IsLiteral))
            {
                var badToken = Current();
                if (IsEof())
                    _diagnostics.Error(badToken, InternalCodes.UnexpectedEof, "Unexpected end of file.");
                else
                {
                    _diagnostics.Error(badToken, InternalCodes.UnexpectedToken, $"Expected range end, got {SafeTokenText(badToken)}.");
                    _position++;
                }

                return new RangePattern(minimum, dotDot, new NullPattern(badToken));
            }

            return new RangePattern(minimum, dotDot, new LiteralPattern(maximumLiteral, LiteralUtility.ResolveValue(maximumLiteral)));
        }

        var current = Current();
        if (IsEof())
            _diagnostics.Error(current, InternalCodes.UnexpectedEof, "Unexpected end of file.");
        else
        {
            _diagnostics.Error(current, InternalCodes.UnexpectedToken, $"Expected pattern, got {SafeTokenText(current)}.");
            _position++;
        }

        return new NullPattern(current);
    }

    private bool IsTypedPatternFollower() =>
        Current().Kind is SyntaxKind.Arrow
            or SyntaxKind.Comma
            or SyntaxKind.Semicolon
            or SyntaxKind.RBrace
            or SyntaxKind.Pipe
            or SyntaxKind.LBrace
            or SyntaxKind.WhenKeyword
            or SyntaxKind.Eof;

    private ObjectPattern ParseObjectPattern(Token leftBrace)
    {
        var fields = new List<ObjectPatternField>();
        if (!Match(out var rightBrace, SyntaxKind.RBrace))
        {
            while (!IsEof() && Current() is not { Kind: SyntaxKind.RBrace })
            {
                fields.Add(ParseObjectPatternField());
                Match(SyntaxKind.Comma, SyntaxKind.Semicolon);
            }

            rightBrace = Expect(SyntaxKind.RBrace);
        }

        return new ObjectPattern(leftBrace, rightBrace, fields);
    }

    private ObjectPatternField ParseObjectPatternField()
    {
        var name = ExpectIdentifier("property name");
        if (!Match(out var colon, SyntaxKind.Colon))
            return new ObjectPatternField(name, null, new IdentifierPattern(name));

        return new ObjectPatternField(name, colon, ParsePattern());
    }
}
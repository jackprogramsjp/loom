using Loom.Diagnostics;
using Loom.Parsing.AST;
using Loom.Text;

namespace Loom.Parsing;

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
            var colon = Expect(SyntaxKind.Colon);
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
        var leftParen = Expect(SyntaxKind.LParen);
        var expression = ParseExpression();
        var rightParen = Expect(SyntaxKind.RParen);
        if (expression is Name name)
            return new NameOf(keyword, leftParen, rightParen, name);

        _diagnostics.Error(expression, InternalCodes.InvalidNameOf, $"'{expression}' is not a valid name.");
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
}
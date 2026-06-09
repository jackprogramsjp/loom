using System.Diagnostics.CodeAnalysis;
using Loom.Diagnostics;
using Loom.Lexing;
using Loom.Parsing.AST;
using Loom.Syntax;

namespace Loom.Parsing;

public class Parser(LexerResult lexerResult)
{
    private record BinaryPrecedenceLevel(bool RightAssociative, Predicate<SyntaxKind> Matches)
    {
        public BinaryPrecedenceLevel(bool rightAssociative, params SyntaxKind[] kinds)
            : this(rightAssociative, kinds.Contains)
        {
        }
    }

    private static readonly BinaryPrecedenceLevel[] _binaryPrecedenceLevels =
    [
        new(true, SyntaxFacts.IsAssignmentOperator),
        new(true, SyntaxKind.QuestionQuestion),
        new(false, SyntaxKind.PipePipe),
        new(false, SyntaxKind.AmpersandAmpersand),
        new(false, SyntaxKind.Pipe),
        new(false, SyntaxKind.Tilde),
        new(false, SyntaxKind.Ampersand),
        new(false, SyntaxKind.EqualsEquals, SyntaxKind.BangEquals),
        new(false, SyntaxKind.LArrow, SyntaxKind.LArrowEquals, SyntaxKind.RArrow, SyntaxKind.RArrowEquals),
        new(false, SyntaxKind.LArrowLArrow, SyntaxKind.RArrowRArrow, SyntaxKind.RArrowRArrowRArrow),
        new(false, SyntaxKind.Plus, SyntaxKind.Minus),
        new(false, SyntaxKind.Star, SyntaxKind.Slash, SyntaxKind.SlashSlash, SyntaxKind.Percent),
        new(true, SyntaxKind.Caret),
    ];

    private readonly DiagnosticBag _diagnostics = new();
    private int _position;

    public ParserResult Parse()
    {
        var statements = new List<Statement>();
        while (!IsEof())
            statements.Add(ParseStatement());

        var tree = new Tree(lexerResult.File, statements);
        return new ParserResult(tree, _diagnostics);
    }

    private Statement ParseStatement()
    {
        if (Match(out var fnKeyword, SyntaxKind.FnKeyword))
            return ParseFunctionDeclaration(fnKeyword);

        if (Match(out var variableKeyword, SyntaxKind.LetKeyword, SyntaxKind.MutKeyword))
            return ParseVariableDeclaration(variableKeyword);

        if (Match(out var typeKeyword, SyntaxKind.TypeKeyword))
            return ParseTypeAlias(typeKeyword);

        if (Match(out var returnKeyword, SyntaxKind.ReturnKeyword))
            return new Return(returnKeyword, ParseExpression());

        var expression = ParseExpression();
        return new ExpressionStatement(expression);
    }

    private Statement ParseFunctionDeclaration(Token keyword)
    {
        var name = ExpectIdentifier();
        var typeParameters = ParseTypeParameters();
        var parameters = ParseParameters();
        var returnType = ParseColonTypeClause();

        Statement body;
        if (Match(out var leftBrace, SyntaxKind.LBrace))
            body = ParseBlock(leftBrace);
        else if (Match(out var arrow, SyntaxKind.Arrow))
            body = new ExpressionBody(arrow, ParseExpression());
        else
            body = new NullStatement(MaybeCurrent());

        if (body is not NullStatement nullStatement)
            return new FunctionDeclaration(
                keyword,
                name,
                typeParameters,
                parameters,
                returnType,
                body
            );

        _diagnostics.Error(
            nullStatement.Token ?? CurrentOrLast(),
            InternalCodes.MissingFunctionBody,
            $"Expected function body, got {SafeTokenText(nullStatement.Token)}."
        );
        return new NullStatement(nullStatement.Token);
    }

    private TypeAlias ParseTypeAlias(Token keyword)
    {
        var name = ExpectIdentifier();
        var typeParameters = ParseTypeParameters();
        var equals = Expect(SyntaxKind.Equals);
        var type = ParseType();
        var equalsTypeClause = new EqualsTypeClause(equals, type);
        return new TypeAlias(keyword, name, typeParameters, equalsTypeClause);
    }

    private VariableDeclaration ParseVariableDeclaration(Token keyword)
    {
        var name = ExpectIdentifier();
        var colonTypeClause = ParseColonTypeClause();
        var equalsValueClause = ParseEqualsValueClause();
        return new VariableDeclaration(keyword, name, colonTypeClause, equalsValueClause);
    }

    private Parameters? ParseParameters()
    {
        if (!Match(out var leftParen, SyntaxKind.LParen))
            return null;

        List<Parameter> parameters = [];
        if (!Match(SyntaxKind.RParen))
        {
            parameters = ParseDelimited(ParseParameter);
            Expect(SyntaxKind.RParen);
        }

        return new Parameters(leftParen, Last(), parameters);
    }

    private Parameter ParseParameter()
    {
        var name = ExpectIdentifier("parameter name");
        var colonTypeClause = ParseColonTypeClause();
        var equalsValueClause = ParseEqualsValueClause();
        return new Parameter(name, colonTypeClause, equalsValueClause);
    }

    private Block ParseBlock(Token leftBrace)
    {
        var statements = new List<Statement>();
        while (!Match(SyntaxKind.RBrace))
            statements.Add(ParseStatement());

        var rightBrace = Last();
        return new Block(leftBrace, rightBrace, statements);
    }

    private EqualsValueClause? ParseEqualsValueClause() => Match(out var equals, SyntaxKind.Equals) ? new EqualsValueClause(equals, ParseExpression()) : null;
    private ColonTypeClause? ParseColonTypeClause() => Match(out var colon, SyntaxKind.Colon) ? new ColonTypeClause(colon, ParseType()) : null;
    private EqualsTypeClause? ParseEqualsTypeClause() => Match(out var equals, SyntaxKind.Equals) ? new EqualsTypeClause(equals, ParseType()) : null;
    private Expression ParseExpression() => ParseBinaryLevel(0);

    private Expression ParseBinaryLevel(int level)
    {
        if (level >= _binaryPrecedenceLevels.Length)
            return ParseUnary();

        var (rightAssociative, matches) = _binaryPrecedenceLevels[level];
        var left = ParseBinaryLevel(level + 1);
        while (Match(out var op, matches))
        {
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
                var typeArguments = ParseTypeArguments(forFunction: true);
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
            else
            {
                // TODO: postfix unary operators
                break;
            }
        }
        
        return expression;
    }

    private Arguments ParseArguments(Token leftParen)
    {
        if (Match(out var matchedRightParen, SyntaxKind.RParen))
            return new Arguments(leftParen, matchedRightParen, []);

        var argumentList = ParseDelimited(ParseExpression);
        var rightParen = Expect(SyntaxKind.RParen);
        return new Arguments(leftParen, rightParen, argumentList);
    }

    private Expression ParsePrimary()
    {
        if (Match(out var leftParen, SyntaxKind.LParen))
        {
            var expression = ParseExpression();
            var rightParen = Expect(
                SyntaxKind.RParen,
                got => $"Expected ')' here to close '{leftParen.Text}' at character {leftParen.Span.Start.Character}, got {SafeTokenText(got)}."
            );

            return new Parenthesized(leftParen, rightParen, expression);
        }

        if (Match(out var mutKeyword, SyntaxKind.MutKeyword) && ParseArrayLiteral(mutKeyword) is { } mutableArrayLiteral)
            return mutableArrayLiteral;

        if (ParseArrayLiteral() is { } arrayLiteral)
            return arrayLiteral;

        if (Match(out var name, SyntaxKind.Identifier))
            return new Identifier(name);

        if (Match(out var token, SyntaxFacts.IsLiteral))
            return new Literal(token, LiteralUtility.ResolveValue(token));

        var currentOrLast = CurrentOrLast();
        _diagnostics.Error(currentOrLast.Span, InternalCodes.UnexpectedToken, "Unexpected token.");
        _position++;
        return new NullExpression(currentOrLast);
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

    private TypeExpression ParseType() => ParseUnionType();

    private TypeExpression ParseUnionType() => ParseChainedType(ParseIntersectionType, SyntaxKind.Pipe, (separators, types) => new UnionType(separators, types));

    private TypeExpression ParseIntersectionType() =>
        ParseChainedType(ParsePostfixType, SyntaxKind.Ampersand, (separators, types) => new IntersectionType(separators, types));

    private TypeExpression ParsePostfixType()
    {
        var type = ParsePrimaryType();
        while (true)
        {
            if (Match(out var leftBracket, SyntaxKind.LBracket))
            {
                var mutKeyword = Match(out var mutToken, SyntaxKind.MutKeyword) ? mutToken : null;
                var rightBracket = Expect(SyntaxKind.RBracket);
                type = new ArrayType(type, leftBracket, mutKeyword, rightBracket);
            }
            else if (Match(out var question, SyntaxKind.Question))
            {
                type = new OptionalType(question, type);
            }
            else
            {
                break;
            }
        }
    
        return type;
    }

    private TypeExpression ParsePrimaryType()
    {
        if (Match(out var leftParen, SyntaxKind.LParen))
        {
            var type = ParseType();
            var rightParen = Expect(
                SyntaxKind.RParen,
                got => $"Expected ')' here to close '{leftParen.Text}' at character {leftParen.Span.Start.Character}, got {SafeTokenText(got)}."
            );

            return new ParenthesizedType(leftParen, rightParen, type);
        }

        if (Match(out var token, SyntaxFacts.IsLiteral))
            return new LiteralType(token, LiteralUtility.ResolveValue(token));

        var name = ExpectIdentifier("type");
        if (SyntaxFacts.IsPrimitiveType(name.Text))
            return new PrimitiveType(name);

        var typeArguments = ParseTypeArguments();
        return new TypeName(name, typeArguments);
    }

    private TypeExpression ParseChainedType(
        Func<TypeExpression> parseInner,
        SyntaxKind separator,
        Func<List<Token>, List<TypeExpression>, TypeExpression> create)
    {
        var types = new List<TypeExpression> { parseInner() };
        var separators = new List<Token>();
        while (Match(out var token, separator))
        {
            separators.Add(token);
            types.Add(parseInner());
        }

        return separators.Count > 0 ? create(separators, types) : types[0];
    }

    private TypeParameters? ParseTypeParameters()
    {
        if (!Match(out var leftArrow, SyntaxKind.LArrow))
            return null;

        var parameters = ParseDelimited(ParseTypeParameter);
        var rightArrow = Expect(SyntaxKind.RArrow);
        return new TypeParameters(leftArrow, rightArrow, parameters);
    }

    private TypeParameter ParseTypeParameter()
    {
        var name = ExpectIdentifier("type parameter name");
        var constraint = ParseColonTypeClause();
        var equalsTypeClause = ParseEqualsTypeClause();
        return new TypeParameter(name, constraint, equalsTypeClause);
    }

    private TypeArguments? ParseTypeArguments(bool forFunction = false)
    {
        if (!Match(out var leftArrow, forFunction ? SyntaxKind.ColonColonLArrow : SyntaxKind.LArrow))
            return null;

        var arguments = ParseDelimited(ParseType);
        if (ExpectClosingArrow(out var rightArrow))
            return new TypeArguments(leftArrow, rightArrow, arguments);

        var token = CurrentOrLast();
        _diagnostics.Error(
            token,
            InternalCodes.UnexpectedToken,
            $"Expected '>', got '{token.Text}'."
        );

        return null;
    }

    // evil token splitting function
    private bool ExpectClosingArrow([MaybeNullWhen(false)] out Token closingArrow)
    {
        closingArrow = null;
        if (IsEof())
            return false;

        var token = Current();
        switch (token.Kind)
        {
            case SyntaxKind.RArrow:
                closingArrow = Advance();
                return true;
            case SyntaxKind.RArrowRArrow:
            {
                var firstSpan = new LocationSpan(token.Span.Start, 1);
                var firstToken = new Token(SyntaxKind.RArrow, firstSpan, ">");
                var remainderSpan = new LocationSpan(token.Span.Start + 1, token.Span.Length - 1);
                var remainderToken = new Token(SyntaxKind.RArrow, remainderSpan, token.Text[1..]);
                lexerResult.Tokens[_position] = firstToken;
                lexerResult.Tokens.Insert(_position + 1, remainderToken);
                Advance();

                closingArrow = firstToken;
                return true;
            }
            case SyntaxKind.RArrowRArrowRArrow:
            {
                var firstSpan = new LocationSpan(token.Span.Start, 1);
                var firstToken = new Token(SyntaxKind.RArrow, firstSpan, ">");
                var remainderSpan = new LocationSpan(token.Span.Start + 1, token.Span.Length - 1);
                var remainderToken = new Token(SyntaxKind.RArrowRArrow, remainderSpan, token.Text[1..]);
                lexerResult.Tokens[_position] = firstToken;
                lexerResult.Tokens.Insert(_position + 1, remainderToken);
                Advance();

                closingArrow = firstToken;
                return true;
            }
            default:
                return false;
        }
    }

    private List<T> ParseDelimited<T>(Func<T> parse, SyntaxKind delimiter = SyntaxKind.Comma)
        where T : Node
    {
        var nodes = new List<T> { parse() };
        while (Match(delimiter))
            nodes.Add(parse());

        return nodes;
    }

    private bool Match([MaybeNullWhen(false)] out Token token, params SyntaxKind[] kinds) => Match(out token, kinds.Contains);
    private bool Match(SyntaxKind kind) => Match(out _, kind);
    private bool Match(params SyntaxKind[] kinds) => Match(out _, kinds);
    private bool Match([MaybeNullWhen(false)] out Token token, SyntaxKind kind) => Match(out token, otherKind => otherKind == kind);

    private bool Match([MaybeNullWhen(false)] out Token token, Predicate<SyntaxKind> predicate)
    {
        if (IsEof())
        {
            token = null;
            return false;
        }

        token = Current();
        var match = predicate(token.Kind);
        if (match)
            Advance();

        return match;
    }

    private Token ExpectIdentifier(string expected = "identifier") => Expect(SyntaxKind.Identifier, token => $"Expected {expected}, got {SafeTokenText(token)}.");

    private Token Expect(SyntaxKind kind, Func<Token?, string>? message = null)
    {
        if (IsEof())
        {
            var last = Last();
            var text = SyntaxFacts.GetText(kind) ?? kind.ToString();
            _diagnostics.Error(last, InternalCodes.UnexpectedEof, message != null ? message(null) : $"Expected '{text}', got EOF.");
            return last;
        }

        var token = Advance();
        if (token.Kind == kind)
            return token;

        var expected = SyntaxFacts.GetText(kind) ?? kind.ToString();
        _diagnostics.Error(
            token,
            InternalCodes.UnexpectedToken,
            message != null ? message(token) : $"Expected '{expected}', got '{SafeTokenText(token)}'."
        );

        return token;
    }

    private Token Advance()
    {
        var current = Current();
        _position++;
        return current;
    }

    private Token? MaybeCurrent() => IsEof() ? null : Current();
    private Token Current() => Peek(0);
    private Token CurrentOrLast() => !IsEof() ? Current() : Last();
    private Token Last() => Peek(-1);
    private Token Peek(int offset) => lexerResult.Tokens[_position + offset];
    private bool IsEof() => _position >= lexerResult.Tokens.Count;
    private static string SafeTokenText(Token? token) => token != null ? $"'{token.Text}'" : "EOF";
}
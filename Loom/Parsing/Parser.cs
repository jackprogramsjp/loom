using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;
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
        if (Match(out var variableKeyword, SyntaxKind.LetKeyword, SyntaxKind.MutKeyword))
            return ParseVariableDeclaration(variableKeyword);

        if (Match(out var typeKeyword, SyntaxKind.TypeKeyword))
            return ParseTypeAlias(typeKeyword);

        var expression = ParseExpression();
        return new ExpressionStatement(expression);
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
        var equalsTypeClause = ParseEqualsTypeClause();
        return new TypeParameter(name, equalsTypeClause);
    }

    private EqualsTypeClause? ParseEqualsTypeClause() => Match(out var equals, SyntaxKind.Equals) ? new EqualsTypeClause(equals, ParseType()) : null;

    private VariableDeclaration ParseVariableDeclaration(Token keyword)
    {
        var name = ExpectIdentifier();
        ColonTypeClause? colonTypeClause = null;
        EqualsValueClause? equalsValueClause = null;
        if (Match(out var colon, SyntaxKind.Colon))
        {
            var type = ParseType();
            colonTypeClause = new ColonTypeClause(colon, type);
        }

        if (Match(out var equals, SyntaxKind.Equals))
        {
            var initializer = ParseExpression();
            equalsValueClause = new EqualsValueClause(equals, initializer);
        }

        return new VariableDeclaration(keyword, name, colonTypeClause, equalsValueClause);
    }

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
            : ParsePrimary();

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

        if (Match(out var name, SyntaxKind.Identifier))
            return new Identifier(name);

        if (Match(out var token, SyntaxFacts.IsLiteral))
            return new Literal(token, Literal.ResolveValue(token));

        var last = Last();
        _diagnostics.Error(last.Span, InternalCodes.UnexpectedToken, "Unexpected token.");
        _position++;
        return new NullExpression(last);
    }

    private TypeExpression ParseType() => ParseUnionType();

    private TypeExpression ParseUnionType() => ParseChainedType(ParseIntersectionType, SyntaxKind.Pipe, (separators, types) => new UnionType(separators, types));

    private TypeExpression ParseIntersectionType() =>
        ParseChainedType(ParseOptionalType, SyntaxKind.Ampersand, (separators, types) => new IntersectionType(separators, types));

    private TypeExpression ParseOptionalType()
    {
        var type = ParsePrimaryType();
        return Match(out var question, SyntaxKind.Question)
            ? new OptionalType(question, type)
            : type;
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

    private TypeArguments? ParseTypeArguments()
    {
        if (!Match(out var leftArrow, SyntaxKind.LArrow))
            return null;

        var arguments = ParseDelimited(ParseType);
        var rightArrow = Expect(SyntaxKind.RArrow);
        return new TypeArguments(leftArrow, rightArrow, arguments);
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
            _diagnostics.Error(last.Span, InternalCodes.UnexpectedEof, message != null ? message(null) : $"Expected '{text}', got end of file.");
            return last;
        }

        var token = Advance();
        if (token.Kind == kind)
            return token;

        var expected = SyntaxFacts.GetText(kind) ?? kind.ToString();
        _diagnostics.Error(
            token.Span,
            InternalCodes.UnexpectedToken,
            message != null ? message(token) : $"Expected '{expected}', got '{token.Text}'."
        );

        return token;
    }

    private Token Advance()
    {
        var current = Current();
        _position++;
        return current;
    }

    private Token Current() => Peek(0);
    private Token Last() => Peek(-1);
    private Token Peek(int offset) => lexerResult.Tokens[_position + offset];
    private bool IsEof() => _position >= lexerResult.Tokens.Count;
    private static string SafeTokenText(Token? token) => token != null ? $"'{token.Text}'" : "EOF";
}
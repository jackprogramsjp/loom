using System.Diagnostics.CodeAnalysis;
using Loom.Diagnostics;
using Loom.Lexing;
using Loom.Parsing.AST;
using Loom.Text;

namespace Loom.Parsing;

public sealed class Parser
{
    private delegate Statement StatementParser(Token keyword);

    private readonly DiagnosticBag _diagnostics = new();
    private readonly Dictionary<SyntaxKind, StatementParser> _statementParsers;
    private readonly LexerResult _lexerResult;
    private int _position;

    public Parser(LexerResult lexerResult)
    {
        _lexerResult = lexerResult;
        _statementParsers = new Dictionary<SyntaxKind, StatementParser>
        {
            [SyntaxKind.LBrace] = ParseBlock,
            [SyntaxKind.ReturnKeyword] = token => new Return(token, ParseExpression()),
            [SyntaxKind.FnKeyword] = ParseFunctionDeclaration,
            [SyntaxKind.LetKeyword] = ParseVariableDeclaration,
            [SyntaxKind.MutKeyword] = ParseVariableDeclaration,
            [SyntaxKind.TypeKeyword] = ParseTypeAlias,
            [SyntaxKind.EnumKeyword] = ParseEnumDeclaration,
            [SyntaxKind.DeclareKeyword] = ParseDeclareStatement,
            [SyntaxKind.InterfaceKeyword] = ParseInterfaceDeclaration,
            [SyntaxKind.SealedKeyword] = ParseInterfaceDeclaration,
            [SyntaxKind.IfKeyword] = ParseIf,
            [SyntaxKind.ForKeyword] = ParseFor,
            [SyntaxKind.AfterKeyword] = ParseAfter,
            [SyntaxKind.WhileKeyword] = ParseWhile,
            [SyntaxKind.BreakKeyword] = ParseBreak,
            [SyntaxKind.ContinueKeyword] = ParseContinue,
        };
    }

    public ParserResult Parse()
    {
        var statements = new List<Statement>();
        while (!IsEof())
            statements.Add(ParseStatement());

        var tree = new Tree(_lexerResult.File, statements);
        return new ParserResult(tree, _diagnostics);
    }

    private Statement ParseStatement()
    {
        foreach (var (kind, parse) in _statementParsers)
        {
            if (!Match(out var token, kind)) continue;
            return parse(token);
        }

        var expression = ParseExpression();
        return new ExpressionStatement(expression);
    }

    private InterfaceDeclaration ParseInterfaceDeclaration(Token keyword)
    {
        var isSealed = keyword.Kind == SyntaxKind.SealedKeyword;
        var interfaceKeyword = isSealed ? Expect(SyntaxKind.InterfaceKeyword) : keyword;
        var sealedKeyword = isSealed ? keyword : null;
        var name = ExpectIdentifier("interface name");
        var typeParameters = ParseTypeParameters();
        var colonTypeListClause = ParseColonTypeListClause();
        var body = ParseInterfaceBody();
        return new InterfaceDeclaration(
            sealedKeyword,
            interfaceKeyword,
            name,
            typeParameters,
            colonTypeListClause,
            body
        );
    }

    private InterfaceBody? ParseInterfaceBody()
    {
        if (!Match(out var leftBrace, SyntaxKind.LBrace))
            return null;

        var members = ParseInterfaceMembers();
        if (members == null)
            return null;

        var rightBrace = Expect(SyntaxKind.RBrace);
        return new InterfaceBody(leftBrace, rightBrace, members);
    }

    private List<InterfaceMember>? ParseInterfaceMembers()
    {
        var members = new List<InterfaceMember>();
        while (!IsEof() && Current() is not { Kind: SyntaxKind.RBrace })
        {
            var member = ParseInterfaceMember(Match(out var mutKeyword, SyntaxKind.MutKeyword) ? mutKeyword : null);
            if (member == null) return null;
            members.Add(member);
            Match(SyntaxKind.Comma);
        }

        return members;
    }

    private InterfaceMember? ParseInterfaceMember(Token? mutKeyword)
    {
        if (Match(out var leftBracket, SyntaxKind.LBracket))
        {
            var indexType = ParseType();
            var rightBracket = Expect(SyntaxKind.RBracket);
            var colonTypeClause = ExpectInterfaceMemberColonTypeClause($"Expected indexer type, got {SafeTokenText(MaybeCurrent())}.");
            return colonTypeClause == null ? null : new IndexerDeclaration(mutKeyword, leftBracket, rightBracket, indexType, colonTypeClause);
        }

        var name = ExpectIdentifier("property name");
        var propertyType = ExpectInterfaceMemberColonTypeClause($"Expected indexer type, got {SafeTokenText(MaybeCurrent())}.");
        return propertyType == null ? null : new PropertyDeclaration(mutKeyword, name, propertyType);
    }

    private ColonTypeClause? ExpectInterfaceMemberColonTypeClause(string message)
    {
        var colonTypeClause = ParseColonTypeClause();
        if (colonTypeClause != null)
            return colonTypeClause;

        var token = CurrentOrLast();
        _diagnostics.Error(token, InternalCodes.ExpectedInterfaceMemberType, message);
        return null;
    }

    private Statement ParseFor(Token keyword)
    {
        var variableKeyword = Match(out var letKeyword, SyntaxKind.LetKeyword) ? letKeyword : Expect(SyntaxKind.MutKeyword, "variable signature");
        var declaration = ParseDeclareVariableSignature(variableKeyword);
        if (variableKeyword.Kind is not (SyntaxKind.LetKeyword or SyntaxKind.MutKeyword) || declaration is not DeclareVariableSignature signature)
            return new NullStatement(variableKeyword);

        var inKeyword = Expect(SyntaxKind.InKeyword);
        var expression = ParseExpression();
        var body = ParseStatement();
        return new For(keyword, signature, inKeyword, expression, body);
    }

    private After ParseAfter(Token keyword)
    {
        var condition = ParseExpression();
        var body = ParseControlFlowBody(keyword);
        return new After(keyword, condition, body);
    }

    private static Break ParseBreak(Token keyword) => new(keyword);
    private static Continue ParseContinue(Token keyword) => new(keyword);

    private While ParseWhile(Token keyword)
    {
        var condition = ParseExpression();
        var body = ParseControlFlowBody(keyword);
        return new While(keyword, condition, body);
    }

    private If ParseIf(Token keyword)
    {
        var condition = ParseExpression();
        var thenBranch = ParseControlFlowBody(keyword);
        var elseBranch = Match(out var elseKeyword, SyntaxKind.ElseKeyword) ? new ElseBranch(elseKeyword, ParseControlFlowBody(keyword)) : null;
        return new If(keyword, condition, thenBranch, elseBranch);
    }
    
    private Statement ParseControlFlowBody(Token keyword)
    {
        var statement = ParseStatement();
        return AssertDeclarationInsideOfBlock(statement) ? statement : new NullStatement(keyword);
    }

    private Statement ParseDeclareStatement(Token declareKeyword)
    {
        var statement = ParseDeclareSignature(declareKeyword);
        if (statement is not DeclareSignature signature)
            return statement;

        return new Declare(declareKeyword, signature);
    }

    private Statement ParseDeclareSignature(Token declareKeyword)
    {
        if (Match(out var fnKeyword, SyntaxKind.FnKeyword))
            return ParseDeclareFunctionSignature(fnKeyword);

        if (Match(out var variableKeyword, SyntaxKind.LetKeyword, SyntaxKind.MutKeyword))
            return ParseDeclareVariableSignature(variableKeyword);

        if (Match(out var interfaceKeyword, SyntaxKind.InterfaceKeyword, SyntaxKind.SealedKeyword))
            return ParseInterfaceDeclaration(interfaceKeyword);

        _diagnostics.Error(
            MaybeCurrent() ?? declareKeyword,
            InternalCodes.ExpectedDeclarationSignature,
            $"Expected declaration signature, got {SafeTokenText(MaybeCurrent())}."
        );

        return new NullStatement(declareKeyword);
    }

    private DeclareVariableSignature ParseDeclareVariableSignature(Token variableKeyword)
    {
        var name = ExpectIdentifier();
        var colonTypeClause = ParseColonTypeClause();
        return new DeclareVariableSignature(variableKeyword, name, colonTypeClause);
    }

    private Statement ParseDeclareFunctionSignature(Token fnKeyword)
    {
        var name = ExpectIdentifier();
        var typeParameters = ParseTypeParameters();
        var parameters = ParseParameters();
        var returnType = ParseColonTypeClause();
        if (!ValidateFunctionSignature("declared function signatures", parameters?.Span ?? typeParameters?.Span ?? name.Span, returnType, parameters))
            return new NullStatement(fnKeyword);

        return new DeclareFunctionSignature(fnKeyword, name, typeParameters, parameters, returnType);
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

    private EnumDeclaration ParseEnumDeclaration(Token keyword)
    {
        var name = ExpectIdentifier();
        var colonTypeClause = ParseColonTypeClause();
        var leftBrace = Expect(SyntaxKind.LBrace);
        var members = !IsEof() && Current() is { Kind: SyntaxKind.Identifier } ? ParseDelimited(ParseEnumMember).OfType<EnumMember>().ToList() : [];
        var rightBrace = Expect(SyntaxKind.RBrace);
        return new EnumDeclaration(
            keyword,
            name,
            leftBrace,
            rightBrace,
            colonTypeClause,
            members
        );
    }

    private EnumMember? ParseEnumMember() => Match(out var name, SyntaxKind.Identifier) ? new EnumMember(name, ParseEqualsValueClause()) : null;

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

    private ColonTypeListClause? ParseColonTypeListClause() =>
        Match(out var colon, SyntaxKind.Colon) ? new ColonTypeListClause(colon, ParseDelimited(ParseType)) : null;

    private EqualsTypeClause? ParseEqualsTypeClause() => Match(out var equals, SyntaxKind.Equals) ? new EqualsTypeClause(equals, ParseType()) : null;
    private Expression ParseExpression() => ParseBinaryLevel(0);

    private Expression ParseBinaryLevel(int level)
    {
        if (level >= BinaryPrecedenceLevel.Levels.Length)
            return ParseRange();

        var (rightAssociative, matches) = BinaryPrecedenceLevel.Levels[level];
        var left = ParseBinaryLevel(level + 1);
        while (Match(out var op, matches))
        {
            if (op.Kind == SyntaxKind.AsKeyword)
            {
                var type = ParseType();
                left = new AsExpression(op, left, type);
                continue;
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
        var expression = ParseNamedAccess();
        if (!Match(out var dotDot, SyntaxKind.DotDot))
            return expression;

        var maximum = ParseNamedAccess();
        return new RangeLiteral(dotDot, expression, maximum);
    }

    private Expression ParseNamedAccess()
    {
        var expression = ParseUnary();
        var names = new List<DotName>();
        while (Match(out var dot, SyntaxKind.Dot))
            names.Add(new DotName(dot, Expect(SyntaxKind.Identifier)));

        if (names.Count <= 0)
            return expression;

        return expression is Identifier identifier
            ? new QualifiedName(identifier, names)
            : new PropertyAccess(expression, names);
    }

    private Expression ParseUnary()
    {
        if (!Match(out var newKeyword, SyntaxKind.NewKeyword))
            return Match(out var op, SyntaxFacts.IsUnaryOperator)
                ? new UnaryOperator(op, ParseUnary())
                : ParsePostfix();

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
        return new InterfaceInvocation(newKeyword, name, typeArguments, body);
    }

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
            else
            {
                // TODO: postfix unary operators
                break;
            }
        }

        return expression;
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

    private Expression ParsePrimary()
    {
        if (Match(out var openingParen, SyntaxKind.LParen))
        {
            var expression = ParseExpression();
            var rightParen = Expect(
                SyntaxKind.RParen,
                got => $"Expected ')' here to close '{openingParen.Text}' at character {openingParen.Span.Start.Character}, got {SafeTokenText(got)}."
            );

            return new Parenthesized(openingParen, rightParen, expression);
        }

        if (Match(out var mutKeyword, SyntaxKind.MutKeyword) && ParseArrayLiteral(mutKeyword) is { } mutableArrayLiteral)
            return mutableArrayLiteral;

        if (ParseArrayLiteral() is { } arrayLiteral)
            return arrayLiteral;

        if (Match(out var nameOfKeyword, SyntaxKind.NameOfKeyword))
        {
            var leftParen = Expect(SyntaxKind.LParen);
            var expression = ParseExpression();
            var rightParen = Expect(SyntaxKind.RParen);
            if (expression is Name name)
                return new NameOf(nameOfKeyword, leftParen, rightParen, name);

            _diagnostics.Error(expression, InternalCodes.InvalidNameOf, $"'{expression}' is not a valid name.");
            return new NullExpression(nameOfKeyword);
        }

        if (Match(out var nameToken, SyntaxKind.Identifier))
            return new Identifier(nameToken);

        if (Match(out var token, SyntaxFacts.IsLiteral))
            return new Literal(token, LiteralUtility.ResolveValue(token));

        var currentOrLast = CurrentOrLast();
        if (IsEof())
        {
            _diagnostics.Error(currentOrLast, InternalCodes.UnexpectedEof, "Unexpected end of file.");
        }
        else
        {
            _diagnostics.Error(currentOrLast, InternalCodes.UnexpectedToken, $"Expected expression, got {SafeTokenText(MaybeCurrent())}.");
            _position++;
        }

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
                if (Match(out var immediateRightBracket, SyntaxKind.RBracket))
                {
                    type = new ArrayType(type, leftBracket, null, immediateRightBracket);
                    continue;
                }

                if (Match(out var mutKeyword, SyntaxKind.MutKeyword))
                {
                    var arrayRightBracket = Expect(SyntaxKind.RBracket);
                    type = new ArrayType(type, leftBracket, mutKeyword, arrayRightBracket);
                    continue;
                }

                var indexType = ParseType();
                var rightBracket = Expect(SyntaxKind.RBracket);
                type = new IndexedType(leftBracket, rightBracket, type, indexType);
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

    private TypeExpression ParseFunctionType(Token fnKeyword)
    {
        var typeParameters = ParseTypeParameters();
        var parameters = ParseParameters();
        var returnType = ParseColonTypeClause();
        if (!ValidateFunctionSignature("function types", parameters?.Span ?? typeParameters?.Span ?? fnKeyword.Span, returnType, parameters))
            return new NullTypeExpression(fnKeyword);

        return new FunctionType(fnKeyword, typeParameters, parameters, returnType);
    }

    private bool ValidateFunctionSignature(string kind, LocationSpan span, [NotNullWhen(true)] ColonTypeClause? returnType, Parameters? parameters)
    {
        if (returnType == null)
        {
            _diagnostics.Error(
                span,
                InternalCodes.MissingDeclareFnReturnType,
                $"{(kind.Length > 0 ? char.ToUpperInvariant(kind[0]) + kind[1..] : kind)} must have a return type."
            );

            return false;
        }

        var parameterWithDefault = parameters?.ParameterList.Find(p => p.EqualsValueClause != null);
        if (parameterWithDefault != null)
        {
            _diagnostics.Error(
                parameterWithDefault,
                InternalCodes.UseOfDeclareFnParameterDefaults,
                $"Parameters may not have default values in {kind}."
            );

            return false;
        }

        var parameterWithoutType = parameters?.ParameterList.Find(p => p.ColonTypeClause == null);
        if (parameterWithoutType == null)
            return true;

        _diagnostics.Error(
            parameterWithoutType,
            InternalCodes.MissingDeclareFnParameterType,
            $"Parameters must have types in {kind}."
        );

        return false;
    }

    private TypeExpression ParsePrimaryType()
    {
        if (Match(out var fnKeyword, SyntaxKind.FnKeyword))
            return ParseFunctionType(fnKeyword);

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

    private TypeArguments? ParseTypeArguments(bool forInvocation = false)
    {
        if (!Match(out var leftArrow, forInvocation ? SyntaxKind.ColonColonLArrow : SyntaxKind.LArrow))
            return null;

        var arguments = ParseDelimited(ParseType);
        if (MatchClosingArrow(out var rightArrow))
            return new TypeArguments(leftArrow, rightArrow, arguments);

        var token = CurrentOrLast();
        _diagnostics.Error(
            token,
            InternalCodes.UnexpectedToken,
            $"Expected '>', got '{token.Text}'."
        );

        return null;
    }

    private bool AssertDeclarationInsideOfBlock(Statement statement)
    {
        if (statement is not NamedDeclaration namedDeclaration)
            return true;

        _diagnostics.Error(
            namedDeclaration,
            InternalCodes.DeclarationOutsideOfBlock,
            "Declarations can only be declared inside of a block.",
            "surround with '{' and '}'"
        );

        return false;
    }

    private bool MatchClosingArrow([MaybeNullWhen(false)] out Token closingArrow)
    {
        closingArrow = null;
        if (IsEof())
            return false;

        return Current().Kind switch
        {
            SyntaxKind.RArrow => (closingArrow = Advance()) != null,
            SyntaxKind.RArrowRArrow => SplitAndAdvance(1, SyntaxKind.RArrow, out closingArrow),
            SyntaxKind.RArrowRArrowRArrow => SplitAndAdvance(1, SyntaxKind.RArrowRArrow, out closingArrow),
            _ => false
        };
    }

    // evil token splitting function
    private bool SplitAndAdvance(int splitIndex, SyntaxKind remainderKind, out Token closingArrow)
    {
        var token = Current();
        var firstSpan = new LocationSpan(token.Span.Start, splitIndex);
        closingArrow = new Token(SyntaxKind.RArrow, firstSpan, token.Text[..splitIndex]);

        var remainder = new Token(
            remainderKind,
            new LocationSpan(token.Span.Start + splitIndex, token.Span.Length - splitIndex),
            token.Text[splitIndex..]
        );

        _lexerResult.Tokens[_position] = closingArrow;
        _lexerResult.Tokens.Insert(_position + 1, remainder);
        Advance();
        return true;
    }

    private List<T> ParseDelimited<T>(Func<T> parse, SyntaxKind delimiter = SyntaxKind.Comma)
        where T : Node?
    {
        var first = parse();
        if (first == null)
            return [];

        var nodes = new List<T> { first };
        while (Match(delimiter))
        {
            var node = parse();
            if (node == null) continue;
            nodes.Add(node);
        }

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

    private Token ExpectIdentifier(string expected = "identifier") => Expect(SyntaxKind.Identifier, expected);
    private Token Expect(SyntaxKind kind, string expected) => Expect(kind, token => $"Expected {expected}, got {SafeTokenText(token)}.");

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
            message != null ? message(token) : $"Expected '{expected}', got {SafeTokenText(token)}."
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
    private Token Peek(int offset) => _lexerResult.Tokens[_position + offset];
    private bool IsEof() => _position >= _lexerResult.Tokens.Count;
    private static string SafeTokenText(Token? token) => token != null ? $"'{token.Text}'" : "EOF";
}
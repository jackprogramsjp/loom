using Loom.Diagnostics;
using Loom.Parsing.AST;
using Loom.Text;

namespace Loom.Parsing;

public sealed partial class Parser
{
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
            var colonTypeClause = ExpectInterfaceMemberColonTypeClause($"Expected indexer type, got {SafeTokenText(Current())}.");
            return colonTypeClause == null ? null : new IndexerDeclaration(mutKeyword, leftBracket, rightBracket, indexType, colonTypeClause);
        }

        var name = ExpectIdentifier("property name");
        var propertyType = ExpectInterfaceMemberColonTypeClause($"Expected indexer type, got {SafeTokenText(Current())}.");
        return propertyType == null ? null : new PropertyDeclaration(mutKeyword, name, propertyType);
    }

    private ColonTypeClause? ExpectInterfaceMemberColonTypeClause(string message)
    {
        var colonTypeClause = ParseColonTypeClause();
        if (colonTypeClause != null)
            return colonTypeClause;

        _diagnostics.Error(Current(), InternalCodes.ExpectedInterfaceMemberType, message);
        return null;
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
            Current() ?? declareKeyword,
            InternalCodes.ExpectedDeclarationSignature,
            $"Expected declaration signature, got {SafeTokenText(Current())}."
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
            body = new NullStatement(Current());

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
            nullStatement.Token ?? Current(),
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
    
    private EqualsValueClause? ParseEqualsValueClause() => Match(out var equals, SyntaxKind.Equals) ? new EqualsValueClause(equals, ParseExpression()) : null;
    private ColonTypeClause? ParseColonTypeClause() => Match(out var colon, SyntaxKind.Colon) ? new ColonTypeClause(colon, ParseType()) : null;

    private ColonTypeListClause? ParseColonTypeListClause() =>
        Match(out var colon, SyntaxKind.Colon) ? new ColonTypeListClause(colon, ParseDelimited(ParseType)) : null;

    private EqualsTypeClause? ParseEqualsTypeClause() => Match(out var equals, SyntaxKind.Equals) ? new EqualsTypeClause(equals, ParseType()) : null;
}
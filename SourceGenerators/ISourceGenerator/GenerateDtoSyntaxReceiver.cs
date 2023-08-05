using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SourceGenerators.ISourceGenerator;

public class GenerateDtoSyntaxReceiver : ISyntaxReceiver
{
    public List<BaseTypeDeclarationSyntax> CandidateTypes { get; } = new();

    public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
    {
        if (syntaxNode is BaseTypeDeclarationSyntax typeDeclarationSyntax
            && typeDeclarationSyntax.AttributeLists.Any(
                attributeListSyntax => attributeListSyntax.Attributes.Any(
                    attributeSyntax => attributeSyntax.Name.ToString() == "GenerateDto")))
        {
            CandidateTypes.Add(typeDeclarationSyntax);
        }
    }
}
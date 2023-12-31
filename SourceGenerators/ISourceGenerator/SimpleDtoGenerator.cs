using System;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace SourceGenerators.ISourceGenerator;

[Generator]
public class SimpleDtoGenerator : Microsoft.CodeAnalysis.ISourceGenerator
{
    public void Initialize(GeneratorInitializationContext context)
    {
        context.RegisterForSyntaxNotifications(() => new GenerateDtoSyntaxReceiver());
        context.RegisterForPostInitialization(i =>i.AddSource("Attributes/GenerateDtoAttribute.g.cs", SourceText.From("""
            // <auto-generated/>
            using System;
            
            namespace Generators;

            [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false)]
            public class GenerateDtoAttribute : Attribute
            {
            }
            """, Encoding.UTF8)));
    }

    public void Execute(GeneratorExecutionContext context)
    {
        // Recherche des classes avec l'attribut GenerateDto
        if (context.SyntaxReceiver is not GenerateDtoSyntaxReceiver receiver)
            return;

        var classesWithAttributeGenerateDto = receiver.CandidateTypes;

        foreach (var classWithAttributeGenerateDto in classesWithAttributeGenerateDto)
        {
            // Récupération des using
            var usingDirectives = classWithAttributeGenerateDto.SyntaxTree.GetRoot()
                .DescendantNodesAndSelf()
                .OfType<UsingDirectiveSyntax>()
                .Select(usingDirective => usingDirective.ToFullString());

            var sourceBuilder = new StringBuilder().AppendLine("// <auto-generated />");
            foreach (var usingDirective in usingDirectives)
            {
                sourceBuilder.AppendLine(usingDirective);
            }

            // Récupération du namespace
            var semanticModel = context.Compilation.GetSemanticModel(classWithAttributeGenerateDto.SyntaxTree);
            var classSymbol = semanticModel.GetDeclaredSymbol(classWithAttributeGenerateDto);
            var namespaceName = classSymbol?.ContainingNamespace.ToDisplayString() ?? "Generators";

            sourceBuilder.AppendLine($@"#nullable enable
namespace {namespaceName};
");
            
            // Récupération de la classe et des propriétés
            var className = classWithAttributeGenerateDto.Identifier.Text;
            var dtoClassName = $"{className}Dto";
            var properties = classWithAttributeGenerateDto switch
            {
                ClassDeclarationSyntax classDeclaration => classDeclaration.Members.OfType<PropertyDeclarationSyntax>()
                    .Select(p => (p.Identifier.Text, p.Type.ToString())).ToArray(),
                
                RecordDeclarationSyntax recordDeclaration => recordDeclaration.ParameterList?.Parameters
                    .Select(p => (p.Identifier.Text, p.Type.ToString())).ToArray(),
                
                _ => throw new InvalidOperationException("Unsupported type declaration.")
            };
            
            sourceBuilder.AppendLine($@"public record {dtoClassName}({string.Join(", ", properties.Select(p => $"{p.Item2} {p.Text}"))})
{{");
            
            // Génération du mapping
            if(properties.Any())
            {
                sourceBuilder.AppendLine($"    public static explicit operator {dtoClassName}({className} model)" +
                                         $" => new({string.Join(", ", properties.Select(p => $"model.{p.Text}"))});");

                sourceBuilder.Append($"    public static explicit operator {className}({dtoClassName} dto)" +
                                     $" => new({string.Join(", ", properties.Select(p => $"dto.{p.Text}"))});");
            }

            sourceBuilder.AppendLine(@$"
}}");

            context.AddSource($"Models/{dtoClassName}.g.cs", SourceText.From(sourceBuilder.ToString(), Encoding.UTF8));
        }
    }
}
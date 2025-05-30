using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Text;
using System.Text.RegularExpressions;

namespace NSubstitute.Documentation.Tests.Generator;

[Generator]
public sealed class DocumentationTestsGenerator : IIncrementalGenerator
{
    private static readonly Regex markdownCodeRegex = new("```(?<tag>\\w+)(?<contents>(?s:.*?))```?");
    private static readonly Regex typeOrTestDeclarationRegex = new(@"(\[Test\]|(public |private |protected )?(class |interface )\w+\s*\{)");

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var markdownFiles = context.AdditionalTextsProvider
            .Where(static file => Path.GetExtension(file.Path) == ".md");

        context.RegisterSourceOutput(markdownFiles, static (context, markdownFile) =>
        {
            var testsClassName = GenerateTestsClassName(markdownFile);
            var testClassContent = GenerateTestClassContent(testsClassName, markdownFile);
            var formattedTestClassContent = FormatCode(testClassContent);
            context.AddSource($"{testsClassName}.g.cs", formattedTestClassContent);
        });
    }

    public static string FormatCode(string code)
    {
        return CSharpSyntaxTree.ParseText(code)
            .GetRoot()
            .NormalizeWhitespace()
            .SyntaxTree
            .GetText()
            .ToString();
    }

    private static string GenerateTestsClassName(AdditionalText markdownFile)
    {
        return $"Tests_{Path.GetFileNameWithoutExtension(markdownFile.Path).Replace("-", "_")}";
    }

    private static string GenerateTestClassContent(string testsClassName, AdditionalText markdownFile)
    {
        var markdownContent = markdownFile.GetText().ToString();
        ParseMarkdownCodeBlocks(markdownContent, out var declarations, out var snippets);

        var testClassContent = new StringBuilder();

        testClassContent.AppendLine(
            $$"""
            using System;
            using NUnit.Framework;
            using System.Linq;
            using System.Collections.Generic;
            using System.ComponentModel;
            using NSubstitute.Extensions;
            using NSubstitute.Compatibility;
            using NSubstitute.ExceptionExtensions;

            namespace NSubstitute.Documentation.Tests.Generated;
            
            public sealed class {{testsClassName}}
            {
            """);

        foreach (var declaration in declarations)
        {
            testClassContent.AppendLine(declaration);
        }

        for (int testCaseNumber = 0; testCaseNumber < snippets.Count; testCaseNumber++)
        {
            testClassContent.AppendLine(
                $$"""
                [Test]
                public void Test{{testCaseNumber}}()
                {
                    {{snippets[testCaseNumber]}}
                }
                """);
        }

        testClassContent.AppendLine("}");

        return testClassContent.ToString();
    }

    private static void ParseMarkdownCodeBlocks(string markdownContent, out List<string> declarations, out List<string> snippets)
    {
        declarations = [];
        snippets = [];

        foreach (Match match in markdownCodeRegex.Matches(markdownContent))
        {
            var codeBlockTitle = match.Groups[1].Value;
            var codeBlockContent = match.Groups[2].Value;

            if (codeBlockTitle == "csharp")
            {
                if (typeOrTestDeclarationRegex.IsMatch(codeBlockContent))
                {
                    declarations.Add(codeBlockContent);
                }
                else
                {
                    snippets.Add(codeBlockContent);
                }
            }
            else if (codeBlockTitle == "requiredcode")
            {
                declarations.Add(codeBlockContent);
            }
        }
    }

}

/*------------------------------------------------------------------------------
Wintellect.Analyzers - .NET Compiler Platform ("Roslyn") Analyzers and CodeFixes
Copyright (c) Wintellect. All rights reserved
Licensed under the Apache License, Version 2.0
See License.txt in the project root for license information
------------------------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Diagnostics;

namespace Wintellect.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ExceptionDocumentationMissingAnalyzer : DiagnosticAnalyzer
    {
        internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticIds.ExceptionDocumentationMissingAnalyzer,
                                                                             Resources.ExceptionDocumentationMissingAnalyzerTitle,
                                                                             Resources.ExceptionDocumentationMissingAnalyzerMessageFormat,
                                                                             Resources.CategoryDocumentation,
                                                                             DiagnosticSeverity.Error,
                                                                             isEnabledByDefault: true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            // I need the SyntaxKind.ThrowStatement so I get the complete line.
            context.RegisterSyntaxNodeAction(AnalyzeThrowDocumentation, SyntaxKind.ThrowStatement);
        }

        private void AnalyzeThrowDocumentation(SyntaxNodeAnalysisContext context)
        {
            // Get the type being thrown by searching for the IdentifierNameSyntax. This works for both "throw new BlahException" and 
            // "throw ex" (in case someone's crazy enough to do that).
            ThrowStatementSyntax throwSyntax = context.Node as ThrowStatementSyntax;
            IdentifierNameSyntax ident = throwSyntax.DescendantNodes().FirstOrDefault(node => node is IdentifierNameSyntax) as IdentifierNameSyntax;

            if (ident != null)
            {
                // Here's the type being thrown by this statement.
                String thrownType = ident.ToString();

                // Look for the parent declaration nodes which will give me the method, operator, constructor, conversionoperator, destructor, indexer, or 
                // property where this throw is happening. 
                var decl = throwSyntax.Ancestors().First(node => ((node is BaseMethodDeclarationSyntax) || (node is BasePropertyDeclarationSyntax)));

                // If this is in a true private method or property, don't touch it.
                Boolean isPrivate = false;
                if (decl is BaseMethodDeclarationSyntax)
                {
                    isPrivate = ((BaseMethodDeclarationSyntax)decl).IsPrivate();
                }
                else
                {
                    isPrivate = ((BasePropertyDeclarationSyntax)decl).IsPrivate();
                }

                if (isPrivate == false)
                {
                    // Assume the exception is not documented.
                    Boolean properlyDocumented = false;

                    // Go find the XML documentation for this declaration.
                    var comments = decl.GetLeadingTrivia().Where(t => t.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia));

                    // There is only one SingleLingDocumentationCommentTrivia, but just in case something changes in the future.
                    if (comments.Count() == 1)
                    {
                        SyntaxTrivia comment = comments.First();

                        // Get the comments in a tree form. The SingleLineDocumentationCommentTrivia is just a single blob.
                        DocumentationCommentTriviaSyntax commentStructure = comment.GetStructure() as DocumentationCommentTriviaSyntax;

                        // If it's not null, look for the exception elements
                        if (commentStructure != null)
                        {
                            var exceptionElements = commentStructure.Content.OfType<XmlElementSyntax>().Where(node => node.EndTag.Name.ToString().Contains("exception"));

                            // Loop through the <exception>... tags.
                            foreach (var documentedException in exceptionElements)
                            {
                                // Get the 'cref="..."' element.
                                var crefList = documentedException.DescendantNodes().OfType<XmlCrefAttributeSyntax>();
                                if (crefList.Count() > 0)
                                {
                                    // It's properly documented if the type of the throw is documented and the inner text of that <exception> tag
                                    // is not empty.
                                    XmlCrefAttributeSyntax cref = crefList.FirstOrDefault();

                                    if (cref.Cref.ToString().Equals(thrownType))
                                    {
                                        var whyThrownText = documentedException.Content.ToString();
                                        if (String.IsNullOrWhiteSpace(whyThrownText) == false)
                                        {
                                            properlyDocumented = true;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (properlyDocumented == false)
                    {
                        var diagnostic = Diagnostic.Create(Rule, throwSyntax.GetLocation(), thrownType);
                        context.ReportDiagnostic(diagnostic);
                    }
                }
            }
        }
    }
}

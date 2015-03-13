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
using System.Xml.Linq;
using System.Xml;

namespace Wintellect.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class ExceptionDocumentationMissingAnalyzer : DiagnosticAnalyzer
    {
        internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticIds.ExceptionDocumentationMissingAnalyzer,
                                                                             new LocalizableResourceString(nameof(Resources.ExceptionDocumentationMissingAnalyzerTitle), Resources.ResourceManager, typeof(Resources)),
                                                                             new LocalizableResourceString(nameof(Resources.ExceptionDocumentationMissingAnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources)),
                                                                             (new LocalizableResourceString(nameof(Resources.CategoryDocumentation), Resources.ResourceManager, typeof(Resources))).ToString(),
                                                                             DiagnosticSeverity.Error,
                                                                             true,
                                                                             new LocalizableResourceString(nameof(Resources.ExceptionDocumentationMissingAnalyzerDescription), Resources.ResourceManager, typeof(Resources)),
                                                                             "http://code.wintellect.com/Wintellect.Analyzers/WebPages/Wintellect010-ExceptionDocumentationMissing.html");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            // I need the SyntaxKind.ThrowStatement so I get the type being thrown.
            context.RegisterSyntaxNodeAction(AnalyzeThrowDocumentation, SyntaxKind.ThrowStatement);
        }

        private void AnalyzeThrowDocumentation(SyntaxNodeAnalysisContext context)
        {
            // Get the method that contains this throw statement. I'm asking for the symbol table because
            // it's got all the XML data and doesn't require me walking the SyntaxNode trees.
            var methodSymbol = context.SemanticModel.GetEnclosingSymbol(context.Node.SpanStart) as IMethodSymbol;

            // I skip private methods because the <exception> tag is for methods called by derived classes or 
            // external classes.
            if (methodSymbol.DeclaredAccessibility == Accessibility.Private)
            {
                return;
            }

            ThrowStatementSyntax throwSyntax = context.Node as ThrowStatementSyntax;

            // If there's no descendant nodes, it's a "throw;" so there's nothing to do here.
            if (!throwSyntax.DescendantNodes().Any())
            {
                return;
            }

            // Get the type being thrown by searching for the IdentifierNameSyntax. This works for both "throw new BlahException" and 
            // "throw ex" (in case someone's crazy enough to do that).
            IdentifierNameSyntax ident = throwSyntax.DescendantNodes().First(node => node is IdentifierNameSyntax) as IdentifierNameSyntax;

            if (ident != null)
            {
                // The type being thrown. I'm getting the fully qualified name because that's the form they
                // are in the ISymbol.GetDocumentationCommentXml.
                ISymbol thrownTypeSymbol = context.SemanticModel.GetSymbolInfo(ident).Symbol;
                String thrownType = thrownTypeSymbol.ToDisplayString();

                String rawDocComment = methodSymbol.GetDocumentationCommentXml(expandIncludes: true);

                // If this method is a property, GetEnclosingSymbol returns the set or get method, which 
                // does not have the XML comments on it only the actual property declaration has those.
                // To go from the setter or getter to the property, use the AssociatedSymbol.
                if (methodSymbol.AssociatedSymbol != null)
                {
                    var propertySymbol = methodSymbol.AssociatedSymbol as IPropertySymbol;
                    rawDocComment = propertySymbol?.GetDocumentationCommentXml();
                }

                // Get all the documented exceptions and the reasons why that exception is thrown.
                Dictionary<String, String> exceptions = this.DocumentedExceptionInformation(rawDocComment);

                // Assume the exception is not documented.
                Boolean properlyDocumented = false;

                foreach (var exception in exceptions)
                {
                    // If the exception type is documented and the reason why is not null, we are 
                    // good to go.
                    if ((thrownType.Equals(exception.Key)) && (!String.IsNullOrEmpty(exception.Value)))
                    {
                        properlyDocumented = true;
                        break;
                    }
                }

                if (properlyDocumented == false)
                {
                    // I report only the exception type name, not the fully qualified name, because that's
                    // how they should be documented.
                    var diagnostic = Diagnostic.Create(Rule, throwSyntax.GetLocation(), thrownTypeSymbol.Name);
                    context.ReportDiagnostic(diagnostic);
                }
            }
        }

        /// <summary>
        /// Takes an ISymbol XML documentation comment string and returns the exception types and  
        /// values from any exception tags found.
        /// </summary>
        /// <param name="methodXml">
        /// The method XML itself.
        /// </param>
        /// <returns>
        /// A dictionary where the key is the type and the value is the description.
        /// </returns>
        private Dictionary<String, String> DocumentedExceptionInformation(String methodXml)
        {
            Dictionary<String, String> exceptList = new Dictionary<String, String>();

            if (false == String.IsNullOrEmpty(methodXml))
            {
                try
                {
                    XElement data = XElement.Parse(methodXml);
                    var exceptTypes = from elem in data.Elements("exception") select elem;

                    // The strings all have the "T:" modifier on them so yank them off.
                    //"T:System.ArgumentOutOfRangeException"
                    foreach (var item in exceptTypes)
                    {
                        String type = item.Attribute("cref").Value.Substring(2);
                        exceptList[type] = item.Value;
                    }
                }
                catch (XmlException)
                {
                    // It's possible to have crap XML in a doc comment.
                }
            }

            return exceptList;
        }

    }
}

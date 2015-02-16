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

namespace Wintellect.Analyzers
{
    // This rule was done by Kevin Pilch-Bisson at TechEd Europe 2014.
    // http://channel9.msdn.com/Events/TechEd/Europe/2014/DEV-B345
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class IfAndElseMustHaveBracesAnalyzer : DiagnosticAnalyzer
    {
        internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticIds.IfAndElseMustHaveBracesAnalyzer,
                                                                             Resources.IfAndElseMustHaveBracesAnalyzerTitle,
                                                                             Resources.IfAndElseMustHaveBracesAnalyzerMessageFormat,
                                                                             Resources.CategoryFormatting, 
                                                                             DiagnosticSeverity.Warning, 
                                                                             true,
                                                                             Resources.IfAndElseMustHaveBracesAnalyzerDescription,
                                                                             helpLink: "http://code.wintellect.com/Wintellect.Analyzers/WebPages/Wintellect003-IfAndElseMustHaveBraces.html");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeIfOrElseStatement, SyntaxKind.IfStatement, SyntaxKind.ElseClause);
        }

        private void AnalyzeIfOrElseStatement(SyntaxNodeAnalysisContext context)
        {
            IfStatementSyntax ifStatement = context.Node as IfStatementSyntax;

            // Is this an if statement followed directly by a call with no braces?
            if ((ifStatement != null) &&
                (ifStatement.Statement != null) &&
                (ifStatement.Statement.IsKind(SyntaxKind.Block) == false))
            {
                Location loc = ifStatement.GetLocation();
                Diagnostic diagnostic = Diagnostic.Create(Rule, loc, "if");
                context.ReportDiagnostic(diagnostic);
            }

            // Is this an else clause followed by a call with no braces?
            ElseClauseSyntax elseSyntax = context.Node as ElseClauseSyntax;
            if ((elseSyntax != null) &&
                (elseSyntax.Statement != null ) &&
                (elseSyntax.Statement.IsKind(SyntaxKind.IfStatement) == false) &&
                (elseSyntax.Statement.IsKind(SyntaxKind.Block) == false))
            {
                Location loc = elseSyntax.GetLocation();
                Diagnostic diagnostic = Diagnostic.Create(Rule, loc, "else");
                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}
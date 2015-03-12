/*------------------------------------------------------------------------------
Wintellect.Analyzers - .NET Compiler Platform ("Roslyn") Analyzers and CodeFixes
Copyright (c) Wintellect. All rights reserved
Licensed under the Apache License, Version 2.0
See License.txt in the project root for license information
------------------------------------------------------------------------------*/
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;

namespace Wintellect.Analyzers
{
    public static class NodeExtensions
    {
        /// <summary>
        /// Adds the string specified using statement to the CompilationUnitSyntax if that using is not already present.
        /// </summary>
        /// <remarks>
        /// The using statement, if inserted, will be followed by a CR/LF.
        /// </remarks>
        /// <param name="unit">
        /// The type being extended.
        /// </param>
        /// <param name="usingString">
        /// The string statement such as "System.Diagnostics" or "Microsoft.CodeAnalysis.CSharp.Syntax".
        /// </param>
        /// <returns>
        /// The CompilationUnitSyntax in <paramref name="unit"/>.
        /// </returns>
        public static CompilationUnitSyntax AddUsingIfNotPresent(this CompilationUnitSyntax unit, String usingString)
        {
            var t = unit.ChildNodes().OfType<UsingDirectiveSyntax>().Where(u => u.Name.ToString().Equals(usingString));
            if (!t.Any())
            {
                UsingDirectiveSyntax usingDirective = SyntaxFactoryHelper.QualifiedUsing(usingString);
                unit = unit.AddUsings(usingDirective).NormalizeWhitespace().WithTrailingTrivia(SyntaxFactory.CarriageReturnLineFeed);
            }
            return unit;
        }

        /// <summary>
        /// Returns true if the PropertyDeclarationSyntax has a getter method.
        /// </summary>
        /// <param name="property">
        /// The property to check.
        /// </param>
        /// <returns>
        /// True if the property has a getter, false otherwise.
        /// </returns>
        public static Boolean HasGetter(this PropertyDeclarationSyntax property)
        {
            return property.AccessorList.Accessors.Where(t => t.IsKind(SyntaxKind.GetAccessorDeclaration)).Any();
        }

        /// <summary>
        /// From a FieldDeclarationSyntax, returns the field name.
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public static String FieldName(this FieldDeclarationSyntax field)
        {
            var vars = field.DescendantNodes().Where(i => i.IsKind(SyntaxKind.VariableDeclarator));
            VariableDeclaratorSyntax varName = (VariableDeclaratorSyntax)vars.First();

            return varName.Identifier.ToString();
        }
    }
}

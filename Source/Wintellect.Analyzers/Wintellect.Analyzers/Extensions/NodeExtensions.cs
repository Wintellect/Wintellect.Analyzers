/*------------------------------------------------------------------------------
Wintellect.Analyzers - .NET Compiler Platform ("Roslyn") Analyzers and CodeFixes
Copyright (c) Wintellect. All rights reserved
Licensed under the Apache License, Version 2.0
See License.txt in the project root for license information
------------------------------------------------------------------------------*/
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using System;
using System.Linq;

namespace Wintellect.Analyzers
{
    public static class NodeExtensions
    {
        /// <summary>
        /// Returns the first parent of a node that is one of the specified types.
        /// </summary>
        /// <param name="node">
        /// The node to check.
        /// </param>
        /// <param name="types">
        /// The array of types to check.
        /// </param>
        /// <returns>
        /// If one of the parents in the <paramref name="types"/> array matches, that type, otherwise null.
        /// </returns>
        /// <remarks>
        /// Full credit to the awesome Giggio at 
        /// https://github.com/code-cracker/code-cracker/blob/master/src/Common/CodeCracker.Common/Extensions/AnalyzerExtensions.cs
        /// </remarks>
        public static SyntaxNode FirstAncestorOfType(this SyntaxNode node, params Type[] types)
        {
            SyntaxNode currentNode = node;
            while (currentNode != null)
            {
                SyntaxNode parent = currentNode.Parent;
                if (parent != null)
                {
                    for (Int32 i = 0; i < types.Length; i++)
                    {
                        if (parent.GetType() == types[i])
                        {
                            return parent;
                        }
                    }
                }

                currentNode = parent;
            }

            return null;
        }

        /// <summary>
        /// Returns the current or first parent of a node that is one of the specified types.
        /// </summary>
        /// <param name="node">
        /// The node to check.
        /// </param>
        /// <param name="types">
        /// The array of types to check.
        /// </param>
        /// <returns>
        /// If the self or one of the parents in the <paramref name="types"/> array matches, that type, otherwise null.
        /// </returns>
        /// <remarks>
        /// Full credit to the awesome Giggio at 
        /// https://github.com/code-cracker/code-cracker/blob/master/src/Common/CodeCracker.Common/Extensions/AnalyzerExtensions.cs
        /// </remarks>
        public static SyntaxNode FirstAncestorOrSelfOfType(this SyntaxNode node, params Type[] types)
        {
            SyntaxNode currentNode = node;
            while (currentNode != null)
            {
                for (Int32 i = 0; i < types.Length; i++)
                {
                    if (currentNode.GetType() == types[i])
                    {
                        return currentNode;
                    }
                }

                currentNode = currentNode.Parent;
            }

            return null;
        }

        /// <summary>
        /// Returns true if this node is part of a looping construct.
        /// </summary>
        /// <param name="node">
        /// The node to check.
        /// </param>
        /// <returns>
        /// True if part of a looping construct, false otherwise.
        /// </returns>
        public static Boolean IsNodeInALoop(this SyntaxNode node)
        {
            return null != node.FirstAncestorOfType(typeof(ForEachStatementSyntax),
                                                    typeof(ForStatementSyntax),
                                                    typeof(WhileStatementSyntax),
                                                    typeof(DoStatementSyntax));
        }

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

                // I'll never understand why WithAdditionalAnnotations(Formatter.Annotation) isn't the default. Picking
                // up the default formatting should be the default and would make developing rules much easier.
                unit = unit.AddUsings(usingDirective).WithAdditionalAnnotations(Formatter.Annotation);
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

        /// <summary>
        /// Returns true if this is generated or non user code.
        /// </summary>
        /// <param name="node">
        /// The SyntaxNode to check.
        /// </param>
        /// <returns>
        /// True if this node or its defining types are non user code.
        /// </returns>
        public static Boolean IsGeneratedOrNonUserCode(this SyntaxNode node)
        {
            // Look at the type, which could be nested.
            TypeDeclarationSyntax currType = (TypeDeclarationSyntax)(node.FirstAncestorOrSelfOfType(typeof(ClassDeclarationSyntax),
                                                                                                    typeof(StructDeclarationSyntax)));
            while (currType != null)
            {
                if (currType.AttributeLists.HasIgnorableAttributes())
                {
                    return true;
                }
                currType = (TypeDeclarationSyntax)(currType.FirstAncestorOfType(typeof(ClassDeclarationSyntax),
                                                                                typeof(StructDeclarationSyntax)));
            }

            // That's as far as we can go with nodes. There's no assembly with them.
            return false;
        }

        /// <summary>
        /// Takes an SyntaxList of Attributes and checks if any are non user code attributes.
        /// </summary>
        /// <param name="attributeList">
        /// The list to check.
        /// </param>
        /// <returns>
        /// True if the list contains a non user code attribute.
        /// </returns>
        public static Boolean HasIgnorableAttributes(this SyntaxList<AttributeListSyntax> attributeList)
        {
            for (Int32 i = 0; i < attributeList.Count; i++)
            {
                AttributeListSyntax currAttrList = attributeList[i];
                for (Int32 k = 0; k < currAttrList.Attributes.Count; k++)
                {
                    AttributeSyntax attr = currAttrList.Attributes[k];
                    if ((attr.Name.ToString().EndsWith("GeneratedCode", StringComparison.Ordinal)) ||
                        ((attr.Name.ToString().EndsWith("DebuggerNonUserCode", StringComparison.Ordinal)))||
                        ((attr.Name.ToString().EndsWith("DebuggerNonUserCodeAttribute", StringComparison.Ordinal))))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}

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
using System.Collections.Immutable;
using System.Linq;

namespace Wintellect.Analyzers
{
    public static class SymbolExtensions
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
        /// Determines if the symbol is part of generated or non-user code.
        /// </summary>
        /// <param name="symbol">
        /// The <see cref="ISymbol"/> derived type to check.
        /// </param>
        /// <param name="checkAssembly">
        /// Set to true to check the assembly for the attributes. False to stop at the type.
        /// </param>
        /// <returns>
        /// Returns true if the item, type, or assembly has the GeneratedCode attribute 
        /// applied.
        /// </returns>
        public static Boolean IsGeneratedOrNonUserCode(this ISymbol symbol, Boolean checkAssembly = true)
        {
            // The goal here is to see if this ISymbol is part of auto generated code.
            // To do that, I'll walk up the hierarchy of item, type, to module/assembly
            // looking to see if the GeneratedCodeAttribute is set on any of them.
            var attributes = symbol.GetAttributes();
            if (!HasIgnorableAttributes(attributes))
            {
                if (symbol.Kind != SymbolKind.NamedType && HasIgnorableAttributes(symbol.ContainingType.GetAttributes()))
                {
                    return true;
                }

                if (checkAssembly)
                {
                    attributes = symbol.ContainingAssembly.GetAttributes();
                    if (HasIgnorableAttributes(attributes))
                    {
                        return true;
                    }
                }

                return false;
            }

            return true;
        }

        private static Boolean HasIgnorableAttributes(ImmutableArray<AttributeData> attributes)
        {
            for (Int32 i = 0; i < attributes.Count(); i++)
            {
                String name = attributes[i].AttributeClass.Name;
                if (name.EndsWith("GeneratedCode") || name.EndsWith("DebuggerNonUserCodeAttribute"))
                {
                    return true;
                }
            }

            return false;
        }
    }
}

/*------------------------------------------------------------------------------
Wintellect.Analyzers - .NET Compiler Platform ("Roslyn") Analyzers and CodeFixes
Copyright (c) Wintellect. All rights reserved
Licensed under the Apache License, Version 2.0
See License.txt in the project root for license information
------------------------------------------------------------------------------*/
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Immutable;
using System.Linq;

namespace Wintellect.Analyzers
{
    public static class Extensions
    {
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

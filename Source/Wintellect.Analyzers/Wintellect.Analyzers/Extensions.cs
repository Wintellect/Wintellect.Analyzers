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
    public static class Extensions
    {
        // Below is a good case for foo<T>(T x) : where T class1,class2,class3 

        /// <summary>
        /// Returns true if this is a private declaration of anything looking like a method.
        /// </summary>
        /// <param name="node">
        /// A <see cref="BaseMethodDeclarationSyntax"/> derived type.
        /// </param>
        /// <returns>
        /// True if private.
        /// </returns>
        public static Boolean IsPrivate(this BaseMethodDeclarationSyntax node)
        {
            // If the modifiers are empty or private is anywhere in the list, it's private.
            if (node.Modifiers.Count() == 0)
            {
                return true;
            }
            if (node.Modifiers.IndexOf(SyntaxKind.PrivateKeyword) != -1)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Returns true if this is anything but a private declaration.
        /// </summary>
        /// <param name="node">
        /// A <see cref="BaseMethodDeclarationSyntax"/> derived type.
        /// </param>
        /// <returns>
        /// True if anything but private
        /// </returns>
        public static Boolean IsExternallyVisible(this BaseMethodDeclarationSyntax node)
        {
            return !node.IsPrivate();
        }

        /// <summary>
        /// Returns true if this is a private declaration of anything looking like a property.
        /// </summary>
        /// <param name="node">
        /// A <see cref="BasePropertyDeclarationSyntax"/> derived type.
        /// </param>
        /// <returns>
        /// True if private.
        /// </returns>
        public static Boolean IsPrivate(this BasePropertyDeclarationSyntax node)
        {
            // If the modifiers are empty or private is anywhere in the list, it's private.
            if (node.Modifiers.Count() == 0)
            {
                return true;
            }
            if (node.Modifiers.IndexOf(SyntaxKind.PrivateKeyword) != -1)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Returns true if this is anything but a private declaration.
        /// </summary>
        /// <param name="node">
        /// A <see cref="BasePropertyDeclarationSyntax"/> derived type.
        /// </param>
        /// <returns>
        /// True if anything but private
        /// </returns>
        public static Boolean IsExternallyVisible(this BasePropertyDeclarationSyntax node)
        {
            return !node.IsPrivate();
        }

        /// <summary>
        /// Returns true if this is a private declaration of anything looking like a field.
        /// </summary>
        /// <param name="node">
        /// A <see cref="BaseFieldDeclarationSyntax"/> derived type.
        /// </param>
        /// <returns>
        /// True if private.
        /// </returns>
        public static Boolean IsPrivate(this BaseFieldDeclarationSyntax node)
        {
            // If the modifiers are empty or private is anywhere in the list, it's private.
            if (node.Modifiers.Count() == 0)
            {
                return true;
            }
            if (node.Modifiers.IndexOf(SyntaxKind.PrivateKeyword) != -1)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Returns true if this is anything but a private declaration.
        /// </summary>
        /// <param name="node">
        /// A <see cref="BaseFieldDeclarationSyntax"/> derived type.
        /// </param>
        /// <returns>
        /// True if anything but private
        /// </returns>
        public static Boolean IsExternallyVisible(this BaseFieldDeclarationSyntax node)
        {
            return !node.IsPrivate();
        }

        /// <summary>
        /// Returns true if this is a private declaration of anything looking like a type.
        /// </summary>
        /// <param name="node">
        /// A <see cref="BaseTypeDeclarationSyntax"/> derived type.
        /// </param>
        /// <returns>
        /// True if private.
        /// </returns>
        public static Boolean IsPrivate(this BaseTypeDeclarationSyntax node)
        {
            // If the modifiers are empty or private is anywhere in the list, it's private.
            if (node.Modifiers.Count() == 0)
            {
                return true;
            }
            if (node.Modifiers.IndexOf(SyntaxKind.PrivateKeyword) != -1)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Returns true if this is anything but a private declaration.
        /// </summary>
        /// <param name="node">
        /// A <see cref="BaseTypeDeclarationSyntax"/> derived type.
        /// </param>
        /// <returns>
        /// True if anything but private
        /// </returns>
        public static Boolean IsExternallyVisible(this BaseTypeDeclarationSyntax node)
        {
            return !node.IsPrivate();
        }

    }
}

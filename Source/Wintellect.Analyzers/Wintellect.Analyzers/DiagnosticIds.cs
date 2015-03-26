/*------------------------------------------------------------------------------
Wintellect.Analyzers - .NET Compiler Platform ("Roslyn") Analyzers and CodeFixes
Copyright (c) Wintellect. All rights reserved
Licensed under the MIT license
------------------------------------------------------------------------------*/
using System;

namespace Wintellect.Analyzers
{
    /// <summary>
    /// The only class that holds the actual analyzer IDs. When creating new rules, 
    /// put the ID constants in here to avoid number conflicts.
    /// </summary>
    static internal class DiagnosticIds
    {
        public const String ReturningTaskRequiresAsyncAnalyzer = "Wintellect001";
        public const String CallAssertMethodsWithMessageParameterAnalyzer = "Wintellect002";
        public const String IfAndElseMustHaveBracesAnalyzer = "Wintellect003";
        public const String AvoidPreDefinedTypesAnalyzer = "Wintellect004";
        public const String AvoidCallingMethodsWithParamArgsInLoopsAnalyzer = "Wintellect005";
        public const String AssembliesHaveCompanyAttributeAnalyzer = "Wintellect006";
        public const String AssembliesHaveCopyrightAttributeAnalyzer = "Wintellect007";
        public const String AssembliesHaveDescriptionAttributeAnalyzer = "Wintellect008";
        public const String AssembliesHaveTitleAttributeAnalyzer = "Wintellect009";
        public const String ExceptionDocumentationMissingAnalyzer = "Wintellect010";
        public const String SuppressionMessageMissingJustificationAnalyzer = "Wintellect011";
        public const String ClassesShouldBeSealedAnalyzer = "Wintellect012";
        public const String UseDebuggerDisplayAttributeAnalyzer = "Wintellect013";
        public const String CatchBlocksShouldRethrowAnalyzer = "Wintellect014";
    }
}

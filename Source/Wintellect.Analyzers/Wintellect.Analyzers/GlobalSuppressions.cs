// This file is used by Code Analysis to maintain SuppressMessage 
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given 
// a specific target and scoped to a namespace, type, member, etc.
//
// To add a suppression to this file, right-click the message in the 
// Code Analysis results, point to "Suppress Message", and click 
// "In Suppression File".
// You do not need to add suppressions to this file manually.
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", 
                                                           "CA2243:AttributeStringLiteralsShouldParseCorrectly", 
                                                           Justification ="The conflict between NuGet's SemVer requirements and the CLR version bites use again")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", 
                                                           "CA1303:Do not pass literals as localized parameters", 
                                                           MessageId = "Microsoft.CodeAnalysis.CSharp.SyntaxFactory.ParseAttributeArgumentList(System.String,System.Int32,Microsoft.CodeAnalysis.ParseOptions,System.Boolean)", 
                                                           Scope = "member", 
                                                           Target = "Wintellect.Analyzers.SyntaxFactoryHelper.#Attribute(System.String,System.String)", 
                                                           Justification = "No sense having resource strings for actual code elements '(' and ')'.")]


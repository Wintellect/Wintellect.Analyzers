# Wintellect.Analyzers #

At [Wintellect](http://www.wintellect.com), we love anything that will help us write the best code possible. Microsoft's new Roslyn compiler is a huge step in that direction so we had to jump in and start writing analyzers and code fixes we've wanted for years. Feel free to fork and add your own favorites. We'll keep adding these as we think of them.

To add these analyzers to your project easily, use the NuGet package. In the Visual Studio Package Manager Console exeute the following:

    Install-Package Wintellect.Analyzers -Prerelease

## Design Analyzers ##
#### [AssembliesHaveCompanyAtrribute](http://code.wintellect.com/Wintellect.Analyzers/WebPages/Wintellect006-AssembliesHaveCompanyAttribute.html) ####
This warning ensures you have the AssemblyCompanyAttribute present and a filled out value in the parameter.

#### [AssembliesHaveCopyrightAtrribute](http://code.wintellect.com/Wintellect.Analyzers/WebPages/Wintellect007-AssembliesHaveCopyrightAttribute.html) ####
This warning ensures you have the AssemblyCopyrightAttribute present and a filled out value in the parameter.

#### [AssembliesHaveDescriptionAtrribute](http://code.wintellect.com/Wintellect.Analyzers/WebPages/Wintellect008-AssembliesHaveDescriptionAttribute.html) ####
This warning ensures you have the AssemblyDescriptionAttribute present and a filled out value in the parameter.

#### [AssembliesHaveTitleAtrribute](http://code.wintellect.com/Wintellect.Analyzers/WebPages/Wintellect009-AssembliesHaveTitleAttribute.html) ####
This warning ensures you have the AssemblyTitleAttribute present and a filled out value in the parameter.

#### [CatchBlocksShouldRethrow](http://code.wintellect.com/Wintellect.Analyzers/WebPages/Wintellect014-CatchBlocksShouldRethrow.html) ####
This informational analyzer will report when you have a catch block that eats an exception. Because exception handling is so hard
to get right, this notification is important to remind you too look at those catch blocks.

## Documentation Analyzers ##
#### [ExceptionDocumentationMissing](http://code.wintellect.com/Wintellect.Analyzers/WebPages/Wintellect010-ExceptionDocumentationMissing.html) ####
If you have a direct throw in your code, you need to document it with an <exception> tag in the XML documentation comments. A direct throw is one where you specifically use the throw statement in your code. This analyzer does not apply to private methods, only accessibility levels where calls outside the defining method can take place.

### [SuppressionMessageMissingJustification](http://code.wintellect.com/Wintellect.Analyzers/WebPages/Wintellect011-SuppressMessageMissingJustification.html) ###
If you are using the SuppressionMessage attribute to suppress Code Analysis items, you need to fill out the
Justification property to explicitly state why you are suppressing the report instead of fixing the code.

## Formatting Analyzers ##

#### [IfAndElseMustHaveBraces](http://code.wintellect.com/Wintellect.Analyzers/WebPages/Wintellect003-IfAndElseMustHaveBraces.html) ####
If and else statements without braces are reasons for being fired. This analyzer and code fix will help you keep your job. :) The idea for this analyzer was shown by Kevin Pilch-Bisson in his awesome [TechEd talk](http://channel9.msdn.com/Events/TechEd/Europe/2014/DEV-B345). We just finished it off.

## Performance Analyzers ##

#### [AvoidCallingMethodsWithParamArgsInLoops](http://code.wintellect.com/Wintellect.Analyzers/WebPages/Wintellect005-AvoidCallingMethodsWithParamArgInLoops.html) ####
This informational level check gives you a hint that you are calling a method using param arrays inside a loop. Because calls to these methods cause memory allocations you should know where these are happening.

## Usage Analzyers ##

#### [AvoidPredefinedTypes](http://code.wintellect.com/Wintellect.Analyzers/WebPages/Wintellect004-AvoidPredefinedTypes.html) ####
The predefined types, such as int, should not be used. You want to be as explicit about types as possible to avoid confusion. 

#### [CallAssertMethodsWithMessageParameter](http://code.wintellect.com/Wintellect.Analyzers/WebPages/Wintellect002-CallAssertMethodsWithMessageParameters.html) ####
Calling the one parameter overload of Debug.Assert is a bad idea because they will not show you the expression you are asserting on. This analyzer will find those calls and the code fix will take the asserting expression and convert it into a string as the second parameter to the two parameter overload of Debug.Assert.

#### [ClassesShouldBeSealed](http://code.wintellect.com/Wintellect.Analyzers/WebPages/Wintellect012-ClassesShouldBeSealed.html) #####
When creating new classes, they should be declared with the the sealed modifier.

#### [ReturningTaskRequiresAsync](http://code.wintellect.com/Wintellect.Analyzers/WebPages/Wintellect001-ReturningTaskRequiresAsync.html) ####
If you are returning a Task or Task<T> from a method, that method name must end in Async.

#### [UseDebuggerDisplay](http://code.wintellect.com/Wintellect.Analyzers/WebPages/Wintellect013-UseDebuggerDisplayAttribute.html) ####
An analyzer and code fix for inserting [DebuggerDisplayAttribute](https://msdn.microsoft.com/en-us/library/system.diagnostics.debuggerdisplayattribute%28v=vs.110%29.aspx) onto public classes. The debugger uses the DebuggerDisplayAttribute to display the class in the expression evaluator (watch/autos/locals windows, data tips) so you can see the important information quickly. The code fix will pull in the first two properties (or fields if one or no properties are present). If the class is derived from IEnumerable, it will default to the count of items. 
 
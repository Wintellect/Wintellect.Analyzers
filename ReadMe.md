# Wintellect.Analyzers #

At [Wintellect](http://www.wintellect.com), we love anything that will help us write the best code possible. Microsoft's new Roslyn compiler is a huge step in that direction so we had to jump in and start writing analyzers and code fixes we've wanted for years. Feel free to fork and add your own favorites. We'll keep adding these as we think of them.

If you contribute code make sure to include unit tests as well as add code to the demo program to show off your analyzer/code fix.

### AvoidCallingMethodsWithParamArgs ###
This informational level gives you a hint that you are calling a method using param arrays. Because calls to these methods cause memory allocations you should know where these are happening.

### AvoidPredefinedTypes ###
The predefined types, such as int, should not be used. You want to be as explicit about types as possible to avoid confusion. 

### CallAssertMethodsWithMessageParameter ###
Calling the one parameter overload of Debug.Assert is a bad idea because they will not show you the expression you are asserting on. This analyzer will find those calls and the code fix will take the asserting expression and convert it into a string as the second parameter to the two parameter overload of Debug.Assert.

### IfAndElseMustHaveBraces ###
If and else statements without braces are reasons for being fired. This analyzer and code fix will help you keep your job. :) The idea for this analyzer was shown by Kevin Pilch-Bisson in his awesome [TechEd talk](http://channel9.msdn.com/Events/TechEd/Europe/2014/DEV-B345). We just finished it off.

### ReturningTaskRequiresAsync ###
If you are returning a Task or Task<T> from a method, that method name must end in Async.
 

CAKE - Convention Based Make Tool
========================

Why?
------
* Drop a couple of .cs files in a folder named *src*, run Cake, and look in the bin folde for your .exe
* Default is debug, because that's what you'll be doing 95% of the time. Run "cake release" to build release mode.
* Forget about .csproj and .sln files. You can keep them, but you don't need them. No more crazy merge conflicts in .csproj files.
* What's in the folder is what gets "made". No "include in project" needed.


What conventions?
---
Based on file extensions, Cake will know what to do in version 0.1:

* everywhere there's a folder named 'src', Cake will build 
* .cs files are compiled as C#
* .resx files are embedded as resources
* app.**x**.config files are transformed at build, and all **x**'s are treated as build configurations
* assemblies in the /lib folder are added as references
* /bin is cleaned before each build
* if there's a .snk file present, it will be used to sign the assembly


Where conventions end and configuration begins
---

GAC references can not be inferred easily. You can place a file named global_references.txt listing the *assembly names* of GAC assemblies you want references.


The future
---

* all other file types are copied to /bin
* support for multiple/sub projects
	* top level project will build as .exe, all sub projects will build as .dlls
* other file types automatically compiled:
	* F# -> assembly
	* VB.NET -> assembly
	* Boo -> assembly
	* Less -> Css
	* Coffee and Iced -> js
	* Markdown -> html
* outputs of different compilers are merged into one .exe
* support for web projects, including MVC and Nancy
	* Automatically attach IIS Express
* configuration free deployment using web deploy packages, nuget, chocolatey and more			folder.EnumerateDirectories("src").Each(FindAndCakeSubProjects);



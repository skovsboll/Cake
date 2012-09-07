
CAKE - Convention Based Make Tool
========================

Why?
------
* Drop a couple of .cs files in a folder named *src*, run Cake, there's your .exe
* Forget about .csproj and .sln files. You can keep them, but you don't need them
* What's in the folder is what gets "made"


What conventions?
---
Based on file extensions, Cake will know what to do in version 0.1:

* .cs files are compiled as C#
* .resx files are embedded as resources
* app.<x>.config files are transformed at build, and all x's are treated as build configurations
* assemblies in the /lib folder are added as references
* /bin is cleaned before each build

The future
---

* all other file types are copied to /bin
* support for multiple/sub projects
* .fs files will compiled with the F# compiler
* .vb files will be compiled with the VB.NET compiler
* .boo files will be compiled with the Boo compiler
* outputs of different compilers are merged into one .exe
* support for web projects, including MVC and Nancy

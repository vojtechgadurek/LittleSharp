# LittleSharp - Statically typed Expression Trees

## Warning 

This library is mostly experimental and more a snaphot, what it can be

## Introduction

Littlesharp brings the possibility to compile functions and actions for some paremeters during runtime and also type safety to them.
This library was mainly developed as sidequest to my Bachealor's thesis, but I decided to make it public. Bacheolor thesis had problem with large amount
of hashing functions. The perfomance issue led mainly in two fact:

	- a % b is a slow operation, but when compiler knows it in addvance, it cam be opmtimized gaining circa 3 times faster execution
	- Virtual calls are expensive for small functions, that are repeated many times and lies in hotpath

Both of those charaterize most of hashing functions.

## Architecture

There are three main types in this library:

### *SmartExpression* and its typed version *SmartExpression\<T>* 

*SmartExpression* represents *Expression* but with added type support

### *Variable* and its typed verison *Variable\<T>*

*Variable* represents *ParameterExpression* but with added type support

### *Scope* and its typed version *Scope\<T>*

*Scope* represents *BlockExpression* but with added type support.

The main goal of this library is to provide better expierence writing expression trees.  These are the main goals of this library:

	- Bad Expression Trees should fail during compilation 
	- The code should be easy to read and write
	- The code should be easy to maintain

Example of usage:

```csharp

var f = CompiledFunction.Create<int, int, int>(out var a, out var b)

f.S.DeclareVariable<int>(out var c, 5);

```


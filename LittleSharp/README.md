# LittleSharp - Statically typed Expression Trees

## Warning 

This library is mostly experimental and more a snaphot, what it can be

## Introduction

Littlesharp brings the possibility to compile functions and actions for some 
paremeters during runtime and also type safety to them.
This library was mainly developed as sidequest to my Bachealor's thesis,
but I decided to make it public. Bacheolor thesis had problem with large amount
of hashing functions. The perfomance issue led mainly in two fact:

- a % b is a slow operation, but when compiler knows it in addvance,
it could be opmtimized gaining circa 3 times faster execution
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

// Declare a function with two parameters a and b
var f = CompiledFunction.Create<int, int, int>(out var a_, out var b_);

// c = 5; 
f.S.DeclareVariable<int>(out var c_, 5)
// return a + b + c
	.Assign(f.Output, a.V + b.V + c.V);
```

### How it works 

Variables are variable and parametares.
Scopes are functions, actions, lambdas and scopes. They hold variables, 
parameters and expressions to be executed. Scopes may be contained 
in other scopes and they may also contain them.
Variables may be used only from scope they are declared in or from any scope declared in same scope. 
SmartExpression\<T> are just expression with type safety and added operators.

Variables may hold value. This value may be accessed in field *V*, where *V* is abbreviation for *Value*. 

It is important to understand, how expression works. Every scope hold list of expressions, 
thus order of their execution on the order when they inserted to this list. Most of methods on Scope does
not add any expression to the expression list. Only Assign does so and AddExpression does so. This may seem counterintuitive,
but is important to remmeber, if function does not change state, there is no need to execute it (Most of the time :D).
```csharp

// Declares a new scope
var scope = new Scope();

// Creates new parameter
scope.DeclareVariable<int>(a, 0)
	// Return expression giving a + 5
	// Marco does not add to expression list
	// Thus a + 5 is not executed
	.Marco(var out b, a + 5)
	// Assigns a + 5 to a 
	// thus now a = b = a + 5 = 0 + 5 = 5
	.Assign(a, b)

var value = scope.Construct()  // Returns SmartExpression<NoneType> that represents scope without return value
// SmartExpression<NoneType> is just wrapper for Expression so it may hold some 
value and for block statements it is the value of last expression
// We may use this knowlede to force type, this may fail if the last expression is not int

value.ForceType<int>() // Returns Int>
```

Sometimes we want to have a scope return some value. Thus one should use *Scope<\T>*. 
Every scope with return value holds a *Output* variable, which may be used as a return value. 
Also sometimes, we may want to go the the end of the scope. May use *scope.GotoEnd( SCOPE_WHICH_END_TO_USE)*. Any scope may go end
of any containg scope. This is equvalent to *return*, *continue*, *break* in C#.
```csharp
var scope = new Scope<int>();
scope.DeclareVariable<int>(a, 0)
	.Assign(a, 5)
	.Assign(scope.Output, a)
	.GotoEnd(scope);
	//Any code afeter this will not be executed
	.Assign(scope.Output, 10)
	.Construct(); // Returns SmartExpression<int> holding expresison tree retunning 5.
```

Is is important to remember, that *construct* method does not execute the code. It just creates the expression tree,
that needs to be compiled to be executed, we may use this to create macros.

```csharp

var scope = new Scope<int>();
scope.DeclareVariable<int>(i_, 0)
	.Marco(out var macro, new Scope().Assign(i_, i_ + 5))
	.AddExpression(macro)
	.AddExpression(macro)
	.Assign(scope.Output, i_)
	.Construct(); // Returns SmartExpression<int> holding expression tree returning 10
```


# LittleSharp - Statically typed Expression Trees

## Warning 

This library is mostly experimental and more a peek into a concept than a fully functional library.

This text does not cover all api, please look to the code. It should be taken more as a primer than a full documentation.

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
- It should not be replacement for native C# code, when performance is a concern

When to use this library:

- There exists need to compile some function during runtime.
- Solution to a problem, leads to using virtual calls, but they would lead to perfomance issues.

Do not forget:
 
- One always has to call compiled function from somewhere and this will be a delegate call.
- There is some cost to compiling code during runtime, it may not be woth it, if called just several times.

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
*SmartExpression\<T>* are just expression with type safety and added operators.

Variables may hold value. This value may be accessed in field *V*, where *V* is abbreviation for *Value*. 

It is important to understand, how expression works. Every scope holds list of expressions, 
thus order of their execution on the order when they are inserted to this list. Most of methods on Scope does
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
	.Marco(var out b, a.V + 5)
	// Assigns a + 5 to a 
	// thus now a = b = a + 5 = 0 + 5 = 5
	.Assign(a, b)

var value = scope.Construct()  // Returns SmartExpression<NoneType> that represents scope without return value
// SmartExpression<NoneType> is just wrapper for Expression so it may hold some 
// value and for block statements, it is the value of last expression
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
	.Assign(scope.Output, a.V)
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
	.Marco(out var macro, new Scope().Assign(i_, i_.V + 5))
	.AddExpression(macro)
	.AddExpression(macro)
	.Assign(scope.Output, i_)
	.Construct(); // Returns SmartExpression<int> holding expression tree returning 10
```

#### Effects
This library works fine for simple types, but sometimes we would like to use more complex types. 
##### May I have a dream
Let's have some variable *a* being *SmartExpresion\<T\>* and *T* is some type with method *A()*, what we would like
to be able to call *a.A()* and get *Expression.Call(a.Expression, typeof(T).GetMethod("A"))*.
This is not possible in C# without lot of work. The workouround is to add such method to *Scope* using extension methods and create
interface *IMedthodName*. Than we just need to have way to tell the compiler, that *a* is of type *IMethodName* and we are done.

Will do this by adding another extension method to Scope with name *ToTypeName*, returning such name.
As this project was mainly developed as sidequest to my Bachealor's thesis. I hardcoded some *effect*. Such as *ISet* which is child of
*IAdd*, *IContains*, *IRemove*.

This not ideal, but it works for the purpose of my main project. I will try to make it more general in the future.

##### ArrayAccess
This is variable, that is gotten by 

```csharp
var scope = new Scope();
scope.DeclareVariable<ISet<int>>(out var set_, new HashSet<int>())
	.ToSet(set_, set_SET)
	// .Add(set, 5)  does not compile
	.Add(set_SET, 5)
	.Add(set_SET, 10)
	.Add(set_SET, 15)
	.Remove(set_SET, 10)
	.Contains(set_SET, 5, out var check) // check is SmartExpression<bool> holding expression tree returning true
	.Contains(set_SET, 10, out var check2) // check2 is SmartExpression<bool> holding expression tree returning false
	.Construct(); // Returns SmartExpression<bool> holding expression tree returning true
```


## Classes

### Scopes

#### Scope and Scope\<T> 
They held variables, parameters and expressions together.
##### While(condition, body)
Works as normal while loop. If condtion is true, body is executed and again condition is checked. If condition is false, the loop ends.
```csharp
var scope = new Scope();
scope.DeclareVariable<int>(i, 0)
	.While(i.V < 10, new Scope().Assign(i, i.V + 1))
	.Construct(); // Returns SmartExpression<NoneType> holding expression tree returning 10
```
##### IfThen(condition, body)
Works as normal if statement. If condition is true, body is executed.
```csharp
var scope = new Scope();
scope.DeclareVariable<int>(i, 0)
	.IfThen(i.V < 10, new Scope().Assign(i, i.V + 1))
	.Construct(); // Returns SmartExpression<NoneType> holding expression tree returning 1
```
##### IfThenElse(condition, ifTrue, ifFalse)
Works as normal if else statement. If condition is true, ifTrue is executed, if condition is false, ifFalse is executed.
```csharp
var scope = new Scope();
scope.DeclareVariable<int>(i, 0)
	.IfThenElse(i.V < 10, new Scope().Assign(i, i.V + 1), new Scope().Assign(i, i.V + 2))
	.Construct(); // Returns SmartExpression<NoneType> holding expression tree returning 1
```
##### DeclareVariable\<T>(out Variable\<T> var, T value)
Declares new variable of type T and assigns value to it.

###### Function(function, params, out output) and Action(action, params)
Runs a action or function with given parameters. If function is used, it returns value of function to output.
It adds a invocation expression to the expression list.

#### Lambda and Lambda\<T>
Inherits scope adds possibility to declare parameters. 
##### DeclareParameter\<T>
Declares parameter of type T

##### Construct(), Construct(ValueParameterPairs valPair)
Returns Expression, that is *Expression<Action<T1 ... TN>>* for *Lambda* or  *Expression<Fucn<T1 ... TN, TOutput>>* for *Lambda\<TOutput>*,
where *T1* ... *TN* are types of unasigned parameters. 
By providing *ValueParameterPairs* we may assign values to parameters, than these types disappear from the signature.
Types in signature are accoring to the order of declaration of parameters.


```csharp
var lambda = new Lambda<int>();
lambda
	.DeclareParameter<int>(a)
	.DeclareParameter<int>(b)
	.Assign(lambda.Output, a.V + b.V)

// Is equvalient to f1 = (a, b) => a + b
Expression<Func<int, int, int>> f1 = (Expression<Func<int, int, int>>) lambda.Construct();

// Is equvalient to f2 = (a) => a + 0
Expression<Func<int, int>> f2 = (Expression<Func<int, int>>) lambda.Construct(new ValuePair.Add(a, 0));
```

#### *CompiledFunction<T1 ... TN, TOutput>* and *CompiledAction\<T1 ... TN, TOutput>*
They wraps *Lambda*. Their main advantage is, 
that their singature does not change and they disallow to declaring new parametrer. Also as signature of function is known in advance,
we may return *Expression<Action<T1 ... TN>>* for *CompiledAction* and *Expression<Func<T1 ... TN, TOutput>>* for *CompiledFunction*.

##### *S*
*S* field allows to access to a the lambda, that is wrapped by *CompiledFunction* or *CompiledAction*. It is returned as
*Scope* to protect user from declaring new parameters.

```csharp
// Declare a function with two parameters a and b
var f = CompiledFunction.Create<int, int, int>(out var a_, out var b_);
f.S.Assign(f.Output, a.V + b.V);
```




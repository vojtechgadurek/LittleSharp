
# LittleSharp - Statically Typed Expression Trees

## Warning 

This library is mostly experimental and serves more as a glimpse into a concept than a fully functional library.

This text does not cover all APIs; please refer to the code. It should be considered more as a primer than comprehensive documentation.

This text was partially rewritten by ChatGPT to fix grammatical and stylistic issues. If was then checked for correctness by the original author.

## Introduction

LittleSharp brings the possibility to compile functions and actions for some parameters during runtime and also provides type safety to them. This library was mainly developed as a sidequest to my Bachelor's thesis, but I decided to make it public. My Bachelor thesis dealt with a problem involving a large number of hashing functions. The performance issue mainly arose from two factors:

- The operation a % b is slow, but when the compiler knows it in advance, it could be optimized, resulting in around 3 times faster execution.
- Virtual calls are expensive for small functions that are repeated many times and are on the hot path.

Both of these characterize most hashing functions.

## Architecture

There are three main types in this library:

### *SmartExpression* and its typed version *SmartExpression\<T>* 

*SmartExpression* represents an *Expression* but with added type support.

### *Variable* and its typed version *Variable\<T>*

*Variable* represents a *ParameterExpression* but with added type support.

### *Scope* and its typed version *Scope\<T>*

*Scope* represents a *BlockExpression* but with added type support.

The main goal of this library is to provide a better experience in writing expression trees. These are the main goals of this library:

- Faulty Expression Trees should fail during compilation.
- The code should be easy to read and write.
- The code should be easy to maintain.
- It should not replace native C# code when performance is a concern.

When to use this library:

- There exists a need to compile some function during runtime.
- The solution to a problem leads to using virtual calls, but they would lead to performance issues.

Do not forget:

- One always has to call the compiled function from somewhere, and this will be a delegate call.
- There is some cost to compiling code during runtime; it may not be worth it if called just several times.

Example of usage:

```csharp

// Declare a function with two parameters a and b
var f = CompiledFunction.Create<int, int, int>(out var a_, out var b_);

// c = 5; 
f.S.DeclareVariable<int>(out var c_, 5)
// return a + b + c
	.Assign(f.Output, a.V + b.V + c.V);
```

### How it Works 

Variables are variables and parameters.
Scopes are functions, actions, lambdas, and scopes. They hold variables, parameters, and expressions to be executed. Scopes may be contained in other scopes, and they may also contain them. Variables may be used only from the scope they are declared in or from any scope declared in the same scope. *SmartExpression\<T>* are just expressions with type safety and added operators.

Variables may hold a value. This value may be accessed in the field *V*, where *V* is an abbreviation for *Value*. 

It is important to understand how expressions work. Every scope holds a list of expressions, thus the order of their execution is the order in which they are inserted into this list. Most of the methods on Scope do not add any expression to the expression list. Only Assign does so, and AddExpression does so. This may seem counterintuitive, but it is important to remember that if a function does not change state, there is no need to execute it (most of the time :D).
```csharp

// Declare a new scope
var scope = new Scope();

// Create a new parameter
scope.DeclareVariable<int>(a, 0)
	// Return expression giving a + 5
	// Marco does not add to the expression list
	// Thus a + 5 is not executed
	.Marco(var out b, a.V + 5)
	// Assigns a + 5 to a 
	// thus now a = b = a + 5 = 0 + 5 = 5
	.Assign(a, b);

var value = scope.Construct();  // Returns SmartExpression<NoneType> that represents a scope without a return value
// SmartExpression<NoneType> is just a wrapper for Expression so it may hold some 
// value and for block statements, it is the value of the last expression
// We may use this knowledge to force type; this may fail if the last expression is not int

value.ForceType<int>(); // Returns Int>
```

Sometimes we want to have a scope return some value. Thus one should use *Scope<\T>*. Every scope with a return value holds an *Output* variable, which may be used as a return value. Also sometimes, we may want to go to the end of the scope. We may use *scope.GotoEnd(SCOPE_WHICH_END_TO_USE)*. Any scope may go to the end of any containing scope. This is equivalent to *return*, *continue*, *break* in C#.
```csharp
var scope = new Scope<int>();
scope.DeclareVariable<int>(a, 0)
	.Assign(a, 5)
	.Assign(scope.Output, a.V)
	.GotoEnd(scope);
	// Any code after this will not be executed
	.Assign(scope.Output, 10)
	.Construct(); // Returns SmartExpression<int> holding expression tree returning 5.
```

It is important to remember that the *Construct* method does not execute the code. It just creates the expression tree that needs to be compiled to be executed. We may use this to create macros.

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
##### May I Have a Dream
Let's have some variable *a* being *SmartExpression\<T\>* and *T* is some type with method *A()*, what we would like is to be able to call *a.A()* and get *Expression.Call(a.Expression, typeof(T).GetMethod("A"))*. This is not possible in C# without a lot of work. The workaround is to add such a method to *Scope* using extension methods and create the interface *IMethodName*. Then we just need to have a way to tell the compiler that *a* is of type *IMethodName*, and we are done.

We will do this by adding another extension method to Scope with the name *ToTypeName*, returning such a name. As this project was mainly developed as a sidequest to my Bachelor's thesis, I hardcoded some *effect*. Such as *ISet* which is a child of *IAdd*, *IContains*, *IRemove*.

This is not ideal, but it works for the purpose of my main project. I will try to make it more general in the future.

##### ArrayAccess
This is a variable

 that is obtained by 

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
They hold variables, parameters, and expressions together.
##### While(condition, body)
Works as a normal while loop. If the condition is true, the body is executed, and again the condition is checked. If the condition is false, the loop ends.
```csharp
var scope = new Scope();
scope.DeclareVariable<int>(i, 0)
	.While(i.V < 10, new Scope().Assign(i, i.V + 1))
	.Construct(); // Returns SmartExpression<NoneType> holding expression tree returning 10
```
##### IfThen(condition, body)
Works as a normal if statement. If the condition is true, the body is executed.
```csharp
var scope = new Scope();
scope.DeclareVariable<int>(i, 0)
	.IfThen(i.V < 10, new Scope().Assign(i, i.V + 1))
	.Construct(); // Returns SmartExpression<NoneType> holding expression tree returning 1
```
##### IfThenElse(condition, ifTrue, ifFalse)
Works as a normal if-else statement. If the condition is true, ifTrue is executed, if the condition is false, ifFalse is executed.
```csharp
var scope = new Scope();
scope.DeclareVariable<int>(i, 0)
	.IfThenElse(i.V < 10, new Scope().Assign(i, i.V + 1), new Scope().Assign(i, i.V + 2))
	.Construct(); // Returns SmartExpression<NoneType> holding expression tree returning 1
```
##### DeclareVariable\<T>(out Variable\<T> var, T value)
Declares a new variable of type T and assigns a value to it.

###### Function(function, params, out output) and Action(action, params)
Runs an action or function with given parameters. If a function is used, it returns the value of the function to output. It adds an invocation expression to the expression list.

#### Lambda and Lambda\<T>
Inherits scope and adds the possibility to declare parameters. 
##### DeclareParameter\<T>
Declares a parameter of type T.

##### Construct(), Construct(ValueParameterPairs valPair)
Returns an Expression that is *Expression<Action<T1 ... TN>>* for *Lambda* or *Expression<Func<T1 ... TN, TOutput>>* for *Lambda\<TOutput>*, where *T1* ... *TN* are types of unasigned parameters. By providing *ValueParameterPairs* we may assign values to parameters; then these types disappear from the signature. Types in the signature are according to the order of declaration of parameters.


```csharp
var lambda = new Lambda<int>();
lambda
	.DeclareParameter<int>(a)
	.DeclareParameter<int>(b)
	.Assign(lambda.Output, a.V + b.V);

// Is equivalent to f1 = (a, b) => a + b
Expression<Func<int, int, int>> f1 = (Expression<Func<int, int, int>>) lambda.Construct();

// Is equivalent to f2 = (a) => a + 0
Expression<Func<int, int>> f2 = (Expression<Func<int, int>>) lambda.Construct(new ValuePair.Add(a, 0));
```

#### *CompiledFunction<T1 ... TN, TOutput>* and *CompiledAction\<T1 ... TN, TOutput>*
They wrap *Lambda*. Their main advantage is that their signature does not change, and they disallow declaring new parameters. Also, as the signature of the function is known in advance, we may return *Expression<Action<T1 ... TN>>* for *CompiledAction* and *Expression<Func<T1 ... TN, TOutput>>* for *CompiledFunction*.

##### *S*
*S* field allows accessing the lambda that is wrapped by *CompiledFunction* or *CompiledAction*. It is returned as a *Scope* to protect the user from declaring new parameters.

```csharp
// Declare a function with two parameters a and b
var f = CompiledFunction.Create<int, int, int>(out var a_, out var b_);
f.S.Assign(f.Output, a.V + b.V);
```

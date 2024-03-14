using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using LittleSharp.Literals;
using LittleSharp.Callables;

namespace LittleSharp
{
	public class Scope
	{
		protected Dictionary<string, ParameterExpression> _variables = new Dictionary<string, ParameterExpression>();
		protected List<Expression> _expressions = new List<Expression>();
		public readonly LabelTarget EndLabel = Expression.Label("End");
		string? _name;
		public Scope(string? name = null)
		{
			this._name = name;
		}
		public SmartExpression<NoneType> Construct()
		{
			return new SmartExpression<NoneType>(
				Expression.Block(
					_variables.Values,
					_expressions.Append(Expression.Label(EndLabel))
					)
				);


		}
		public Scope Macro<T>(out T expression, T value)
		{
			expression = value;
			return this;
		}

		public Variable<TType> DeclareVariable<TType>(string name)
		{
			var variable = new Variable<TType>(name);
			_variables.Add(name, (ParameterExpression)variable.Expression);
			return variable;
		}

		public Scope DeclareVariable<TType>(string name, out Variable<TType> variable)
		{
			variable = DeclareVariable<TType>(name);
			return this;
		}

		public Scope DeclareVariable<TType>(out Variable<TType> variable)
		{
			variable = DeclareVariable<TType>(nameof(variable));
			return this;
		}

		public Scope DeclareVariable<TType>(out Variable<TType> variable, SmartExpression<TType> value)
		{
			variable = DeclareVariable<TType>(nameof(variable));
			_variables.Add(variable.Name, (ParameterExpression)variable.Expression);
			_expressions.Add(Expression.Assign(variable.Expression, value.Expression));
			return this;
		}

		public Scope AddVariable<TType>(Variable<TType> variable)
		{
			_variables.Add(variable.Name, (ParameterExpression)variable.Expression);
			return this;
		}

		public Scope Assign<T>(ILiteral<T> variable, SmartExpression<T> value)
		{
			_expressions.Add(Expression.Assign(variable.GetExpression(), value.Expression));
			return this;
		}

		public Scope ArrayAssing<T>(Variable<T> variable, SmartExpression<int> index, SmartExpression<T> value)
		{
			_expressions.Add(Expression.Assign(Expression.ArrayAccess(variable.Expression, index.Expression), value.Expression));
			return this;
		}

		public Scope Invoke<TValue>(SmartExpression<NoneType> function, params SmartExpression[] arguments)
		{
			Invoke<TValue>(function, out var ansver, arguments);
			_expressions.Add(ansver.Expression);
			return this;
		}

		public Scope Invoke<TValueIn, TValueOut>(Expression<Func<TValueIn, TValueOut>> function, SmartExpression<TValueIn> argument, out SmartExpression<TValueOut> returnValue)
		{
			returnValue = new SmartExpression<TValueOut>(Expression.Invoke(function, argument.Expression));
			return this;
		}

		public Scope Invoke<TValue>(SmartExpression<NoneType> function, out SmartExpression<TValue> returnValue, params SmartExpression[] arguments)
		{
			returnValue = new SmartExpression<TValue>(Expression.Invoke(function.Expression, arguments.Select(a => a.Expression)));
			return this;
		}


		public Scope Invoke<TValueIn, TValueOut>(CompiledFunction<TValueIn, TValueOut> function, SmartExpression<TValueIn> argument, out SmartExpression<TValueOut> returnValue)
		{
			Invoke(function.Construct(), argument, out returnValue);
			return this;
		}

		public Scope Invoke<TValue>(Expression<Action<TValue>> function, SmartExpression<TValue> argument)
		{
			_expressions.Add(Expression.Invoke(function, argument.Expression));
			return this;
		}

		public Scope This(out Scope thisScope)
		{
			thisScope = this;
			return this;
		}
		public Scope IfThen(SmartExpression<bool> condition, Scope actionToDo)
		{
			_expressions.Add(
				Expression.IfThen(
					condition.Expression,
					actionToDo.Construct().Expression
				)
			);

			return this;
		}

		public Scope IfThenElse(SmartExpression<bool> condition, Scope actionToDoConditionIsTrue, Scope actionToDoIfConditionIsFalse)
		{
			_expressions.Add(
				Expression.IfThenElse(
					condition.Expression,
					actionToDoConditionIsTrue.Construct().Expression,
					actionToDoIfConditionIsFalse.Construct().Expression
					)
				);
			return this;
		}

		public Scope AddExpression<T>(SmartExpression<T> expression)
		{
			_expressions.Add(expression.Expression);
			return this;
		}

		public Scope SubScope(Scope scope)
		{
			_expressions.Add(scope.Construct().Expression);
			return this;
		}

		public Scope While(SmartExpression<bool> condition, Scope actionToDo)
		{

			var breakLabel = Expression.Label("LoopBreak");
			Expression expression = Expression.Loop(
				Expression.IfThenElse(
					condition.Expression,
					actionToDo.Construct().Expression,
					Expression.Break(breakLabel)
					),
				breakLabel
				);
			_expressions.Add(expression);
			return this;
		}

		public Scope GoToEnd(Scope scope)
		{
			_expressions.Add(Expression.Goto(scope.EndLabel));
			return this;
		}
	}

	public class Scope<TReturnValue> : Scope
	{
		public readonly Variable<TReturnValue> Output;
		public Scope(string? name = null) : base(name)
		{
			Output = DeclareVariable<TReturnValue>("Output");
		}
		public new SmartExpression<TReturnValue> Construct()
		{
			return new SmartExpression<TReturnValue>(
				Expression.Block(
					_variables.Values,
					_expressions.Append(
						Expression.Label(EndLabel, Output.Expression)
						)
					)
				);
		}
	}
}

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
		protected List<ParameterExpression> _variables = new List<ParameterExpression>();
		protected List<Expression> _expressions = new List<Expression>();
		protected List<Expression> _finalizerExpression = new List<Expression>();
		public readonly LabelTarget EndLabel = Expression.Label("End");
		string? _name;
		// This is little hack for debugging, should not be used for any other purpose
		// Rewrite of the architecture is needed to make it more safe
		public static object? _debugOutput = null;
		public Scope(string? name = null, TextWriter? _output = null)
		{
			this._name = name;
		}

		public SmartExpression<NoneType> Construct()
		{
			return new SmartExpression<NoneType>(
				Expression.Block(
					_variables,
					_expressions.Append(Expression.Label(EndLabel)).Concat(_finalizerExpression)
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
			_variables.Add((ParameterExpression)variable.Expression);
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
			_expressions.Add(Expression.Assign(variable.Expression, value.Expression));
			return this;
		}
		public Scope AddVariable<TType>(Variable<TType> variable)
		{
			_variables.Add((ParameterExpression)variable.Expression);
			return this;
		}

		public Scope AddFinalizer(Scope expression)
		{
			_finalizerExpression.Add(expression.Construct().Expression);
			return this;
		}
		public Scope Assign<T>(ILiteral<T> variable, SmartExpression<T> value)
		{
			_expressions.Add(Expression.Assign(variable.GetExpression(), value.Expression));
			return this;
		}

		public Scope Print(SmartExpression<string> text)
		{
			if (_debugOutput == null)
			{
				_expressions.Add(Expression.Call(typeof(Console).GetMethod("WriteLine", new Type[] { typeof(string) }), text.Expression));
			}
			else
			{
				_expressions.Add(Expression.Call(Expression.Constant(_debugOutput), _debugOutput.GetType().GetMethod("WriteLine", new Type[] { typeof(string) }), text.Expression));
			}
			return this;
		}
		public Scope ArrayAssing<T>(Variable<T> variable, SmartExpression<int> index, SmartExpression<T> value)
		{
			_expressions.Add(Expression.Assign(Expression.ArrayAccess(variable.Expression, index.Expression), value.Expression));
			return this;
		}

		public Scope Add<TExpression, TValue>(IAdd<TExpression, TValue> add, SmartExpression<TValue> value)
		{
			_expressions.Add(add.Add(value).Expression);
			return this;
		}

		#region Actions
		public Scope UnTypedAction(Expression action, params SmartExpression[] arguments)
		{
			_expressions.Add(Expression.Invoke(action, arguments.Select(x => x.Expression)));
			return this;
		}

		public Scope Action(Expression<Action> action)
		{
			_expressions.Add(Expression.Invoke(action));
			return this;
		}

		public Scope Action<T1>(Expression<Action<T1>> action, SmartExpression<T1> smartExpression)
		{
			_expressions.Add(Expression.Invoke(action, smartExpression.Expression));
			return this;
		}

		public Scope Action<T1, T2>(Expression<Action<T1, T2>> action, SmartExpression<T1> smartExpression1, SmartExpression<T2> smartExpression2)
		{
			_expressions.Add(Expression.Invoke(action, smartExpression1.Expression, smartExpression2.Expression));
			return this;
		}

		public Scope Action<T1, T2, T3>(Expression<Action<T1, T2, T3>> action, SmartExpression<T1> smartExpression1, SmartExpression<T2> smartExpression2, SmartExpression<T3> smartExpression3)
		{
			_expressions.Add(Expression.Invoke(action, smartExpression1.Expression, smartExpression2.Expression, smartExpression3.Expression));
			return this;
		}

		public Scope Action<T1, T2, T3, T4>(Expression<Action<T1, T2, T3, T4>> action, SmartExpression<T1> smartExpression1, SmartExpression<T2> smartExpression2, SmartExpression<T3> smartExpression3, SmartExpression<T4> smartExpression4)
		{
			_expressions.Add(Expression.Invoke(action, smartExpression1.Expression, smartExpression2.Expression, smartExpression3.Expression, smartExpression4.Expression));
			return this;
		}

		#endregion

		#region Functions

		public Scope UnTypedFunction(Expression action, params SmartExpression[] arguments)
		{
			_expressions.Add(Expression.Invoke(action, arguments.Select(x => x.Expression)));
			return this;
		}

		public Scope Function<TOut>(Expression<Func<TOut>> action, out SmartExpression<TOut> output)
		{
			output = new SmartExpression<TOut>(Expression.Invoke(action));
			return this;
		}

		public Scope Function<T1, TOut>(Expression<Func<T1, TOut>> action, SmartExpression<T1> value1, out SmartExpression<TOut> output)
		{
			output = new SmartExpression<TOut>(Expression.Invoke(action, value1.Expression));
			return this;
		}

		public Scope Function<T1, T2, TOut>(Expression<Func<T1, T2, TOut>> action, SmartExpression<T1> value1, SmartExpression<T2> value2, out SmartExpression<TOut> output)
		{
			output = new SmartExpression<TOut>(Expression.Invoke(action, value1.Expression, value2.Expression));
			return this;
		}

		public Scope Function<T1, T2, T3, TOut>(Expression<Func<T1, T2, T3, TOut>> action, SmartExpression<T1> value1, SmartExpression<T2> value2, SmartExpression<T3> value3, out SmartExpression<TOut> output)
		{
			output = new SmartExpression<TOut>(Expression.Invoke(action, value1.Expression, value2.Expression, value3.Expression));
			return this;
		}

		public Scope Function<T1, T2, T3, T4, TOut>(Expression<Func<T1, T2, T3, T4, TOut>> action, SmartExpression<T1> value1, SmartExpression<T2> value2, SmartExpression<T3> value3, SmartExpression<T4> value4, out SmartExpression<TOut> output)
		{
			output = new SmartExpression<TOut>(Expression.Invoke(action, value1.Expression, value2.Expression, value3.Expression, value4.Expression));
			return this;
		}

		public SmartExpression<TOut> Function<TOut>(Expression<Func<TOut>> action)
		{
			return new SmartExpression<TOut>(Expression.Invoke(action));
		}

		public SmartExpression<TOut> Function<T1, TOut>(Expression<Func<T1, TOut>> action, SmartExpression<T1> value1)
		{
			return new SmartExpression<TOut>(Expression.Invoke(action, value1.Expression));
		}


		public SmartExpression<TOut> Function<T1, T2, TOut>(Expression<Func<T1, T2, TOut>> action, SmartExpression<T1> value1, SmartExpression<T2> value2)
		{
			return new SmartExpression<TOut>(Expression.Invoke(action, value1.Expression, value2.Expression));
		}


		public SmartExpression<TOut> Function<T1, T2, T3, TOut>(Expression<Func<T1, T2, T3, TOut>> action, SmartExpression<T1> value1, SmartExpression<T2> value2, SmartExpression<T3> value3)
		{
			return new SmartExpression<TOut>(Expression.Invoke(action, value1.Expression, value2.Expression, value3.Expression));
		}

		#endregion
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

		public Scope AddExpressions<T>(IEnumerable<SmartExpression<T>> expressions)
		{
			foreach (var ex in expressions)
			{
				AddExpression(ex);
			}
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


		public Scope BuildAction<T>(Action<T> action, T values)
		{
			action(values);
			return this;
		}

		public Scope BuildAction<T>(Action<T> action, IEnumerable<T> values)
		{
			foreach (var item in values)
			{
				BuildAction(action, item);
			}
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
					_variables,
					_expressions.Append(
						Expression.Label(EndLabel, Output.Expression)
						)
					)
				);
		}
	}
}

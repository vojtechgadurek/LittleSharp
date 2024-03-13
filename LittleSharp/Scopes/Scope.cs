using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using LittleSharp.Variables;

namespace LittleSharp
{
	public class Scope
	{
		Dictionary<string, ParameterExpression> _variables = new Dictionary<string, ParameterExpression>();
		protected List<Expression> _expressions = new List<Expression>();
		string? name;
		public Scope(string? name = null)
		{
			this.name = name;
		}
		public SmartExpression<NoneType> ToSmartExpression()
		{
			return new SmartExpression<NoneType>(Expression.Block(_variables.Values, _expressions));
		}
		public Variable<TType> DeclareVariable<TType>(string name)
		{
			var variable = new Variable<TType>(name);
			_variables.Add(name, (ParameterExpression)variable.Expression);
			return variable;
		}

		public Scope Macro<T>(out T expression, T value)
		{
			expression = value;
			return this;
		}

		public Scope DeclareVariable<TType>(string name, out Variable<TType> variable)
		{
			variable = DeclareVariable<TType>(name);
			return this;
		}

		public Scope AddVariable<TType>(Variable<TType> variable)
		{
			_variables.Add(variable.Name, (ParameterExpression)variable.Expression);
			return this;
		}

		public Scope Assign<T>(IAssingableExpression<T> variable, SmartExpression<T> value)
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

		public Scope IfThen(SmartExpression<bool> condition, Scope actionToDo)
		{
			_expressions.Add(
				Expression.IfThen(
					condition.Expression,
					actionToDo.ToSmartExpression().Expression
				)
			);

			return this;
		}

		public Scope IfThenElse(SmartExpression<bool> condition, Scope actionToDoConditionIsTrue, Scope actionToDoIfConditionIsFalse)
		{
			_expressions.Add(
				Expression.IfThenElse(
					condition.Expression,
					actionToDoConditionIsTrue.ToSmartExpression().Expression,
					actionToDoIfConditionIsFalse.ToSmartExpression().Expression
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
			_expressions.Add(scope.ToSmartExpression().Expression);
			return this;
		}

		public Scope For<TValue>(Variable<TValue> variable, SmartExpression<bool> condition, Scope actionToDo, SmartExpression<TValue> increment)
		{
			actionToDo.AddVariable(variable);
			actionToDo.Assign(variable, increment);
			While(condition, actionToDo);
			return this;
		}


		public Scope While(SmartExpression<bool> condition, Scope actionToDo)
		{

			var breakLabel = Expression.Label("LoopBreak");
			Expression expression = Expression.Loop(
				Expression.IfThenElse(
					condition.Expression,
					actionToDo.ToSmartExpression().Expression,
					Expression.Break(breakLabel)
					),
				breakLabel
				);
			_expressions.Add(expression);
			return this;
		}

		public static implicit operator SmartExpression(Scope scope)
		{
			return scope.ToSmartExpression();
		}
	}
}

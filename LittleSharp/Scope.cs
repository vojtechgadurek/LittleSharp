using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LittleSharp
{
	public class Scope
	{
		Dictionary<string, ParameterExpression> variables = new Dictionary<string, ParameterExpression>();
		List<Expression> expressions = new List<Expression>();
		public SmartExpression<TType> CreateVariable<TType>(string name)
		{
			var variable = Expression.Variable(typeof(TType), name);
			variables.Add(name, variable);
			return new SmartExpression<TType>(variable);
		}
		public void AddExpression(Expression expression)
		{
			expressions.Add(expression);
		}

		public void Assing<T>(Variable<T> variable, SmartExpression<T> value)
		{
			expressions.Add(Expression.Assign(variable.Expression, value.Expression));
		}

	}
}

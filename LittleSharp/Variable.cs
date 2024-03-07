using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LittleSharp
{
	public abstract class Variable
	{
		public abstract Expression GetExpression();
	}
	public class Variable<T> : Variable
	{
		Type _type;
		public readonly Expression Expression;
		public readonly string Name;
		public Variable(Type type, string name)
		{
			Name = name;
			Expression = Expression.Parameter(type, name);
		}

		public override Expression GetExpression()
		{
			return Expression;
		}
		public static implicit operator SmartExpression<T>(Variable<T> variable)
		{
			return new SmartExpression<T>(variable.GetExpression());
		}
	}
}

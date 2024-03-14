using LittleSharp.Literals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LittleSharp.Literals
{
	public class ArrayAccess
	{

		public readonly IndexExpression Expression;
		public ArrayAccess(IndexExpression expression)
		{
			Expression = expression;
		}

		public ArrayAccess<T> SetType<T>()
		{
			return new ArrayAccess<T>(Expression);
		}

		public Expression GetExpression()
		{
			return Expression;
		}

	}
	public class ArrayAccess<T> : ArrayAccess, ILiteral<T>
	{
		public readonly Type Type;
		public readonly SmartExpression<T> SmartExpression;
		public SmartExpression<T> V { get => SmartExpression; }
		public ArrayAccess(IndexExpression expression) : base(expression)
		{
			Type = typeof(T);
			SmartExpression = new SmartExpression<T>(Expression);
		}
		public void Assign(Scope scope, SmartExpression<T> value)
		{
			scope.Assign(this, value);
		}
	}
}

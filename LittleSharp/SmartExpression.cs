using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;


namespace LittleSharp
{
	public class SmartExpression<T>
	{
		public readonly Expression Expression;
		public SmartExpression(Expression expression)
		{
			Expression = expression;
		}

		public SmartExpression<T> this[Expression index]
		{
			get { return new SmartExpression<T>(Expression.ArrayAccess(Expression, index)); }
		}

		public SmartExpression<TNewType> Convert<TNewType>()
		{
			return new SmartExpression<TNewType>(Expression.Convert(Expression, typeof(TNewType)));
		}

		public static SmartExpression<T> operator +(SmartExpression<T> a, SmartExpression<T> b)
		{
			return new SmartExpression<T>(Expression.Add(a.Expression, b.Expression));
		}
		public static SmartExpression<T> operator -(SmartExpression<T> a, SmartExpression<T> b)
		{
			return new SmartExpression<T>(Expression.Subtract(a.Expression, b.Expression));
		}
		public static SmartExpression<T> operator *(SmartExpression<T> a, SmartExpression<T> b)
		{
			return new SmartExpression<T>(Expression.Multiply(a.Expression, b.Expression));
		}
		public static SmartExpression<T> operator /(SmartExpression<T> a, SmartExpression<T> b)
		{
			return new SmartExpression<T>(Expression.Divide(a.Expression, b.Expression));
		}
		public static SmartExpression<T> operator %(SmartExpression<T> a, SmartExpression<T> b)
		{
			return new SmartExpression<T>(Expression.Modulo(a.Expression, b.Expression));
		}
		public static SmartExpression<T> operator &(SmartExpression<T> a, SmartExpression<T> b)
		{
			return new SmartExpression<T>(Expression.And(a.Expression, b.Expression));
		}
		public static SmartExpression<T> operator |(SmartExpression<T> a, SmartExpression<T> b)
		{
			return new SmartExpression<T>(Expression.Or(a.Expression, b.Expression));
		}
		public static SmartExpression<T> operator ^(SmartExpression<T> a, SmartExpression<T> b)
		{
			return new SmartExpression<T>(Expression.ExclusiveOr(a.Expression, b.Expression));
		}
		public static SmartExpression<T> operator <<(SmartExpression<T> a, SmartExpression<T> b)
		{
			return new SmartExpression<T>(Expression.LeftShift(a.Expression, b.Expression));
		}
		public static SmartExpression<T> operator >>(SmartExpression<T> a, SmartExpression<T> b)
		{
			return new SmartExpression<T>(Expression.RightShift(a.Expression, b.Expression));
		}
		public static SmartExpression<T> operator ==(SmartExpression<T> a, SmartExpression<T> b)
		{
			return new SmartExpression<T>(Expression.Equal(a.Expression, b.Expression));
		}
		public static SmartExpression<T> operator !=(SmartExpression<T> a, SmartExpression<T> b)
		{
			return new SmartExpression<T>(Expression.NotEqual(a.Expression, b.Expression));
		}
		public static SmartExpression<T> operator >(SmartExpression<T> a, SmartExpression<T> b)
		{
			return new SmartExpression<T>(Expression.GreaterThan(a.Expression, b.Expression));
		}
		public static SmartExpression<T> operator <(SmartExpression<T> a, SmartExpression<T> b)
		{
			return new SmartExpression<T>(Expression.LessThan(a.Expression, b.Expression));
		}
		public static SmartExpression<T> operator >=(SmartExpression<T> a, SmartExpression<T> b)
		{
			return new SmartExpression<T>(Expression.GreaterThanOrEqual(a.Expression, b.Expression));
		}
		public static SmartExpression<T> operator <=(SmartExpression<T> a, SmartExpression<T> b)
		{
			return new SmartExpression<T>(Expression.LessThanOrEqual(a.Expression, b.Expression));
		}
		public static SmartExpression<T> operator !(SmartExpression<T> a)
		{
			return new SmartExpression<T>(Expression.Not(a.Expression));
		}
		public static SmartExpression<T> operator ~(SmartExpression<T> a)
		{
			return new SmartExpression<T>(Expression.OnesComplement(a.Expression));
		}
		public static SmartExpression<T> operator +(SmartExpression<T> a)
		{
			return new SmartExpression<T>(Expression.UnaryPlus(a.Expression));
		}
		public static SmartExpression<T> operator -(SmartExpression<T> a)
		{
			return new SmartExpression<T>(Expression.Negate(a.Expression));
		}
		public static SmartExpression<T> operator ++(SmartExpression<T> a)
		{
			return new SmartExpression<T>(Expression.Increment(a.Expression));
		}
		public static SmartExpression<T> operator --(SmartExpression<T> a)
		{
			return new SmartExpression<T>(Expression.Decrement(a.Expression));
		}
	}

}
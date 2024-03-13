using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;


namespace LittleSharp
{
	public abstract class SmartExpression
	{
		public readonly Expression Expression;
		public SmartExpression(Expression expression)
		{
			Expression = expression;
		}
	}


	public class SmartExpression<T> : SmartExpression
	{
		static int counter = 0;
		public SmartExpression(Expression expression) : base(expression)
		{

		}

		// array access overload
		public ArrayAccess this[SmartExpression<int> i]
		{
			get { return new ArrayAccess(Expression.ArrayAccess(this.Expression, i.Expression)); }
		}

		public ArrayAccess<TItem> ArrayAccess<TItem>(SmartExpression<int> index)
		{
			return new ArrayAccess<TItem>(Expression.ArrayAccess(Expression, index.Expression));
		}

		public SmartExpression<TNewType> Convert<TNewType>()
		{
			return new SmartExpression<TNewType>(Expression.Convert(Expression, typeof(TNewType)));
		}

		public static implicit operator SmartExpression<T>(T value)
		{
			return new SmartExpression<T>(Expression.Constant(value));
		}


		#region Operators
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
		public static SmartExpression<T> operator <<(SmartExpression<T> a, SmartExpression<int> b)
		{
			return new SmartExpression<T>(Expression.LeftShift(a.Expression, b.Expression));
		}
		public static SmartExpression<T> operator >>(SmartExpression<T> a, SmartExpression<int> b)
		{
			return new SmartExpression<T>(Expression.RightShift(a.Expression, b.Expression));
		}
		public static SmartExpression<bool> operator ==(SmartExpression<T> a, SmartExpression<T> b)
		{
			return new SmartExpression<bool>(Expression.Equal(a.Expression, b.Expression));
		}
		public static SmartExpression<bool> operator !=(SmartExpression<T> a, SmartExpression<T> b)
		{
			return new SmartExpression<bool>(Expression.NotEqual(a.Expression, b.Expression));
		}
		public static SmartExpression<bool> operator >(SmartExpression<T> a, SmartExpression<T> b)
		{
			return new SmartExpression<bool>(Expression.GreaterThan(a.Expression, b.Expression));
		}
		public static SmartExpression<bool> operator <(SmartExpression<T> a, SmartExpression<T> b)
		{
			return new SmartExpression<bool>(Expression.LessThan(a.Expression, b.Expression));
		}
		public static SmartExpression<bool> operator >=(SmartExpression<T> a, SmartExpression<T> b)
		{
			return new SmartExpression<bool>(Expression.GreaterThanOrEqual(a.Expression, b.Expression));
		}
		public static SmartExpression<bool> operator <=(SmartExpression<T> a, SmartExpression<T> b)
		{
			return new SmartExpression<bool>(Expression.LessThanOrEqual(a.Expression, b.Expression));
		}
		public static SmartExpression<bool> operator !(SmartExpression<T> a)
		{
			return new SmartExpression<bool>(Expression.Not(a.Expression));
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
		#endregion
	}



}
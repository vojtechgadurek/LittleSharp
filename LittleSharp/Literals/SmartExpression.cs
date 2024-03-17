using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using LittleSharp.Literals;


namespace LittleSharp.Literals
{
	public abstract class SmartExpression
	{
		public readonly Expression Expression;
		public SmartExpression(Expression expression)
		{
			Expression = expression;
		}
	}


	public class SmartExpression<TValue> : SmartExpression
	{
		static int counter = 0;
		public SmartExpression(Expression expression) : base(expression)
		{

		}

		public Table<TValue, TValueHeld> IsTable<TValueHeld>()
		{
			return new Table<TValue, TValueHeld>(this);
		}

		// array access overload	
		public SmartExpression<TNewType> Convert<TNewType>()
		{
			return new SmartExpression<TNewType>(Expression.Convert(Expression, typeof(TNewType)));
		}

		public static implicit operator SmartExpression<TValue>(TValue value)
		{
			return new SmartExpression<TValue>(Expression.Constant(value));
		}

		public SmartExpression<TValue> Call(MethodInfo method, params SmartExpression[] parameters)
		{
			return new SmartExpression<TValue>(Expression.Call(method, parameters.Select(x => x.Expression).ToArray()));
		}

		public SmartExpression<TValue> Field(FieldInfo field)
		{
			return new SmartExpression<TValue>(Expression.Field(Expression, field));
		}

		#region Operators
		public static SmartExpression<TValue> operator +(SmartExpression<TValue> a, SmartExpression<TValue> b)
		{
			return new SmartExpression<TValue>(Expression.Add(a.Expression, b.Expression));
		}
		public static SmartExpression<TValue> operator -(SmartExpression<TValue> a, SmartExpression<TValue> b)
		{
			return new SmartExpression<TValue>(Expression.Subtract(a.Expression, b.Expression));
		}
		public static SmartExpression<TValue> operator *(SmartExpression<TValue> a, SmartExpression<TValue> b)
		{
			return new SmartExpression<TValue>(Expression.Multiply(a.Expression, b.Expression));
		}
		public static SmartExpression<TValue> operator /(SmartExpression<TValue> a, SmartExpression<TValue> b)
		{
			return new SmartExpression<TValue>(Expression.Divide(a.Expression, b.Expression));
		}
		public static SmartExpression<TValue> operator %(SmartExpression<TValue> a, SmartExpression<TValue> b)
		{
			return new SmartExpression<TValue>(Expression.Modulo(a.Expression, b.Expression));
		}
		public static SmartExpression<TValue> operator &(SmartExpression<TValue> a, SmartExpression<TValue> b)
		{
			return new SmartExpression<TValue>(Expression.And(a.Expression, b.Expression));
		}
		public static SmartExpression<TValue> operator |(SmartExpression<TValue> a, SmartExpression<TValue> b)
		{
			return new SmartExpression<TValue>(Expression.Or(a.Expression, b.Expression));
		}
		public static SmartExpression<TValue> operator ^(SmartExpression<TValue> a, SmartExpression<TValue> b)
		{
			return new SmartExpression<TValue>(Expression.ExclusiveOr(a.Expression, b.Expression));
		}
		public static SmartExpression<TValue> operator <<(SmartExpression<TValue> a, SmartExpression<int> b)
		{
			return new SmartExpression<TValue>(Expression.LeftShift(a.Expression, b.Expression));
		}
		public static SmartExpression<TValue> operator >>(SmartExpression<TValue> a, SmartExpression<int> b)
		{
			return new SmartExpression<TValue>(Expression.RightShift(a.Expression, b.Expression));
		}
		public static SmartExpression<bool> operator ==(SmartExpression<TValue> a, SmartExpression<TValue> b)
		{
			return new SmartExpression<bool>(Expression.Equal(a.Expression, b.Expression));
		}
		public static SmartExpression<bool> operator !=(SmartExpression<TValue> a, SmartExpression<TValue> b)
		{
			return new SmartExpression<bool>(Expression.NotEqual(a.Expression, b.Expression));
		}
		public static SmartExpression<bool> operator >(SmartExpression<TValue> a, SmartExpression<TValue> b)
		{
			return new SmartExpression<bool>(Expression.GreaterThan(a.Expression, b.Expression));
		}
		public static SmartExpression<bool> operator <(SmartExpression<TValue> a, SmartExpression<TValue> b)
		{
			return new SmartExpression<bool>(Expression.LessThan(a.Expression, b.Expression));
		}
		public static SmartExpression<bool> operator >=(SmartExpression<TValue> a, SmartExpression<TValue> b)
		{
			return new SmartExpression<bool>(Expression.GreaterThanOrEqual(a.Expression, b.Expression));
		}
		public static SmartExpression<bool> operator <=(SmartExpression<TValue> a, SmartExpression<TValue> b)
		{
			return new SmartExpression<bool>(Expression.LessThanOrEqual(a.Expression, b.Expression));
		}
		public static SmartExpression<bool> operator !(SmartExpression<TValue> a)
		{
			return new SmartExpression<bool>(Expression.Not(a.Expression));
		}
		public static SmartExpression<TValue> operator ~(SmartExpression<TValue> a)
		{
			return new SmartExpression<TValue>(Expression.OnesComplement(a.Expression));
		}
		public static SmartExpression<TValue> operator +(SmartExpression<TValue> a)
		{
			return new SmartExpression<TValue>(Expression.UnaryPlus(a.Expression));
		}
		public static SmartExpression<TValue> operator -(SmartExpression<TValue> a)
		{
			return new SmartExpression<TValue>(Expression.Negate(a.Expression));
		}
		public static SmartExpression<TValue> operator ++(SmartExpression<TValue> a)
		{
			return new SmartExpression<TValue>(Expression.Increment(a.Expression));
		}
		public static SmartExpression<TValue> operator --(SmartExpression<TValue> a)
		{
			return new SmartExpression<TValue>(Expression.Decrement(a.Expression));
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(this, obj))
			{
				return true;
			}

			if (ReferenceEquals(obj, null))
			{
				return false;
			}

			throw new NotImplementedException();
		}
		#endregion
	}



}
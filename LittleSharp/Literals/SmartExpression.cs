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

		public SmartExpression<TNew> ForceType<TNew>(SmartExpression<TValue> value)
		{
			return new SmartExpression<TNew>(value.Expression);
		}
		public Table<TValue, TValueHeld> ToTable<TValueHeld>()
		{
			return new Table<TValue, TValueHeld>(this);
		}

		public ISimpleSet<TValue, TValueHeld> ToSet<TValueHeld>()
		{
			return new Set<TValue, TValueHeld>(this);
		}
		public SmartExpression<TNewType> Convert<TNewType>()
		{
			return new SmartExpression<TNewType>(Expression.Convert(Expression, typeof(TNewType)));
		}

		public static implicit operator SmartExpression<TValue>(TValue value)
		{
			return new SmartExpression<TValue>(Expression.Constant(value));
		}

		public SmartExpression<TAnswer> Call<TAnswer>(MethodInfo method, params SmartExpression[] parameters)
		{
			return new SmartExpression<TAnswer>(Expression.Call(Expression, method, parameters.Select(x => x.Expression).ToArray()));
		}

		public SmartExpression<TAnswer> Call<TAnswer>(string name, params SmartExpression[] parameters)
		{

			var func = typeof(TValue).GetMethod(name);
			if (func is null) throw new InvalidOperationException($"{name} is not a method of {typeof(TValue).Name}");
			return Call<TAnswer>(func, parameters);
		}


		public SmartExpression<TAnswer> Field<TAnswer>(FieldInfo field)
		{
			return new SmartExpression<TAnswer>(Expression.Field(Expression, field));
		}

		public SmartExpression<TAnswer> Field<TAnswer>(string name)
		{
			var field = typeof(TValue).GetField(name);
			if (field is null) throw new InvalidOperationException($"{name} is not a field of {typeof(TValue).Name}, maybe it is a property");
			return Field<TAnswer>(typeof(TValue).GetField(name)!);
		}

		public SmartExpression<TAnswer> Property<TAnswer>(PropertyInfo field)
		{
			return new SmartExpression<TAnswer>(Expression.Property(Expression, field));
		}

		public SmartExpression<TAnswer> Property<TAnswer>(string name)
		{
			var property = typeof(TValue).GetProperty(name);
			if (property is null) throw new InvalidOperationException($"{name} is not a property of {typeof(TValue).Name}");
			return Property<TAnswer>(property);
		}

		public SmartExpression<TAnswer> PropertyOrField<TAnswer>(string field)
		{
			return new SmartExpression<TAnswer>(Expression.PropertyOrField(Expression, field));
		}



		public SmartExpression<string> ToStringExpression()
		{
			return new SmartExpression<string>(Expression.Call(Expression, typeof(object).GetMethod("ToString")!));
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
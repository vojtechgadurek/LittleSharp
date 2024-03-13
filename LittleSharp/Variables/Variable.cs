using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace LittleSharp
{
	public interface IAssingableExpression<T>
	{
		void Assing(Scope scope, SmartExpression<T> expression);
		SmartExpression<T> V { get; }
		Expression GetExpression();
	}
	public abstract class Variable
	{
		public abstract Expression GetExpression();
	}

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
	public class ArrayAccess<T> : ArrayAccess, IAssingableExpression<T>
	{
		public readonly Type Type;
		public readonly SmartExpression<T> SmartExpression;
		public SmartExpression<T> V { get => SmartExpression; }
		public ArrayAccess(IndexExpression expression) : base(expression)
		{
			Type = typeof(T);
			SmartExpression = new SmartExpression<T>(Expression);
		}
		public void Assing(Scope scope, SmartExpression<T> value)
		{
			scope.Assign(this, value);
		}
	}
	public class Variable<T> : Variable, IAssingableExpression<T>
	{
		public readonly Type Type;
		public readonly ParameterExpression Expression;
		public readonly string Name;
		public readonly SmartExpression<T> SmartExpression;
		public SmartExpression<T> V { get => SmartExpression; }

		public Variable(string name)
		{
			Name = name;
			Type = typeof(T);
			Expression = System.Linq.Expressions.Expression.Parameter(Type, name);
			SmartExpression = new SmartExpression<T>(Expression);
		}

		public void Assing(Scope scope, SmartExpression<T> value)
		{
			scope.Assign(this, value);
		}

		public override Expression GetExpression()
		{
			return Expression;
		}
		public (Type, string) GetInfo()
		{
			return (Type, Name);
		}
	}
}

using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using LittleSharp.Literals;

namespace LittleSharp
{
	public abstract class Variable
	{
		public abstract Expression GetExpression();
	}


	public class Variable<T> : Variable, ILiteral<T>
	{
		public readonly Type Type;
		public readonly ParameterExpression Expression;
		public readonly string Name;
		public readonly SmartExpression<T> SmartExpression;
		public SmartExpression<T> V { get => SmartExpression; }

		public Variable()
		{
			Type = typeof(T);
			Expression = System.Linq.Expressions.Expression.Parameter(Type);
			SmartExpression = new SmartExpression<T>(Expression);
		}

		public void Assign(Scope scope, SmartExpression<T> value)
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LittleSharp.Literals
{
	public interface ILiteral<T>
	{
		void Assign(Scope scope, SmartExpression<T> expression);
		SmartExpression<T> V { get; }
		Expression GetExpression();
	}
}

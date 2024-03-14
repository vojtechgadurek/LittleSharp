using LittleSharp.Callables;
using LittleSharp.Literals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LittleSharp.Actions
{
	public class Transfer<TIn, TOut>
	{
		SmartExpression<TIn> _expression;
		public SmartExpression<TOut> Select(CompiledFunction<TIn, TOut> function, SmartExpression<int> numberOfItems)
		{
			var f = CompiledFunctions.Create()

		}
	}



}

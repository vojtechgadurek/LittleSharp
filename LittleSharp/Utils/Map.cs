using LittleSharp.Callables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LittleSharp.Utils
{
	public static class Map
	{
		static public Expression<Func<TKey, TValue>> GetMap<TKey, TValue>(IEnumerable<(TKey, TValue)> values)
		{
			var f = CompiledFunctions.Create<TKey, TValue>(out var key_);
			foreach (var value in values)
			{
				f.S.IfThen(key_.V == value.Item1, new Scope().Assign(f.Output, value.Item2).GoToEnd(f.Scope));
			}
			return f.Construct();
		}
	}
}

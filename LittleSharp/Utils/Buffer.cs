using LittleSharp.Callables;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LittleSharp.Utils
{

	public static class Buffering
	{/// <summary>
	 /// Takes a function f2 : (a -> b) and returns action f2 : (a[] -> b[] -> int -> int -> void)
	 /// a[] input, b[] output, int start, int size
	 /// </summary>
	 /// <typeparam name="TInput"></typeparam>
	 /// <typeparam name="TOutput"></typeparam>
	 /// <param name="bufferedFunction"></param>
	 /// <returns></returns>
		public static Expression<Action<TInput[], TOutput[], int, int>> BufferFunction<TInput, TOutput>(
			Expression<Func<TInput, TOutput>> bufferedFunction)
		{
			var f = CompiledActions.Create<TInput[], TOutput[], int, int>(
				out var input_,
				out var output_,
				out var start_,
				out var size_
				);
			f.S.DeclareVariable<int>(out var i_, start_.V)
				.While(
					i_.V < size_.V,
					new Scope()
						.Macro(out var input_T, input_.V.ToTable<TInput>())
						.Macro(out var output_T, output_.V.ToTable<TOutput>())
						.Function(bufferedFunction, input_T[i_.V].V, out var hash_)
						.Assign(output_T[i_.V], hash_)
						.Assign(i_, i_.V + 1)
						);
			return f.Construct();
		}

	}
}

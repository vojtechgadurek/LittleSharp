using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using LittleSharp.Callables;

namespace LittleSharp.Callables
{
	public static class CompiledFunctions
	{
		public static CompiledFunction<TOut> Create<TOut>()
		{
			return new CompiledFunction<TOut>();
		}

		public static CompiledFunction<TInFirst, TOut> Create<TInFirst, TOut>(out Variable<TInFirst> input)
		{
			return new CompiledFunction<TInFirst, TOut>(out input);
		}
		public static CompiledFunction<TInFirst, TInSecond, TOut> Create<TInFirst, TInSecond, TOut>(out Variable<TInFirst> first, out Variable<TInSecond> second)
		{
			return new CompiledFunction<TInFirst, TInSecond, TOut>(out first, out second);
		}
		public static CompiledFunction<TInFirst, TInSecond, TInThird, TOut> Create<TInFirst, TInSecond, TInThird, TOut>(out Variable<TInFirst> first, out Variable<TInSecond> second, out Variable<TInThird> third)
		{
			return new CompiledFunction<TInFirst, TInSecond, TInThird, TOut>(out first, out second, out third);
		}
	}
	public abstract class CompiledFunctionBase<TOut>
	{
		internal Lambda<TOut> _baseFunction;
		public Variable<TOut> Output => _baseFunction.Output;
		public CompiledFunctionBase() : base()
		{
			_baseFunction = new Lambda<TOut>();
		}
		public Scope Scope => _baseFunction;
		public Scope S => Scope;

	}
	public class CompiledFunction<TOut> : CompiledFunctionBase<TOut>
	{
		public CompiledFunction() : base()
		{
		}

		public Expression<Func<TOut>> Construct()
		{
			return (Expression<Func<TOut>>)_baseFunction.Construct(new ParameterValuePairs());
		}


	}
	public class CompiledFunction<TInFirst, TOut> : CompiledFunctionBase<TOut>
	{
		public CompiledFunction(out Variable<TInFirst> input) : base()
		{
			input = _baseFunction.DeclareParameter<TInFirst>("input");
		}
		public Expression<Func<TInFirst, TOut>> Construct()
		{
			return (Expression<Func<TInFirst, TOut>>)_baseFunction.Construct(new ParameterValuePairs());
		}
	}
	public class CompiledFunction<TInFirst, TInSecond, TOut> : CompiledFunctionBase<TOut>
	{
		public CompiledFunction(out Variable<TInFirst> first, out Variable<TInSecond> second) : base()
		{
			first = _baseFunction.DeclareParameter<TInFirst>("first");
			second = _baseFunction.DeclareParameter<TInSecond>("second");
		}
		public Expression<Func<TInFirst, TInSecond, TOut>> Construct()
		{
			return (Expression<Func<TInFirst, TInSecond, TOut>>)_baseFunction.Construct(new ParameterValuePairs());
		}
	}

	public class CompiledFunction<TInFirst, TInSecond, TInThird, TOut> : CompiledFunctionBase<TOut>
	{
		public CompiledFunction(out Variable<TInFirst> first, out Variable<TInSecond> second, out Variable<TInThird> third) : base()
		{
			first = _baseFunction.DeclareParameter<TInFirst>("first");
			second = _baseFunction.DeclareParameter<TInSecond>("second");
			third = _baseFunction.DeclareParameter<TInThird>("third");
		}
		public Expression<Func<TInFirst, TInSecond, TInThird, TOut>> Construct()
		{
			return (Expression<Func<TInFirst, TInSecond, TInThird, TOut>>)_baseFunction.Construct(new ParameterValuePairs());
		}
	}


}

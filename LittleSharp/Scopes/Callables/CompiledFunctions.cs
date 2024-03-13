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
		internal Lambda<TOut> _lambda;
		public Variable<TOut> Output => _lambda.Output;
		public CompiledFunctionBase() : base()
		{
			_lambda = new Lambda<TOut>();
		}
		public Scope Scope => _lambda;
		public Scope S => Scope;

	}
	public class CompiledFunction<TOut> : CompiledFunctionBase<TOut>
	{
		Type _type = typeof(Func<TOut>);
		public CompiledFunction() : base()
		{
		}

		public Expression<Func<TOut>> Construct()
		{
			return (Expression<Func<TOut>>)_lambda.Construct(_type, new ParameterValuePairs());
		}


	}
	public class CompiledFunction<TInFirst, TOut> : CompiledFunctionBase<TOut>
	{
		Type _type = typeof(Func<TInFirst, TOut>);
		public CompiledFunction(out Variable<TInFirst> input) : base()
		{
			input = _lambda.DeclareParameter<TInFirst>("input");
		}
		public Expression<Func<TInFirst, TOut>> Construct()
		{
			return (Expression<Func<TInFirst, TOut>>)_lambda.Construct(_type, new ParameterValuePairs());
		}
	}
	public class CompiledFunction<TInFirst, TInSecond, TOut> : CompiledFunctionBase<TOut>
	{
		Type _type = typeof(Func<TInFirst, TInSecond, TOut>);
		public CompiledFunction(out Variable<TInFirst> first, out Variable<TInSecond> second) : base()
		{
			first = _lambda.DeclareParameter<TInFirst>("first");
			second = _lambda.DeclareParameter<TInSecond>("second");
		}
		public Expression<Func<TInFirst, TInSecond, TOut>> Construct()
		{
			return (Expression<Func<TInFirst, TInSecond, TOut>>)_lambda.Construct(_type, new ParameterValuePairs());
		}
	}

	public class CompiledFunction<TInFirst, TInSecond, TInThird, TOut> : CompiledFunctionBase<TOut>
	{
		Type _type = typeof(Func<TInFirst, TInSecond, TInThird, TOut>);
		public CompiledFunction(out Variable<TInFirst> first, out Variable<TInSecond> second, out Variable<TInThird> third) : base()
		{
			first = _lambda.DeclareParameter<TInFirst>("first");
			second = _lambda.DeclareParameter<TInSecond>("second");
			third = _lambda.DeclareParameter<TInThird>("third");
		}
		public Expression<Func<TInFirst, TInSecond, TInThird, TOut>> Construct()
		{
			return (Expression<Func<TInFirst, TInSecond, TInThird, TOut>>)_lambda.Construct(_type, new ParameterValuePairs());
		}
	}


}

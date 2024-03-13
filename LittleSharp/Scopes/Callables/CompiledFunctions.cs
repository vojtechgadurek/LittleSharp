using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using LittleSharp.Scopes.Callables;

namespace LittleSharp.Callables
{
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
	public class CompiledFunction<TOut>
	{
		Lambda<TOut> baseFunction;
		public CompiledFunction(out Variable output) : base()
		{
			baseFunction = new Lambda<TOut>();
			output = baseFunction.Output;
		}
		public Scope Scope => baseFunction;
		public Scope S => Scope;

	}
	public class CompiledFunction<TInFirst, TOut> : CompiledFunctionBase<TOut>
	{
		public CompiledFunction(out Variable<TInFirst> input) : base()
		{
			input = _baseFunction.DeclareParameter<TInFirst>("input");
		}
		public Expression<Func<TInFirst, TOut>> Get()
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
		public Expression<Func<TInFirst, TInSecond, TOut>> Get()
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
		public Expression<Func<TInFirst, TInSecond, TInThird, TOut>> Get()
		{
			return (Expression<Func<TInFirst, TInSecond, TInThird, TOut>>)_baseFunction.Construct(new ParameterValuePairs());
		}
	}


}

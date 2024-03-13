using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LittleSharp.Callables
{
	public static class CompiledActions
	{
		public static CompiledAction Create()
		{
			return new CompiledAction();
		}
		public static CompiledAction<TInFirst> Create<TInFirst>(out Variable<TInFirst> input)
		{
			return new CompiledAction<TInFirst>(out input);
		}
		public static CompiledAction<TInFirst, TInSecond> Create<TInFirst, TInSecond>(out Variable<TInFirst> first, out Variable<TInSecond> second)
		{
			return new CompiledAction<TInFirst, TInSecond>(out first, out second);
		}
		public static CompiledAction<TInFirst, TInSecond, TInThird> Create<TInFirst, TInSecond, TInThird>(out Variable<TInFirst> first, out Variable<TInSecond> second, out Variable<TInThird> third)
		{
			return new CompiledAction<TInFirst, TInSecond, TInThird>(out first, out second, out third);
		}
	}
	public abstract class CompiledActionBase
	{
		internal Lambda _lambda;
		public CompiledActionBase() : base()
		{
			_lambda = new Lambda();
		}
		public Scope Scope => _lambda;
		public Scope S => Scope;

	}
	public class CompiledAction : CompiledActionBase
	{
		Type _type = typeof(Action);
		public CompiledAction() : base()
		{
		}
		public Expression<Action> Construct()
		{
			return (Expression<Action>)_lambda.Construct(_type, new ParameterValuePairs());
		}
	}
	public class CompiledAction<TInFirst> : CompiledActionBase
	{
		Type _type = typeof(Action<TInFirst>);
		public CompiledAction(out Variable<TInFirst> input) : base()
		{
			input = _lambda.DeclareParameter<TInFirst>("input");
		}
		public Expression<Action<TInFirst>> Construct()
		{
			return (Expression<Action<TInFirst>>)_lambda.Construct(_type, new ParameterValuePairs());
		}
	}
	public class CompiledAction<TInFirst, TInSecond> : CompiledActionBase
	{
		Type _type = typeof(Action<TInFirst, TInSecond>);
		public CompiledAction(out Variable<TInFirst> first, out Variable<TInSecond> second) : base()
		{
			first = _lambda.DeclareParameter<TInFirst>("first");
			second = _lambda.DeclareParameter<TInSecond>("second");
		}
		public Expression<Action<TInFirst, TInSecond>> Construct()
		{
			return (Expression<Action<TInFirst, TInSecond>>)_lambda.Construct(_type, new ParameterValuePairs());
		}
	}

	public class CompiledAction<TInFirst, TInSecond, TInThird> : CompiledActionBase
	{
		Type _type = typeof(Action<TInFirst, TInSecond, TInThird>);
		public CompiledAction(out Variable<TInFirst> first, out Variable<TInSecond> second, out Variable<TInThird> third) : base()
		{
			first = _lambda.DeclareParameter<TInFirst>("first");
			second = _lambda.DeclareParameter<TInSecond>("second");
			third = _lambda.DeclareParameter<TInThird>("third");
		}
		public Expression<Action<TInFirst, TInSecond, TInThird>> Construct()
		{
			return (Expression<Action<TInFirst, TInSecond, TInThird>>)_lambda.Construct(_type, new ParameterValuePairs());
		}
	}
}

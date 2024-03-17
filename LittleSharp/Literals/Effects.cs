using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace LittleSharp.Literals
{
	public interface ISmartExpression<TExpression>
	{
		SmartExpression<TExpression> V { get; }
	}
	public interface IAdd<TExpression, TValue> : ISmartExpression<TExpression>
	{
		SmartExpression<NoneType> Add(SmartExpression<TValue> value)
		{
			return new SmartExpression<NoneType>(Expression.Call(V.Expression, typeof(TValue).GetMethod("Add")!, value.Expression));
		}
	}
	public interface IRemove<TExpression, TValue> : ISmartExpression<TExpression>
	{
		SmartExpression<NoneType> Remove(SmartExpression<TValue> value)
		{
			return new SmartExpression<NoneType>(Expression.Call(V.Expression, typeof(TValue).GetMethod("Remove")!, value.Expression));
		}
	}

	public interface IContains<TExpression, TValue> : ISmartExpression<TExpression>
	{
		SmartExpression<bool> Contains(SmartExpression<TValue> value)
		{
			return new SmartExpression<bool>(Expression.Call(V.Expression, typeof(TValue).GetMethod("Contains")!, value.Expression));
		}
	}

	public interface ICount : ISmartExpression<int>
	{
		SmartExpression<int> Count()
		{
			return new SmartExpression<int>(Expression.Call(V.Expression, typeof(ICollection<>).GetMethod("Count")!));
		}
	}

	public interface ISimpleSet<TSet, TValue> : IAdd<TSet, TValue>, IRemove<TSet, TValue>, IContains<TSet, TValue>
	{
	}
}

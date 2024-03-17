using Microsoft.Diagnostics.Tracing.Parsers.MicrosoftWindowsTCPIP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using LittleSharp.Callables;
using System.Net.WebSockets;
using Microsoft.CodeAnalysis;

namespace LittleSharp.Literals
{
	public class Table<TTable, TValue>
	{
		SmartExpression<TTable> _table;
		public SmartExpression<TTable> V { get => _table; }

		public Table(SmartExpression<TTable> smartExpression)
		{
			this._table = smartExpression;
		}

		public ArrayAccess<TValue> this[SmartExpression<int> i]
		{
			get { return new ArrayAccess<TValue>(Expression.ArrayAccess(_table.Expression, i.Expression)); }
		}

		public ArrayAccess<TValue> this[SmartExpression<ulong> i]
		{
			get { return new ArrayAccess<TValue>(Expression.ArrayAccess(_table.Expression, i.Expression)); }
		}

		public Table<TOutputTable, TOutput> Select<TOutputTable, TOutput>(
			Expression<Func<TValue, TOutput>> function, Table<TOutputTable, TOutput> outputTable, SmartExpression<int> numberOfItems)
		{
			var Scope = new Scope<TOutputTable>();
			Scope
				.DeclareVariable(out var inputTable_, _table)
				.DeclareVariable(out var outputTable_, outputTable.V)
				//Main loop
				.DeclareVariable<int>(out var i_, 0)
				.While(
					i_.V < numberOfItems,
					new Scope()
						.Function(function, inputTable_.V.IsTable<TValue>()[i_.V].V, out var returnValue)
						.Assign(outputTable_.V.IsTable<TOutput>()[i_.V], returnValue)
						.Assign(i_, i_.V + 1)
						)
				.Assign(Scope.Output, outputTable_.V);
			return Scope.Construct().IsTable<TOutput>();
		}
		public SmartExpression<NoneType> ForEach(Expression<Action<TValue>> action, SmartExpression<int> numberOfItems)
		{
			var Scope = new Scope();
			Scope
				.DeclareVariable(out var inputTable_, _table)
				//Main loop
				.DeclareVariable<int>(out var i_, 0)
				.While(
						i_.V < numberOfItems,
						new Scope()
							.Action(action, inputTable_.V.IsTable<TValue>()[i_.V].V)
							.Assign(i_, i_.V + 1)
				);
			return Scope.Construct();
		}
	}
}

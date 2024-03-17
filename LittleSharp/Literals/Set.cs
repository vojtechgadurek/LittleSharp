using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace LittleSharp.Literals
{
	public class Set<TSet, TValue> : ISimpleSet<TSet, TValue>
	{
		SmartExpression<TSet> _set;
		public SmartExpression<TSet> V { get => _set; }
		public Set(SmartExpression<TSet> set)
		{
			_set = set;
		}

	}
}

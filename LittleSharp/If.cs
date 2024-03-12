using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LittleSharp
{
	internal class If : Scope
	{
		Scope _parent;
		List<Scope> _scopes = new List<Scope>();
		public If(SmartExpression<bool> condition, Scope parent)
		{
		}

		public Scope Else(Scope actions)
		{
			Scope scope = new Scope();
			_scopes.Add(scope);
			return scope;
		}

		public Scope End()
		{
			return _parent;
		}

	}
}
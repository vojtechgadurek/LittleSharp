using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using LittleSharp.Literals;

namespace LittleSharp.Callables
{

	public class ParameterValuePairs()
	{
		public readonly List<(Variable, SmartExpression)> Pairs = new List<(Variable, SmartExpression)>();
		public ParameterValuePairs SetParameterToValue<T>(Variable<T> variable, SmartExpression<T> value)
		{
			Pairs.Add((variable, value));
			return this;
		}
	}

	public class Lambda<T> : Lambda
	{
		public readonly Variable<T> Output = new Variable<T>("Ouput");

		public Lambda(string? functionName = null) : base(functionName)
		{
			ReturnValue = Output.Expression;
		}
	}

	public class Lambda : Scope
	{
		List<Variable> _parameters = new List<Variable>();
		public bool IsAction() => ReturnValue is null;
		public ParameterExpression? ReturnValue;

		public Lambda(string? name = null) : base(name)
		{
			ReturnValue = null;
		}
		internal Variable<T> DeclareParameter<T>(string name)
		{
			var parameter = new Variable<T>(name);
			_parameters.Add(parameter);
			return parameter;
		}

		internal Lambda DeclareParameter<TValue>(string name, out Variable<TValue> variable)
		{
			var v = DeclareParameter<TValue>(name);
			variable = v;
			return this;
		}

		internal IEnumerable<ParameterExpression> CreateVariablesFromAssignedParamters(ParameterValuePairs assignedParemeters)
		{
			var parameters = assignedParemeters.Pairs
			   .Select(x => x.Item1.GetExpression())
			   .Cast<ParameterExpression>();
			if (IsAction())
			{
				return parameters;
			}
			else
			{
				return parameters.Append(ReturnValue)!;
			}
		}

		internal Expression CreateMainScopeBlock(IEnumerable<ParameterExpression> assignedParameters, Expression valuesToParametersAssignmets)
		{
			if (IsAction())
			{
				return Expression.Block(assignedParameters, valuesToParametersAssignmets, base.ToSmartExpression().Expression);
			}
			else
			{
				return Expression.Block(assignedParameters, valuesToParametersAssignmets, base.ToSmartExpression().Expression, ReturnValue!);
			}
		}

		public Expression Construct(Type type)
		{
			return Construct(type, new ParameterValuePairs());
		}

		public Expression Construct(Type type, ParameterValuePairs parameterValuePairs)
		{
			// How it works?
			// lambda[not assigned parameters]{
			//		MainBlock[assigned parameters]{
			// 			Block{ assign values to parameters},
			// 			Block{ scope block}	
			//			Return Value?
			// 	     }
			// }



			var parametersWithAssignedValue = parameterValuePairs.Pairs;

			var assignedParameters = CreateVariablesFromAssignedParamters(parameterValuePairs);

			//Create block that holds the assignments of the parameters
			var setParametersBlock = Expression.Block(
					parametersWithAssignedValue.Select(x => Expression.Assign(x.Item1.GetExpression(), x.Item2.Expression))
					);

			//Main block holding action done in the scope
			var functionBlock = CreateMainScopeBlock(assignedParameters, setParametersBlock);

			//Filter out the parameters that have been assigned a value
			var functionParameters = _parameters
				.Where(x => !parametersWithAssignedValue.Any(y => x == y.Item1))
				.Select(x => x.GetExpression())
				.Cast<ParameterExpression>();

			return Expression.Lambda(type, functionBlock, functionParameters);
		}

	}
}

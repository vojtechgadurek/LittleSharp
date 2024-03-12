using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace LittleSharp
{


	public class FunctionParametersAssigment()
	{
		public readonly List<(Variable, SmartExpression)> VariablesAndItsAssigments = new List<(Variable, SmartExpression)>();
		public FunctionParametersAssigment SetParameterToValue<T>(Variable<T> variable, SmartExpression<T> smartExpression)
		{
			VariablesAndItsAssigments.Add((variable, smartExpression));
			return this;
		}
	}
	public class Function<T> : Scope
	{
		List<Variable> _parameters = new List<Variable>();
		public readonly LabelTarget ReturnLabel = Expression.Label("return");
		public readonly SmartExpression<NoneType> ReturnExpression;
		public readonly Variable<T> ReturnVariable = new Variable<T>("Return");

		public Function(string? functionName = null) : base(functionName)
		{
			ReturnExpression = new SmartExpression<NoneType>(Expression.Goto(ReturnLabel));
		}


		public Variable<T> DeclareParameter<T>(string name)
		{
			var parameter = new Variable<T>(name);
			_parameters.Add(parameter);
			return parameter;
		}

		public Function<T> DeclareParameter<TValue>(string name, out Variable<TValue> variable)
		{
			var v = DeclareParameter<TValue>(name);
			variable = v;
			return this;
		}

		public SmartExpression<VarType> Get()
		{
			return Get(new FunctionParametersAssigment());
		}
		public SmartExpression<VarType> Get(FunctionParametersAssigment setParameters)
		{
			var parametersWithAssignedValue = setParameters.VariablesAndItsAssigments;
			var setParametersBlock = Expression.Block(
				parametersWithAssignedValue.Select(x => Expression.Assign(x.Item1.GetExpression(), x.Item2.Expression))
				);

			var assignedParameters = parametersWithAssignedValue
				.Select(x => x.Item1.GetExpression())
				.Append(ReturnVariable.Expression)
				.Cast<ParameterExpression>();



			var functionBlock = Expression.Block(assignedParameters, setParametersBlock, base.ToSmartExpression().Expression, Expression.Label(ReturnLabel), ReturnVariable.Expression);

			var functionParameters = _parameters
				.Where(x => !parametersWithAssignedValue.Any(y => x == y.Item1))
				.Select(x => x.GetExpression())
				.Cast<ParameterExpression>();

			return new SmartExpression<VarType>(Expression.Lambda(functionBlock, functionParameters));
		}
	}
}

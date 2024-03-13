using LittleSharp;
using System.Linq.Expressions;

namespace LittleSharp
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

	public class Lambda : Scope
	{
		List<Variable> _parameters = new List<Variable>();
		public readonly LabelTarget ReturnLabel = Expression.Label("return");
		public bool IsAction() => ReturnValue is not null;
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
				return Expression.Block(assignedParameters, valuesToParametersAssignmets, base.ToSmartExpression().Expression, Expression.Label(ReturnLabel));
			}
			else
			{
				return Expression.Block(assignedParameters, valuesToParametersAssignmets, base.ToSmartExpression().Expression, Expression.Label(ReturnLabel), ReturnValue!);
			}
		}

		public Expression Construct(ParameterValuePairs parameterValuePairs)
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

			return Expression.Lambda(functionBlock, functionParameters);
		}

	}



	public class BaseFunction<T> : Lambda
	{
		public readonly SmartExpression<NoneType> ReturnExpression;
		public readonly Variable<T> ReturnVariable = new Variable<T>("Return");

		public BaseFunction(string? functionName = null) : base(functionName)
		{
			ReturnExpression = new SmartExpression<NoneType>(Expression.Goto(ReturnLabel));
		}
	}

	public class CompiledFunction<TOut>
	{
	}

	public class CompiledFunction<TIn, TOut>
	{
		BaseFunction<TOut> baseFunction;
		public CompiledFunction(out Variable<TIn> input, out Variable output) : base()
		{
			baseFunction = new BaseFunction<TOut>();
			input = baseFunction.DeclareParameter<TIn>("input");
			output = baseFunction.ReturnVariable;
		}
		public Scope Scope => baseFunction;
		public Scope S => Scope;

	}

	public class CompiledAction<TIn>
	{

	}
}

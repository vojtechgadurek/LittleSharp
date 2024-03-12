using LittleSharp;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace LittleSharpTests
{
	public class BasicTests
	{
		[Fact]
		public void TestPlus()
		{
			var func = new Function<int>();
			var a = func.DeclareParameter<int>("a");
			var b = func.DeclareParameter<int>("b");

			func.Assing(func.ReturnVariable, a.V + b.V);

			var del = ((Expression<Func<int, int, int>>)func.Get(new FunctionParametersAssigment()).Expression);
			var fun = del.Compile();
			Assert.Equal(3, fun(1, 2));
		}

		[Theory]
		[InlineData(false)]
		[InlineData(true)]
		public void TestIf(bool inputBool)
		{

			var func = new Function<int>();
			func
				.DeclareParameter<bool>("input", out var input)
				.IfThen(
				input.V,
				new Scope()
					.Assing(func.ReturnVariable, 1)
					.AddExpression(func.ReturnExpression)
					)
				.Assing(func.ReturnVariable, 0);

			var del = ((Expression<Func<bool, int>>)func.Get(new FunctionParametersAssigment()).Expression);
			var fun = del.Compile();
			Assert.Equal(inputBool ? 1 : 0, fun(inputBool));
		}

		[Fact]
		public void TestScopeBasic()
		{
			var x = Expression.Parameter(typeof(int), "x");
			var a = Expression.Assign(x, Expression.Constant(1));
			var block = Expression.Block(new[] { x }, a, x);
			var del = Expression.Lambda<Func<int>>(block).Compile();
			Assert.Equal(1, del());

		}

		[Fact]
		public void TestScope()
		{
			var func = new Function<int>("TestScope");
			func.DeclareVariable<int>("a", out var a)
				.Assing(a, 1);
			;

			func.Assing(func.ReturnVariable, a.V);

			var x = func.ToSmartExpression();


			var del = (func.Get().Expression);

			Assert.Equal(1, ((Expression<Func<int>>)del).Compile()());


		}

		[Fact]
		public void TestSetParameters()
		{
			var func = new Function<int>("TestScope");
			func
				.DeclareParameter<int>("a", out var a)
				.Assing(func.ReturnVariable, a.V);

			var del = (func.Get(new FunctionParametersAssigment().SetParameterToValue(a, 1)).Expression);
			var fun = ((Expression<Func<int>>)del).Compile();
			Assert.Equal(1, fun());
		}
	}


	// if(a < b)(
}
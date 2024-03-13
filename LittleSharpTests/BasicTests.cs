using LittleSharp;
using LittleSharp.Callables;
using System.Linq.Expressions;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;

namespace LittleSharpTests
{
	public class BasicTests
	{
		[Fact]
		public void TestPlus()
		{
			var func = CompiledFunctions.Create<int, int, int>(out var a, out var b);

			func.Output.Assing(func.S, a.V + b.V);

			var del = func.Construct();
			var fun = del.Compile();
			Assert.Equal(3, fun(1, 2));
		}

		[Theory]
		[InlineData(false)]
		[InlineData(true)]
		public void TestIf(bool inputBool)
		{

			var func = CompiledFunctions.Create<bool, int>(out var input);
			func.S
				.IfThen(
				input.V,
				new Scope()
					.Assign(func.Output, 1)
					)
				.Assign(func.Output, 0);

			var del = func.Construct();
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
	}


	// if(a < b)(
}
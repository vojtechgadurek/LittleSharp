using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace LittleSharp.Benchmarks
{
	public class TestDivision
	{
		static ulong[] ulongs;
		static ulong[] division;
		public TestDivision()
		{
			ulongs = new ulong[100];
			division = new ulong[100];
			Random random = new Random(42);
			for (int i = 0; i < ulongs.Length; i++)
			{
				ulongs[i] = (ulong)random.NextInt64();

			}
			for (int i = 0; i < division.Length; i++)
			{
				division[i] = (ulong)random.NextInt64();
				if (division[i] == 0)
				{
					division[i] = 1;
				}
			}
		}

		public ulong Division()
		{
			ulong ansver = 0;
			for (int j = 0; j < division.Length; j++)
			{

				for (int i = 0; i < ulongs.Length; i++)
				{
					ansver += ulongs[i] / division[j];
				}
			}
			return ansver;
		}
		[Benchmark]
		public ulong DivisionWithSmartExpression()
		{
			ulong ansver = 0;
			for (int j = 0; j < division.Length; j++)
			{
				Function<ulong> function = new Function<ulong>();
				function
					.DeclareParameter<ulong[]>("input", out Variable<ulong[]> input)
					.DeclareVariable<ulong>("localAnsver", out Variable<ulong> localAnsver)
					.SubScope(
						new Scope()
						.DeclareVariable("i", out Variable<int> i)
						.Assing(i, 0)
						.Assing(localAnsver, 0)
						.While(
							i.V < ulongs.Length,
							new Scope()
							.Assing(localAnsver, localAnsver.V + input.V.ArrayAccess<ulong>(i.V).V / division[j])
							.Assing(i, i.V + 1)
						)
					);
				function.Assing(function.ReturnVariable, localAnsver.V);

				var del = ((Expression<Func<ulong[], ulong>>)function.Get(new FunctionParametersAssigment()).Expression);
				var fun = del.Compile();
				ansver += fun(ulongs);
			}
			return ansver;
		}

		[Benchmark]
		public ulong DivisionWithSmartExpressionB()
		{
			ulong ansver = 0;
			Function<ulong> function = new Function<ulong>();
			function
				.DeclareParameter<ulong[]>("input", out Variable<ulong[]> input)
				.DeclareParameter<ulong>("divisor", out Variable<ulong> divisor)
				.DeclareVariable<ulong>("localAnsver", out Variable<ulong> localAnsver)
				.SubScope(
					new Scope()
					.DeclareVariable("i", out Variable<int> i)
					.Assing(i, 0)
					.Assing(localAnsver, 0)
					.While(
						i.V < ulongs.Length,
						new Scope()
						.Assing(localAnsver, localAnsver.V + input.V.ArrayAccess<ulong>(i.V).V / divisor.V)
						.Assing(i, i.V + 1)
					)
				);
			function.Assing(function.ReturnVariable, localAnsver.V);
			for (int j = 0; j < division.Length; j++)
			{


				var del = ((Expression<Func<ulong[], ulong>>)function.Get(new FunctionParametersAssigment().SetParameterToValue(divisor, 1)).Expression);
				var fun = del.Compile();
				ansver += fun(ulongs);
			}
			return ansver;
		}

	}
}

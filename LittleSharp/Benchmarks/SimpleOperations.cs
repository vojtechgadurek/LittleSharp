using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using LittleSharp.Callables;

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
				Lambda<ulong> function = new Lambda<ulong>();
				function
					.DeclareParameter<ulong[]>("input", out Variable<ulong[]> input)
					.DeclareVariable<ulong>("localAnswer", out Variable<ulong> localAnswer)
					.SubScope(
						new Scope()
						.DeclareVariable("i", out Variable<int> i)
						.Assign(i, 0)
						.Assign(localAnswer, 0)
						.While(
							i.V < ulongs.Length,
							new Scope()
							.Assign(localAnswer, localAnswer.V + input.V.ArrayAccess<ulong>(i.V).V / division[j])
							.Assign(i, i.V + 1)
						)
					);
				function.Assign(function.Output, localAnswer.V);

				var del = ((Expression<Func<ulong[], ulong>>)function.Construct(typeof(Expression<Func<ulong[], ulong>>)));
				var fun = del.Compile();
				ansver += fun(ulongs);
			}
			return ansver;
		}
	}
}

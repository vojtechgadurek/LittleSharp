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
			return ansver;
		}
	}
}

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace LittleSharpGenerator
{

	public static class BaseNamespace
	{
		public static string Attribute = @"
namespace Generated{
}
";
	}
	[Generator]
	public class Generator : IIncrementalGenerator
	{
		public void Initialize(IncrementalGeneratorInitializationContext initContext)
		{
			// define the execution pipeline here via a series of transformations:

			// find all additional files that end with .txt
			IncrementalValuesProvider<AdditionalText> textFiles = initContext.AdditionalTextsProvider.Where(file => file.Path.EndsWith(".txt"));

			// read their contents and save their name
			IncrementalValuesProvider<(string name, string content)> namesAndContents = textFiles.Select((text, cancellationToken) =>
			(name: Path.GetFileNameWithoutExtension(text.Path), content: text.GetText(cancellationToken).ToString()));

			// generate a class that contains their values as const strings
			initContext.RegisterSourceOutput(namesAndContents, (spc, nameAndContent) =>
			{
				spc.AddSource($"ConstStrings.{nameAndContent.name}", $@"
    public static partial class ConstStrings
    {{
        public const string {nameAndContent.name} = ""{nameAndContent.content}"";
    }}");
			});
		}
	}
}

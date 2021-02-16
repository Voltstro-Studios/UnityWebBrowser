using System;
using System.Diagnostics.CodeAnalysis;

namespace Voltstro.CommandLineParser
{
	/// <summary>
	///     An <see cref="Attribute" /> for making a static field be set when the supplied arguments is provided
	/// </summary>
	[AttributeUsage(AttributeTargets.Field)]
	public class CommandLineArgumentAttribute : Attribute
	{
		/// <summary>
		///     What argument this is using
		/// </summary>
		public readonly string Name;

		/// <summary>
		///     Set a field to be set by the command line parser if the argument is supplied
		/// </summary>
		/// <param name="name"></param>
		public CommandLineArgumentAttribute([NotNull] string name)
		{
			if (string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException(nameof(name));

			Name = name;
		}
	}
}
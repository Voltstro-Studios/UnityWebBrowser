using System.Globalization;

namespace Voltstro.CommandLineParser.TypeReaders
{
	/// <summary>
	///     A default reader for <see cref="int" />
	/// </summary>
	public sealed class IntReader : ITypeReader
	{
		public object ReadType(string input)
		{
			if (string.IsNullOrWhiteSpace(input))
				return 0;

			return int.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out int result) ? result : 0;
		}
	}
}
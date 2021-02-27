using System.Globalization;

namespace Voltstro.CommandLineParser.TypeReaders
{
	/// <summary>
	///     A default reader for <see cref="bool" />
	/// </summary>
	public sealed class BoolReader : ITypeReader
	{
		public object ReadType(string input)
		{
			if (string.IsNullOrWhiteSpace(input))
				return true;

			//Check to see if it is just 'true' or 'false'
			input = input.ToLower();
			if (bool.TryParse(input, out bool result)) return result;

			//See if it is just '1' or '0'
			if (!int.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out int intResult)) return false;

			return intResult == 1;
		}
	}
}
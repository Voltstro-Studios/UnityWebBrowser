namespace Voltstro.CommandLineParser.TypeReaders
{
	/// <summary>
	///     A default reader for <see cref="byte" />
	/// </summary>
	public sealed class ByteReader : ITypeReader
	{
		public object ReadType(string input)
		{
			if (string.IsNullOrWhiteSpace(input))
				return (byte)0;

			return byte.TryParse(input, out byte result) ? result : (byte)0;
		}
	}
}
namespace Voltstro.CommandLineParser.TypeReaders
{
	/// <summary>
	///     The interface for a type reader
	/// </summary>
	public interface ITypeReader
	{
		/// <summary>
		///     Read the type and return it
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		object ReadType(string input);
	}
}
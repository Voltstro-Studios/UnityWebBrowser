namespace CefBrowserProcess.CommandLine.TypeReader
{
	public class BoolReader : ITypeReader
	{
		public object ReadType(string input)
		{
			return bool.TryParse(input, out bool result) && result;
		}
	}
}
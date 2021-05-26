using System.Collections.Generic;

namespace UnityWebBrowser
{
    internal class WebBrowserArgsBuilder
    {
        private readonly List<string> arguments;
        
        internal WebBrowserArgsBuilder()
        {
            arguments = new List<string>();
        }

        public void AppendArgument(string arg, object parm = null, bool quotes = false)
        {
            string builtArg = $"-{arg}";
            if(string.IsNullOrEmpty(parm.ToString())) return;
            
            //We got spaces
            if (quotes)
                builtArg += $" \"{parm}\"";
            else
                builtArg += $" {parm}";
            
            arguments.Add(builtArg);
        }
        
        public void AppendCefArgument(string arg, object parm = null, bool quotes = false)
        {
            string builtArg = $"-{arg}";
            if (string.IsNullOrEmpty(parm.ToString())) return;
            
            //We got spaces
            if (quotes)
                builtArg += $"=\"{parm}\"";
            else
                builtArg += $"={parm}";
            
            arguments.Add(builtArg);
        }

        public override string ToString()
        {
            return string.Join(" ", arguments);
        }
    }
}
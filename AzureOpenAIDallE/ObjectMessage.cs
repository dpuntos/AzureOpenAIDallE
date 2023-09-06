using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureOpenAIDallE
{
    internal class ObjectMessage
    {
        public string Prompt { get; }
        public int N { get; }
        public string Size { get; }

        public ObjectMessage(string prompt, int n, string size)
        {
            Prompt = prompt;
            N = n;
            Size = size;
        }
    }
}

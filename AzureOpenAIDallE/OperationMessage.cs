using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureOpenAIDallE
{
    internal class OperationMessage
    {
        public string Id { get; }
        public string Status { get; }

        public OperationMessage(string id, string status)
        {
            Id = id;
            Status = status;
        }
    }
}

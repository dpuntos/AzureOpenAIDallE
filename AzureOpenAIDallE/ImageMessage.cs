using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureOpenAIDallE
{
    internal class ImageMessage
    {
        public int created { get; set; }
        public int expires { get; set; }
        public string id { get; set; }
        public Result result { get; set; }
        public string status { get; set; }
    }

    internal class Datum
    {
        public string url { get; set; }
    }

    internal class Result
    {
        public int created { get; set; }
        public List<Datum> data { get; set; }
    }
}

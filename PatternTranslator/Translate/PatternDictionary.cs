using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PatternTranslator
{
    public class PatternDictionary : ICloneable
    {
        public bool Enable { get; set; }
        public bool Regex { get; set; }
        public bool IgnoreCase { get; set; }
        public int Priority { get; set; }

        public Dictionary<string, string> Entries { get; set; }

        //[JsonIgnore]
        //public Dictionary<string, Regex> RegexKeys { get; set; }
        [JsonIgnore]
        public string Path { get; set; }

        public object Clone()
        {
            var dict = new PatternDictionary()
            {
                Enable = Enable,
                Regex = Regex,
                IgnoreCase = IgnoreCase,
                Priority = Priority,
                Path = Path,

                Entries = Entries,
            };
            return dict;
        }
    }
}

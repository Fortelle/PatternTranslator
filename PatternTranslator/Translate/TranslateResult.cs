using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternTranslator
{
    public class TranslateResult
    {
        public bool Success { get; set; }
        public string Output { get; set; }
        public int PatternUsedCount { get; set; }
        public int PatternReplaceTimes { get; set; }
        public TimeSpan TotalTimes { get; set; }
    }
}

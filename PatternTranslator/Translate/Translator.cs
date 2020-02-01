using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PatternTranslator
{
    public class Translator
    {
        public IEnumerable<PatternDictionary> Dictionaries { get; set; }
        public string Source { get; set; }

        public Translator(List<PatternDictionary> dictionaries)
        {
            Dictionaries = dictionaries
                .Where(x => x.Enable)
                .Where(x => x.Entries?.Count() > 0)
                .OrderBy(x => x.Priority)
                ;
        }

        public TranslateResult Translate(string source)
        {
            var beginTime = DateTime.Now;
            var output = source;
            var success = false;
            var patternCount = 0;
            var patternMatched = 0;

            foreach (var dict in Dictionaries)
            {
                if (dict.Regex)
                {
                    foreach (var entry in dict.Entries)
                    {
                        output = Regex.Replace(output, entry.Key, entry.Value, RegexOptions.IgnoreCase);
                    }
                }
                else if (dict.IgnoreCase)
                {
                    foreach (var entry in dict.Entries)
                    {
                        output = Regex.Replace(output, entry.Key, entry.Value, RegexOptions.IgnoreCase);
                    }
                }
                else
                {
                    foreach (var entry in dict.Entries)
                    {
                        output = output.Replace(entry.Key, entry.Value);
                    }
                }
            }

            return new TranslateResult()
            {
                 Success = Source == output,
                 Output = output,
                 TotalTimes = DateTime.Now - beginTime,
                 PatternUsedCount = patternCount ,
                 PatternReplaceTimes = patternMatched,
            };
        }

        public TranslateTestResult[] Test(string source)
        {
            var output = source;
            var list = new List<TranslateTestResult>();

            foreach (var dict in Dictionaries.Where(x => x.Regex))
            {
                foreach (var entry in dict.Entries)
                {
                    var count = 0;
                    var begin = DateTime.Now;

                    output = Regex.Replace(output, entry.Key, (m) => {
                        count++;
                        return entry.Value;
                        });
                    var time = DateTime.Now - begin;
                    list.Add(new TranslateTestResult(){
                        Key = entry.Key,
                        Time = time,
                        Count = count
                    });
                }
            }

            return list.ToArray();
        }
    }
}

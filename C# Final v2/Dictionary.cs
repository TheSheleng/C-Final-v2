using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace C__Final_v2
{
    class LanguageDictionary
    {
        public SortedDictionary<string, string[]> Translations { get; set; } = new SortedDictionary<string, string[]>();
        public void Load(string path)
        {
            using (var reader = new StreamReader(path))
            {
                string? line = reader.ReadToEnd();
                var matches = new Regex(@"(.*):(.*)").Matches(line);
                foreach (Match match in matches)
                {
                    Translations.Add(
                        match.Groups[1].Value, 
                        match.Groups[2].Value.Trim().Split(
                            new char[] { ';' }, 
                            StringSplitOptions.RemoveEmptyEntries
                        )
                    );
                    Array.Sort(Translations[match.Groups[1].Value]);
                }
            }
        }
        public void Save(string path)
        {
            using (var writer = new StreamWriter(path, false))
            {
                foreach (var word in Translations)
                {
                    string line = $"{word.Key}:";
                    foreach (var translation in word.Value)
                    {
                        line += $"{translation};";
                    }
                    writer.WriteLine(line);
                }
            }
        }
        public void Export(string key, string path)
        {
            using (StreamWriter writer = new StreamWriter(path, true))
            {
                writer.WriteLine($"{key}:{String.Join(";", Translations[key])}");
            }
        }
    }
}

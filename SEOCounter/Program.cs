using CsvHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace SEOCounter
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello SEO sort!");

            var records = LoadCSV("modules.csv");

            var results = ParseSEOText(records);

            //output results to the console for validation
            foreach(var seo in results)
            {
                Console.WriteLine($"{seo.Key}: {seo.Value}");
            }

            //write the results to a new CSV file
            SaveCSV(results, "SEOCount.csv");
        }

        private static void SaveCSV(Dictionary<string, int> entries, string csvName )
        {
            using (var writer = new StreamWriter(csvName))
            {
                using (var csv = new CsvWriter(writer))
                {
                    csv.WriteRecords(entries);
                }
            }
        }

        private static Dictionary<string, int> ParseSEOText(List<SEOEntry> records)
        {
            //create an empty dictionary to hold our results, the key will be the SEO term, the value will be the count
            var seoTerms = new SortedDictionary<string, int>();

            foreach (var record in records)
            {
                //split the seo terms into an array of strings
                var terms = record.SEO.Split(",", StringSplitOptions.RemoveEmptyEntries);

                //loop over every term in the array of terms
                foreach (var term in terms)
                {
                    //remove leading and training spaces ... i.e. if there was a space after the comma we don't want it
                    var cleanTerm = term.Trim();

                    //is the term the dictionary already? then increase value (used as a count)
                    if (seoTerms.ContainsKey(cleanTerm))
                    {
                        seoTerms[cleanTerm]++;
                    }
                    else //otherwise, create a new enry in the dictionary with a count of 1
                    {
                        seoTerms.Add(cleanTerm, 1);
                    }
                }
            }

            return new Dictionary<string, int>(seoTerms);
        }

        //Load file from an embedded resource and export a list of populated SEOEntry objects 
        private static List<SEOEntry> LoadCSV(string resName)
        {
            var assembly = Assembly.GetExecutingAssembly();

            var resourceName = $"SEOCounter.{resName}";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                using (var reader = new StreamReader(stream))
                {
                    var csvReader = new CsvReader(reader);
                    var records = csvReader.GetRecords<SEOEntry>().ToList();

                    return records;
                }
            }
        }
    }
}
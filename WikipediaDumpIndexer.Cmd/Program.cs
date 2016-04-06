using System;
using System.Diagnostics;
using WikipediaDumpIndexer.Core;

namespace WikipediaDumpIndexer.Cmd
{
    class Program
    {
        static void Main()
        {
            OnParse();

            Console.ReadLine();
        }

        private static async void OnParse()
        {
            DateTime startTime = DateTime.Now;
            Stopwatch stopWatch = Stopwatch.StartNew();

            Console.WriteLine("Opening Wikipedia stream...");

            // var uri = "https://dumps.wikimedia.org/enwiki/latest/enwiki-latest-abstract.xml";
            // var uri = "https://dumps.wikimedia.org/dewiki/20160305/dewiki-20160305-pages-meta-current2.xml-p000425451p001877043.bz2";
            // var uri = @"enwiki-latest-pages-articles.xml.bz2";
            // var uri = @"dewiki-20160305-pages-articles1.xml-p000000001p000425449.bz2";

            Console.WriteLine("Parsing & Indexing...");
            try
            {
                await WikiPediaService.ParseAsync(@"C:\Projects\GitHub\WikipediaDumpIndexer\Storages\dewiki-20160305-pages-articles1.xml-p000000001p000425449.bz2");

                Console.WriteLine("Done... Started: {0} Duration: {1}", startTime, stopWatch.Elapsed);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
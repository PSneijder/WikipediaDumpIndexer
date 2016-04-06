using Ninject;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using WikipediaDumpIndexer.Core.Lucene;
using WikipediaDumpIndexer.Core.Wiki.Parsers;

namespace WikipediaDumpIndexer.Core
{
    public static class WikiPediaService
    {
        private static readonly StandardKernel Kernel;

        static WikiPediaService()
        {
            var kernel = new StandardKernel();
            kernel.Load(new WikiPediaModule());

            Kernel = kernel;
        }

        public static Task ParseAsync()
        {
            var fileName = ConfigurationManager.AppSettings["StreamUrl"];

            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new SettingsPropertyNotFoundException("No setting for StreamUrl found.");
            }

            return ParseAsync(fileName);
        }

        public static Task ParseAsync(string fileName)
        {
            return Task.Factory.StartNew(() =>
                {
                    using (var parser = Kernel.Get<WikiXmlParser>())
                    {
                        parser.Parse(new Uri(fileName));
                    }
                });
        }

        public static IEnumerable<Tuple<string, float>> Search(string searchText)
        {
            var items = new List<Tuple<string, float>>();

            using (var searcher = Kernel.Get<Searcher>())
            {
                var results = searcher.Search(searchText);

                items.AddRange(from result in results
                                    let flattend = string.Join(" ", result.Item2.GetFields().Select(field => field.StringValue))
                                        select Tuple.Create(flattend, result.Item1));
            }

            return items;
        }
    }
}
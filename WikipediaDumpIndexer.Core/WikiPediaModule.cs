using System;
using Ninject.Modules;
using WikipediaDumpIndexer.Core.Lucene;
using WikipediaDumpIndexer.Core.Wiki.Parsers;
using System.Configuration;

namespace WikipediaDumpIndexer.Core
{
    public sealed class WikiPediaModule
        : NinjectModule
    {
        public override void Load()
        {
            Bind<WikiParserBase>().To<WikiXmlParser>().InSingletonScope();

            Bind<Indexer>().ToSelf().InSingletonScope()
                .WithConstructorArgument("path", ReadFromConfiguration("StoragePathRead", "IndexStorage"))
                    .WithConstructorArgument("recreateIfExists", true);

            Bind<Searcher>().ToSelf().InSingletonScope()
                .WithConstructorArgument("path", ReadFromConfiguration("StoragePathWrite", "IndexStorage"))
                    .WithConstructorArgument("recreateIfExists", false);
        }

        private string ReadFromConfiguration(string name, string fallback)
        {
            return ConfigurationManager.AppSettings[name] ?? fallback;
        }
    }
}
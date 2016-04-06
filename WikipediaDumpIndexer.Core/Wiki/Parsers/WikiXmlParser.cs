using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks.Dataflow;
using System.Xml;
using WikipediaDumpIndexer.Core.Lucene;
using WikipediaDumpIndexer.Core.Wiki.Handlers;

namespace WikipediaDumpIndexer.Core.Wiki.Parsers
{
    internal sealed class WikiXmlParser
       : WikiParserBase
    {
        private readonly Indexer _indexer;

        public WikiXmlParser(Indexer indexer)
        {
            _indexer = indexer;
        }

        public override void Parse()
        {
            FileTypeHandlerFactory.Handle(FileName, ProduceAndConsume);
        }

        private void ProduceAndConsume(Stream stream)
        {
            long counter = 0;
            Stopwatch stopWatch = Stopwatch.StartNew();

            using (Indexer indexer = _indexer)
                using (XmlReader reader = XmlReader.Create(stream))
                    using (DataFlow dataflow = new DataFlow(reader))
                    {
                        reader.MoveToContent();

                        var buffer = new BufferBlock<object>();
                        var consumer = dataflow.ConsumeAsync(buffer, (page) =>
                        {
                            indexer.Index(page);

                            if(counter % 1000 == 0)
                            {
                                Console.WriteLine("{0} documents in {1} minutes {2} seconds", counter, stopWatch.Elapsed.Minutes, stopWatch.Elapsed.Seconds);
                                stopWatch = Stopwatch.StartNew();
                            }

                            counter++;
                        });

                        dataflow.Produce(buffer);

                        consumer.Wait();
                    }
        }
    }
}
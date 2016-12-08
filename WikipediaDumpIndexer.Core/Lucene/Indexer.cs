using Lucene.Net.Analysis;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Store;
using System.Collections.Generic;
using System;
using System.Configuration;

namespace WikipediaDumpIndexer.Core.Lucene
{
    sealed class Indexer
        : IIndexer
    {
        private long _counter;
        private long _threshold;

        private Document _document;
        private IndexWriter _writer;
        
        public Indexer(string path, bool recreateIfExists = false)
        {
            Initialize(FSDirectory.Open(path), new SimpleAnalyzer(), recreateIfExists);
        }

        public Indexer(FSDirectory indexDirectory, Analyzer analyzer, bool recreateIfExists)
        {
            Initialize(indexDirectory, analyzer, recreateIfExists);
        }

        private void Initialize(FSDirectory indexDirectory, Analyzer analyzer, bool recreateIfExists)
        {
            _writer = new IndexWriter(indexDirectory, analyzer, recreateIfExists, new IndexWriter.MaxFieldLength(IndexWriter.DEFAULT_MAX_FIELD_LENGTH));

            LoadAndSetConfiguration(_writer);

            _document = new Document();
            Field id = new Field("id", "", Field.Store.YES, Field.Index.NOT_ANALYZED);
            _document.Add(id);
            Field title = new Field("title", "", Field.Store.YES, Field.Index.ANALYZED);
            _document.Add(title);
            Field revision = new Field("revision", "", Field.Store.YES, Field.Index.ANALYZED);
            _document.Add(revision);

            _counter = 0;
            _threshold = 50000;
        }

        private void LoadAndSetConfiguration(IndexWriter writer)
        {
            int size = 1024;
            int.TryParse(ConfigurationManager.AppSettings["RAMBufferSizeMB"], out size);
            int mergeFactor = 40;
            int.TryParse(ConfigurationManager.AppSettings["MergeFactor"], out mergeFactor);
            int termIndexInterval = 1024;
            int.TryParse(ConfigurationManager.AppSettings["TermIndexInterval"], out termIndexInterval);
            bool useCompoundFile = false;
            bool.TryParse(ConfigurationManager.AppSettings["UseCompoundFile"], out useCompoundFile);

            writer.SetRAMBufferSizeMB(size);
            writer.MergeFactor = mergeFactor;
            writer.SetMergePolicy(new LogDocMergePolicy(writer));
            writer.SetMergeScheduler(new ConcurrentMergeScheduler());
            writer.TermIndexInterval = termIndexInterval;
            writer.UseCompoundFile = useCompoundFile;
        }

        public void Index(dynamic page)
        {
            _document.GetField("id").SetValue(page.id);
            _document.GetField("title").SetValue(page.title);
            _document.GetField("revision").SetValue(page.revision.text);

            _writer.AddDocument(_document);
            if(_counter++ >= _threshold)
            {
                _writer.Flush(false, false, false);
                _counter = 0;
            }
        }

        public void Dispose()
        {
            _writer.Flush(false, false, false);
            _writer.Dispose();
        }

        public void Index(IEnumerable<Field> fields)
        {

        }
    }
}
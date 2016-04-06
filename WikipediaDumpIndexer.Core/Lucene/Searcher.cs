using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using System;
using System.Collections.Generic;
using System.IO;
using Lucene.Net.Search.Highlight;

namespace WikipediaDumpIndexer.Core.Lucene
{
    internal sealed class Searcher
            : IDisposable
    {
        private const bool ReadonlyMode = true;
        private readonly Analyzer _analyzer = new StandardAnalyzer(Constants.Version);

        private FSDirectory _indexDirectory;
        private readonly bool _recreateIfExists;
        private readonly string _path;

        public Searcher(string path, bool recreateIfExists = false)
        {
            _path = path;
            _recreateIfExists = recreateIfExists;
        }

        private FSDirectory Directory
        {
            get {
                return _indexDirectory ??
                       (_indexDirectory = FSDirectory.Open(new DirectoryInfo(_path), new NoLockFactory()));
            }
        }

        public long Count()
        {
            using (var reader = IndexReader.Open(Directory, ReadonlyMode))
            {
                return reader.NumDocs();
            }
        }

        public Indexer GetIndexer()
        {
            return new Indexer(Directory, _analyzer, _recreateIfExists);
        }

        public IEnumerable<Tuple<float, Document>> Search(string text, string defaultField = "title", int maxResultCount = 500)
        {
            var parser = new QueryParser(Constants.Version, defaultField, _analyzer);
            var query = parser.Parse(text);

            Directory.EnsureOpen();

            using (var searcher = new IndexSearcher(IndexReader.Open(Directory, ReadonlyMode)))
            {
                var hits = searcher.Search(query, maxResultCount);

                foreach (var scoreDoc in hits.ScoreDocs)
                {
                    var doc = searcher.Doc(scoreDoc.Doc);
                    yield return new Tuple<float, Document>(scoreDoc.Score, doc);
                }
            }
        }

        public void Close()
        {
            _indexDirectory.Dispose();
        }

        public void Dispose()
        {
            if (_indexDirectory != null)
            { 
                _indexDirectory.Dispose();
            }
        }
    }
}
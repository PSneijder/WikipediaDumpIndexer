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
    sealed class Searcher
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

        public IEnumerable<Tuple<float, Document, string[]>> Search(string text, string defaultField = "title", int maxResultCount = 500)
        {
            var parser = new QueryParser(Constants.Version, defaultField, _analyzer);
            Query query = parser.Parse(text ?? string.Empty);

            var formatter = new SimpleHTMLFormatter(string.Empty, string.Empty);
            var fragmenter = new SimpleFragmenter(120);
            var scorer = new QueryScorer(query);
            var highlighter = new Highlighter(formatter, scorer) { TextFragmenter = fragmenter };

            using (var directory = FSDirectory.Open(new DirectoryInfo(_path), new NoLockFactory()))
            {
                using (var searcher = new IndexSearcher(IndexReader.Open(directory, ReadonlyMode)))
                {
                    TopDocs hits = searcher.Search(query, maxResultCount);

                    foreach (var scoreDoc in hits.ScoreDocs)
                    {
                        Document doc = searcher.Doc(scoreDoc.Doc);

                        var field = doc.Get(defaultField);
                        var tokenStream = _analyzer.TokenStream(defaultField, new StringReader(field));
                        var framgents = highlighter.GetBestFragments(tokenStream, field, 5);

                        yield return new Tuple<float, Document, string[]>(scoreDoc.Score, doc, framgents);
                    }
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
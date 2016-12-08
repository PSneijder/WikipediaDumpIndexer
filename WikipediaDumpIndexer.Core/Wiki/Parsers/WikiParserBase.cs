using System;

namespace WikipediaDumpIndexer.Core.Wiki.Parsers
{
    abstract class WikiParserBase
        : IDisposable
    {
        protected Uri FileName = null;

        protected WikiParserBase()
        {

        }

        protected WikiParserBase(Uri fileName)
        {
            FileName = fileName;
        }

        public void Parse(Uri fileName)
        {
            FileName = fileName;
            Parse();
        }

        public abstract void Parse();

        public void Dispose()
        {

        }
    }
}
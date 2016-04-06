using Lucene.Net.Documents;
using System;
using System.Collections.Generic;

namespace WikipediaDumpIndexer.Core.Lucene
{
    public interface IIndexer 
        : IDisposable
    {
        void Index(IEnumerable<Field> fields);
    }
}
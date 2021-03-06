﻿using Lucene.Net.Store;

namespace WikipediaDumpIndexer.Core.Extensions
{
    static class FsDirectoryExtensions
    {
        public static void ToFileStream(this RAMDirectory directory, string path)
        {
            Directory.Copy(directory, FSDirectory.Open(path), false);
        }
    }
}
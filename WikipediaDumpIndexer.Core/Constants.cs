﻿using System.ComponentModel;
using Lucene.Net.Util;

namespace WikipediaDumpIndexer.Core
{
    enum FileExtensions
    {
        [Description(".XML")]
        Xml = 0,
        [Description(".BZ")]
        Bz = 1,
        [Description(".BZ2")]
        Bz2 = 2
    }

    static class Constants
    {
        internal static int DocumentThreshold = 1000;
        internal static Version Version = Version.LUCENE_30;
    }
}
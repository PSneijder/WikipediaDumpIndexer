using System;
using System.IO;
using System.Net;

namespace WikipediaDumpIndexer.Core.Wiki.Handlers
{
    static class FileTypeHandlerFactory
    {
        public static void Handle(Uri fileName, Action<Stream> parse)
        {
            if (fileName.IsFile && fileName.AbsoluteUri.EndsWith(".bz") || fileName.IsFile && fileName.AbsoluteUri.EndsWith(".bz2"))
            {
                using (FileStream fileStream = new FileStream(fileName.OriginalString, FileMode.Open))
                {
                    var inputStream = new Bzip2.BZip2InputStream(fileStream, false);
                    parse(inputStream);
                }
            }
            else if (fileName.IsFile && fileName.AbsoluteUri.EndsWith(".xml"))
            {
                using (FileStream fileStream = new FileStream(fileName.OriginalString, FileMode.Open))
                {
                    parse(fileStream);
                }
            }
            else
            {
                WebRequest request = WebRequest.Create(fileName);
                Stream stream = request.GetResponse().GetResponseStream();
                
                if (stream != null)
                {
                    using (StreamReader streamReader = new StreamReader(stream, true))
                    {
                        if (fileName.AbsoluteUri.EndsWith(".bz") || fileName.AbsoluteUri.EndsWith(".bz2"))
                        {
                            var inputStream = new Bzip2.BZip2InputStream(streamReader.BaseStream, false);
                            parse(inputStream);
                        }
                        else if (fileName.AbsoluteUri.EndsWith(".xml"))
                        {
                            parse(streamReader.BaseStream);
                        }
                    }
                }
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Xml;

namespace WikipediaDumpIndexer.Core.Wiki.Handlers
{
    internal sealed class DataFlow
        : IDisposable
    {
        private readonly XmlReader _reader;

        public DataFlow(XmlReader reader)
        {
            _reader = reader;
        }

        public void Produce(ITargetBlock<object> target)
        {
            while(_reader.Read())
            {
                if (_reader.IsStartElement("page"))
                {
                    object page = ReadElement(_reader);

                    target.Post(page);
                }
            }

            target.Complete();
        }

        public async Task<dynamic> ConsumeAsync(ISourceBlock<object> source, Action<object> action)
        {
            while (await source.OutputAvailableAsync())
            {
                object data = source.Receive();

                action(data);
            }

            return null;
        }

        public void Dispose()
        {

        }

        private IEnumerable<object> Read(XmlReader reader)
        {
            object page = ReadElement(reader);

            yield return page;
        }

        private object ReadElement(XmlReader reader)
        {
            var name = reader.Name;
            IDictionary<string, object> result = new ExpandoObject();

            while (reader.Read())
            {
                if (reader.IsEmptyElement)
                    continue;
                if (reader.NodeType == XmlNodeType.EndElement && reader.Name == name)
                    return result;
                if (reader.NodeType == XmlNodeType.EndElement)
                    continue;
                if (reader.NodeType == XmlNodeType.Text)
                    return reader.Value;
                if (reader.Name != "")
                    result[reader.Name] = ReadElement(reader);
            }

            throw new Exception();
        }
    }
}
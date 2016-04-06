using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Microsoft.Build.Framework;

namespace CopyDependencies
{
    class Program
    {
        static void Main()
        {
            var project = @"C:\Projects\GitHub\WikipediaDumpIndexer\WikipediaDumpIndexer.Desktop\WikipediaDumpIndexer.Desktop.csproj";
            var outputPath = @"bin\Debug\";

            ScanDependencies(project, outputPath);

            Console.ReadLine();
        }

        static void ScanDependencies(string Project, string OutputPath)
        {
            var outputMap = new HashSet<string>();
            var referenceMap = new HashSet<string>();
            
            var queue = new Queue<string>();
            queue.Enqueue(Project);

            while (queue.Count > 0)
            {
                var project = queue.Dequeue();
                Log.LogMessage(MessageImportance.High, project);

                if (!project.EndsWith(".csproj"))
                    continue;
                
                var doc = new XmlDocument();
                doc.Load(project);

                var nsmgr = new XmlNamespaceManager(doc.NameTable);
                nsmgr.AddNamespace("msb", "http://schemas.microsoft.com/developer/msbuild/2003");

                var projectDirectory = Path.GetDirectoryName(project);

                var projectReferences = doc
                    .SelectNodes("/msb:Project/msb:ItemGroup/msb:ProjectReference/@Include", nsmgr)
                    .Cast<XmlAttribute>().Select(a => Path.GetFullPath(projectDirectory + "\\" + a.Value.Trim()));

                var references = doc
                    .SelectNodes("/msb:Project/msb:ItemGroup/msb:Reference/msb:HintPath", nsmgr)
                    .Cast<XmlElement>().Select(a => Path.GetFullPath(projectDirectory + "\\" + a.InnerText.Trim()));

                foreach (var projectReference in projectReferences)
                {
                    if (!referenceMap.Contains(projectReference))
                    {
                        referenceMap.Add(projectReference);

                        queue.Enqueue(projectReference);
                    }

                    var referenceDoc = new XmlDocument();
                    referenceDoc.Load(projectReference);

                    var referenceNsmgr = new XmlNamespaceManager(doc.NameTable);
                    referenceNsmgr.AddNamespace("msb", "http://schemas.microsoft.com/developer/msbuild/2003");

                    var assemblyDirectory = Path.GetDirectoryName(projectReference);
                    var assemblyName = referenceDoc.SelectSingleNode("/msb:Project/msb:PropertyGroup/msb:AssemblyName", nsmgr).InnerText.Trim() + ".dll";
                    var assembly = Path.GetFullPath(Path.Combine(assemblyDirectory, OutputPath, assemblyName));

                    // Add to output list
                    if (!outputMap.Contains(assembly))
                    {
                        Log.LogMessage(MessageImportance.High, "Adding to ouput ->" + assembly);
                        outputMap.Add(assembly);
                    }
                }
                
                foreach (var reference in references)
                {
                    // Add to output list
                    if (!outputMap.Contains(reference))
                    {
                        Log.LogMessage(MessageImportance.High, "Adding to ouput ->" + reference);
                        outputMap.Add(reference);
                    }
                }
            }

            Console.WriteLine(referenceMap.Count);
        }
    }
}
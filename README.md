# WikipediaDumpIndexer

![](https://img.shields.io/badge/.net-v4.5.2-blue.svg)
![](https://img.shields.io/badge/build-passing-green.svg)

![alt tag](https://github.com/PSneijder/WikipediaDumpIndexer/blob/master/Assets/WikipediaDumpIndexer.png)

The project uses the German Wikipedia as source of documents for several purposes: as training data and as source of data to be annotated.
The Wikipedia maintainers provide, each month, an XML, BZ, BZ2 dump of all documents in the database: it consists of a single XML file containing the whole encyclopedia, that can be used for various kinds of analysis, such as statistics, service lists, etc.

Wikipedia dumps are available from [Wikipedia database download](http://dumps.wikimedia.org/). 
The WikipediaDumpIndexer tool generates plain text from a Wikipedia database dump, discarding any other information or annotation present in Wikipedia pages, such as images, tables, references and lists.

![alt tag](https://github.com/PSneijder/WikipediaDumpIndexer/blob/master/Assets/CodeMap.png)

## TODOs
* Indexing wikipedia contents in a more meaningful manner.
* <strike>xml parsing of wikipedia dumps</strike>
* <strike>bz parsing of wikipedia dumps</strike>
* <strike>bz2 parsing of wikipedia dumps</strike>

## Recent Changes
See [CHANGES.txt](CHANGES.txt)

## Committers
* [Philip Schneider](https://github.com/PSneijder)

## Licensing
The license for the code is [ALv2](http://www.apache.org/licenses/LICENSE-2.0.html).

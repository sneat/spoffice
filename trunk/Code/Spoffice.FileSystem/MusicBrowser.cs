using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lucene.Net;
using Lucene.Net.Store;
using Lucene.Net.Index;
using Lucene.Net.Analysis.Standard;

namespace Spoffice.FileSystem
{
    public class MusicBrowser
    {
        public MusicBrowser()
        {
            /*
            string indexDirectory = "";
            if (!IndexReader.IndexExists(indexPath))
            {
                string indexFileLocation = "C:\\Index";
                Lucene.Net.Store.Directory dir =
                    Lucene.Net.Store.FSDirectory.GetDirectory(indexFileLocation, true);

                //create an analyzer to process the text
                Lucene.Net.Analysis.Analyzer analyzer = new
                Lucene.Net.Analysis.Standard.StandardAnalyzer();

                //create the index writer with the directory and analyzer defined.
                Lucene.Net.Index.IndexWriter indexWriter = new
                Lucene.Net.Index.IndexWriter(dir, analyzer,
                   true);

                //create a document, add in a single field
                Lucene.Net.Documents.Document doc = new
                Lucene.Net.Documents.Document();

                Lucene.Net.Documents.Field fldContent =
                  new Lucene.Net.Documents.Field("content",
                  "The quick brown fox jumps over the lazy dog",
                  Lucene.Net.Documents.Field.Store.YES,


                Lucene.Net.Documents.Field.Index.TOKENIZED,
                Lucene.Net.Documents.Field.TermVector.YES);

                doc.Add(fldContent);

                //write the document to the index
                indexWriter.AddDocument(doc);

                //optimize and close the writer
                indexWriter.Optimize();
                indexWriter.Close();
            }*/
        }
    }
}

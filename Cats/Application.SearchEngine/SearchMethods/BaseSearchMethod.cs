using System;
using System.IO;
using System.Linq;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers.Classic;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.Util;

namespace Application.SearchEngine.SearchMethods
{
    public abstract class BaseSearchMethod
    {
        private readonly string _luceneDirectory;
        private FSDirectory _directoryTemp;
        protected const int HitsLimits = 1000;

        protected BaseSearchMethod(string luceneDirectory)
        {
            if (!System.IO.Directory.Exists(luceneDirectory))
                System.IO.Directory.CreateDirectory(luceneDirectory);
            _luceneDirectory = luceneDirectory;
        }

        public bool IsIndexExist()
        {
            return Directory.Directory.GetFiles().FirstOrDefault() != null;
        }

        protected FSDirectory Directory
        {
            get
            {
                if (_directoryTemp == null)
                    _directoryTemp = FSDirectory.Open(new DirectoryInfo(_luceneDirectory));
                if (IndexWriter.IsLocked(_directoryTemp))
                    IndexWriter.Unlock(_directoryTemp);
                var lockFilePath = Path.Combine(_luceneDirectory, "write.lock");
                if (File.Exists(lockFilePath))
                    File.Delete(lockFilePath);
                return _directoryTemp;
            }
        }        

        public void DeleteIndex(int id)
        {
            using var analyzer = new StandardAnalyzer(LuceneVersion.LUCENE_48);
            using var writer = new IndexWriter(Directory, new IndexWriterConfig(LuceneVersion.LUCENE_48, analyzer));/*Directory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED*/
            var searchQuery = new TermQuery(new Term(SearchingFields.Id.ToString(), id.ToString()));
            writer.DeleteDocuments(searchQuery);
        }

        public bool DeleteIndex()
        {
            try
            {
                using var analyzer = new StandardAnalyzer(LuceneVersion.LUCENE_48);
                using var writer = new IndexWriter(Directory, new IndexWriterConfig(LuceneVersion.LUCENE_48, analyzer));
                writer.DeleteAll();
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public void Optimize()
        {
            using var analyzer = new StandardAnalyzer(LuceneVersion.LUCENE_48);
            using var writer = new IndexWriter(Directory, new IndexWriterConfig(LuceneVersion.LUCENE_48, analyzer));
            //writer.Optimize(); todo #
        }

        protected Query ParseQuery(string searchQuery, QueryParser parser)
        {
            Query query;
            try
            {
                query = parser.Parse(searchQuery.Trim());
            }
            catch (ParseException)
            {
                query = parser.Parse(QueryParserBase.Escape(searchQuery.Trim()));
            }
            return query;
        }
    }

    enum SearchingFields
    {
        Id,
        FirstName,
        MiddleName,
        LastName,
        Group,
        Name
    }
}

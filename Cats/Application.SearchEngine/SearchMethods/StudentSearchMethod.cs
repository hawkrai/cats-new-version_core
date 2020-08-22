using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LMP.Models;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers.Classic;
using Lucene.Net.Search;
using Lucene.Net.Util;

namespace Application.SearchEngine.SearchMethods
{
    public class StudentSearchMethod : BaseSearchMethod
    {
        private static readonly string LuceneDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "Lucene_indeces", "Student_indeces");                

        public StudentSearchMethod(): base(LuceneDirectory){}        

        private void AddToIndex(Student student, IndexWriter writer)
        {
            var searchQuery = new TermQuery(new Term(SearchingFields.Id.ToString(), student.Id.ToString()));
            writer.DeleteDocuments(searchQuery);

            var doc = new Document
            {
                // todo # replace obsolete
                new Field(SearchingFields.Id.ToString(), student.Id.ToString(), Field.Store.YES,
                    Field.Index.NOT_ANALYZED),
                new Field(SearchingFields.FirstName.ToString(), student.FirstName ?? "", Field.Store.YES,
                    Field.Index.ANALYZED),
                new Field(SearchingFields.MiddleName.ToString(), student.MiddleName ?? "", Field.Store.YES,
                    Field.Index.ANALYZED),
                new Field(SearchingFields.LastName.ToString(), student.LastName ?? "", Field.Store.YES,
                    Field.Index.ANALYZED),
                new Field(SearchingFields.Group.ToString(), student.Group != null ? student.Group.Name : "",
                    Field.Store.YES, Field.Index.ANALYZED),
                new Field(SearchingFields.Name.ToString(), student.User != null ? student.User.UserName : "",
                    Field.Store.YES, Field.Index.ANALYZED)
            };


            writer.AddDocument(doc);
        }

        public void UpdateIndex(Student student)
        {
            DeleteIndex(student.Id);
            AddToIndex(student);
        }

        public void AddToIndex(IEnumerable<Student> students)
        {
            using var analyzer = new StandardAnalyzer(LuceneVersion.LUCENE_48);
            using var writer = new IndexWriter(Directory, new IndexWriterConfig(LuceneVersion.LUCENE_48, analyzer));
            foreach (var student in students)
                AddToIndex(student, writer);
        }

        public void AddToIndex(Student student)
        {
            AddToIndex(new List<Student> {student});
        }

        private Student MapSearchResult(Document doc)
        {
            int.TryParse(doc.Get(SearchingFields.Group.ToString()), out var groupId);
            return new Student
            {
                Id = int.Parse(doc.Get(SearchingFields.Id.ToString())),
                FirstName = doc.Get(SearchingFields.FirstName.ToString()),
                MiddleName = doc.Get(SearchingFields.MiddleName.ToString()),
                LastName = doc.Get(SearchingFields.LastName.ToString()),
                Email = doc.Get(SearchingFields.Name.ToString()), //логин
                GroupId = groupId
            };
        }

        private IEnumerable<Student> MapSearchResults(IEnumerable<Document> documents)
        {
            return documents.Select(MapSearchResult).ToList();
        }

        private IEnumerable<Student> MapSearchResults(IEnumerable<ScoreDoc> scoreDocs, IndexSearcher searcher)
        {
            return scoreDocs.Select(scoreDoc => MapSearchResult(searcher.Doc(scoreDoc.Doc))).ToList();
        }

        private IEnumerable<Student> search(string searchQuery, string searchField = "")
        {
            if (string.IsNullOrEmpty(searchQuery.Replace("*", "").Replace("?", "")))
                return new List<Student>();

            var searcher = new IndexSearcher(new MultiReader()); //new IndexSearcher(Directory, false); todo #
            using var analyzer = new StandardAnalyzer(LuceneVersion.LUCENE_48);

            var parser = new MultiFieldQueryParser(LuceneVersion.LUCENE_48, new[]
            {
                SearchingFields.Id.ToString(),
                SearchingFields.FirstName.ToString(),
                SearchingFields.MiddleName.ToString(),
                SearchingFields.LastName.ToString(),
                SearchingFields.Group.ToString(),
                SearchingFields.Name.ToString()
            }, analyzer);

            var query = ParseQuery(searchQuery, parser);
            var docs = searcher.Search(query, null, HitsLimits, Sort.RELEVANCE).ScoreDocs;
            var results = MapSearchResults(docs, searcher);

            return results;
        }

        public IEnumerable<Student> Search(string searchText, string fieldName = "")
        {
            if (string.IsNullOrEmpty(searchText)) return new List<Student>();

            var terms = searchText.Trim().Replace("-", " ").Split(' ')
                .Where(term => !string.IsNullOrEmpty(term)).Select(term => term.Trim() + "*");
            searchText = string.Join(" ", terms);

            return search(searchText, fieldName);
        }        
    }    
}

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
    public class GroupSearchMethod : BaseSearchMethod
    {
        private static readonly string LuceneDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "Lucene_indeces", "Group_indeces");        
        
        public GroupSearchMethod(): base(LuceneDirectory){}

        private void AddToIndex(Group group, IndexWriter writer)
        {
            var searchQuery = new TermQuery(new Term(SearchingFields.Id.ToString(), group.Id.ToString()));
            writer.DeleteDocuments(searchQuery);

            var doc = new Document
            {
                // todo # replace obsolete
                new Field(SearchingFields.Id.ToString(), @group.Id.ToString(), Field.Store.YES,
                    Field.Index.NOT_ANALYZED),
                new Field(SearchingFields.Name.ToString(), @group.Name, Field.Store.YES, Field.Index.ANALYZED)
            };

            writer.AddDocument(doc);
        }

        public void UpdateIndex(Group group)
        {
            DeleteIndex(group.Id);
            AddToIndex(group);
        }

        public void AddToIndex(IEnumerable<Group> groups)
        {
            using var analyzer = new StandardAnalyzer(LuceneVersion.LUCENE_48);
            using var writer = new IndexWriter(Directory, new IndexWriterConfig(LuceneVersion.LUCENE_48, analyzer));
            foreach (var group in groups)
                AddToIndex(group, writer);
        }

        public void AddToIndex(Group group)
        {
            AddToIndex(new List<Group> {group});
        }

        private Group MapSearchResult(Document doc)
        {
            return new Group
            {
                Id = int.Parse(doc.Get(SearchingFields.Id.ToString())),
                Name = doc.Get(SearchingFields.Name.ToString())
            };
        }

        private IEnumerable<Group> MapSearchResults(IEnumerable<Document> documents)
        {
            return documents.Select(MapSearchResult).ToList();
        }

        public IEnumerable<Group> MapSearchResults(IEnumerable<ScoreDoc> scoreDocs, IndexSearcher searcher)
        {
            return scoreDocs.Select(scoreDoc => MapSearchResult(searcher.Doc(scoreDoc.Doc))).ToList();
        }

        private IEnumerable<Group> search(string searchQuery, string searchField = "")
        {
            if (string.IsNullOrEmpty(searchQuery.Replace("*", "").Replace("?", "")))
                return new List<Group>();

            var searcher = new IndexSearcher(new MultiReader()); //new IndexSearcher(Directory, false); todo #
            using var analyzer = new StandardAnalyzer(LuceneVersion.LUCENE_48);

            var parser = new MultiFieldQueryParser(LuceneVersion.LUCENE_48, new[]
            {
                SearchingFields.Id.ToString(),
                SearchingFields.Name.ToString()
            }, analyzer);
            var query = ParseQuery(searchQuery, parser);
            var docs = searcher.Search(query, null, HitsLimits, Sort.RELEVANCE).ScoreDocs;
            var results = MapSearchResults(docs, searcher);

            return results;
            
        }

        public IEnumerable<Group> Search(string searchText, string fieldName = "")
        {
            if (string.IsNullOrEmpty(searchText)) return new List<Group>();

            var terms = searchText.Trim().Replace("-", " ").Split(' ')
                .Where(term => !string.IsNullOrEmpty(term)).Select(term => term.Trim() + "*");
            searchText = string.Join(" ", terms);

            return search(searchText, fieldName);
        }
    }
}

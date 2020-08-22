using System.Collections.Generic;
using LMP.PlagiarismNet.XMLDocs;

namespace LMP.PlagiarismNet.Data
{
    public class SimilarityRow
    {
        public SimilarityRow()
        {
            Similarity = new Dictionary<Doc, int>();
        }

        public Doc Doc { get; set; }

        public Dictionary<Doc, int> Similarity { get; set; }

        public int GetSumSimilarity()
        {
            if (Similarity == null || Similarity.Count < 1)
                return 0;
            var total = 0;
            foreach (var sim in Similarity.Values) total += sim;

            return total;
        }

        public void RemoveSimilarityByIndex(List<Doc> docs)
        {
            foreach (var doc in docs) Similarity.Remove(doc);
        }
    }
}
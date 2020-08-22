using System.Collections.Generic;
using LMP.PlagiarismNet.Data;
using LMP.PlagiarismNet.XMLDocs;

namespace LMP.PlagiarismNet.Services.Interfaces
{
    public  interface ISimilarity
    {
         List<SimilarityRow> MakeSimilarityRows(List<Doc> paramList, int paramInt);
    }
}

using System.Collections.Generic;
using LMP.PlagiarismNet.Data;

namespace LMP.PlagiarismNet.Services.Interfaces
{
    public interface IGreedy
    {
        List<Cluster> Clustering(List<SimilarityRow> paramList, int paramInt);
    }
}
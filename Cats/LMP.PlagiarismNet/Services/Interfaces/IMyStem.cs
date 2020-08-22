using System.Collections.Generic;

namespace LMP.PlagiarismNet.Services.Interfaces
{
    public interface IMyStem
    {
        List<string> Parse(string paramString);
    }
}

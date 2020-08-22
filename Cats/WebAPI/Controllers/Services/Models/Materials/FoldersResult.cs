using System.Collections.Generic;

namespace WebAPI.Controllers.Services.Models.Materials
{
    public class FoldersResult : ResultViewData
    {
        public int Pid { get; set; }

        public List<FoldersViewData> Folders { get; set; }
    }
}
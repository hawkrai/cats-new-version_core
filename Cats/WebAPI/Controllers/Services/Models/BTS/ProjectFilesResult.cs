using System.Collections.Generic;

namespace WebAPI.Controllers.Services.Models.BTS
{
    public class ProjectFilesResult : ResultViewData
    {
        public List<ProjectFileViewData> ProjectFiles { get; set; }
    }
}
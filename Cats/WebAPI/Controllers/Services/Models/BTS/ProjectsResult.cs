using System.Collections.Generic;

namespace WebAPI.Controllers.Services.Models.BTS
{
    public class ProjectsResult : ResultViewData
    {
        public List<ProjectViewData> Projects { get; set; }

        public int TotalCount { get; set; }
    }
}
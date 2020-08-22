using System.Collections.Generic;

namespace WebAPI.Controllers.Services.Models.BTS
{
    public class ProjectMatrixViewData
    {
        public string RequirementsFileName { get; set; }

        public string TestsFileName { get; set; }

        public List<ProjectMatrixRequirementViewData> Requirements { get; set; }
    }
}
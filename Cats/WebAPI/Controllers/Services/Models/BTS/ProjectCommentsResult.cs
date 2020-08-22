using System.Collections.Generic;

namespace WebAPI.Controllers.Services.Models.BTS
{
    public class ProjectCommentsResult : ResultViewData
    {
        public List<ProjectCommentViewData> Comments { get; set; }
    }
}
using System.Collections.Generic;

namespace WebAPI.Controllers.Services.Models.Lectures
{
    public class LecturesMarkVisitingResult : ResultViewData
    {
        public List<LecturesGroupsVisitingViewData> GroupsVisiting { get; set; }
    }
}
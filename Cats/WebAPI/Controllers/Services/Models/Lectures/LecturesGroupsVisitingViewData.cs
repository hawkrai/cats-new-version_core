using System.Collections.Generic;

namespace WebAPI.Controllers.Services.Models.Lectures
{
    public class LecturesGroupsVisitingViewData
    {
        public int GroupId { get; set; }

        public List<LecturesMarkVisitingViewData> LecturesMarksVisiting { get; set; }
    }
}
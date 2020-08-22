using System.Collections.Generic;

namespace WebAPI.Controllers.Services.Models.Lectures
{
    public class LecturesMarkVisitingViewData
    {
        public int StudentId { get; set; }

        public string StudentName { get; set; }

        public string Login { get; set; }

        public List<MarkViewData> Marks { get; set; }
    }
}
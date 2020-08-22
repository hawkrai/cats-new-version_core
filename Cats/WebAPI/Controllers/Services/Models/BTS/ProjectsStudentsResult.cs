using System.Collections.Generic;

namespace WebAPI.Controllers.Services.Models.BTS
{
    public class StudentsParticipationsResult : ResultViewData
    {
        public List<StudentParticipationViewData> ProjectsStudents { get; set; }

        public int TotalCount { get; set; }
    }
}
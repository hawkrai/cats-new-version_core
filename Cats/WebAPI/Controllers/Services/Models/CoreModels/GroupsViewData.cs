using System.Collections.Generic;
using WebAPI.Controllers.Services.Models.Lectures;
using WebAPI.Controllers.Services.Models.Practicals;

namespace WebAPI.Controllers.Services.Models.CoreModels
{
    public class GroupsViewData
    {
        public int GroupId { get; set; }

        public string GroupName { get; set; }

        public int CountUnconfirmedStudents { get; set; }

        public List<StudentsViewData> Students { get; set; }

        public SubGroupsViewData SubGroupsOne { get; set; }

        public SubGroupsViewData SubGroupsTwo { get; set; }

        public SubGroupsViewData SubGroupsThird { get; set; }

        public List<LecturesMarkVisitingViewData> LecturesMarkVisiting { get; set; }

        public List<ScheduleProtectionPracticalViewData> ScheduleProtectionPracticals { get; set; }
    }
}
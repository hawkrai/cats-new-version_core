using System.Collections.Generic;
using WebAPI.Controllers.Services.Models.Labs;

namespace WebAPI.Controllers.Services.Models.CoreModels
{
    public class SubGroupsViewData
    {
        public int SubGroupId { get; set; }

        public int GroupId { get; set; }

        public string Name { get; set; }

        public List<StudentsViewData> Students { get; set; }

        public List<LabsViewData> Labs { get; set; }

        public List<ScheduleProtectionLabsViewData> ScheduleProtectionLabs { get; set; }

        public List<ScheduleProtectionLab> ScheduleProtectionLabsRecomendMark { get; set; }
    }
}
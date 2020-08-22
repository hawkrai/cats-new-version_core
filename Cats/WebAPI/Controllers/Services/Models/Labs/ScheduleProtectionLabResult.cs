using System.Collections.Generic;

namespace WebAPI.Controllers.Services.Models.Labs
{
    public class ScheduleProtectionLabResult : ResultViewData
    {
        public List<ScheduleProtectionLab> ScheduleProtectionLabRecomended { get; set; }
    }
}
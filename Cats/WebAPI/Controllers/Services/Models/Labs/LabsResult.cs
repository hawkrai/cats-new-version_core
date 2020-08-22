using System.Collections.Generic;

namespace WebAPI.Controllers.Services.Models.Labs
{
    public class LabsResult : ResultViewData
    {
        public List<LabsViewData> Labs { get; set; }

        public List<ScheduleProtectionLabsViewData> ScheduleProtectionLabs { get; set; }
    }
}
using System.Runtime.Serialization;
using LMP.Models;

namespace WebAPI.Controllers.Services.Models.Labs
{
    public class ScheduleProtectionLabsViewData
    {
        public ScheduleProtectionLabsViewData(ScheduleProtectionLabs scheduleProtectionLabs)
        {
            ScheduleProtectionLabId = scheduleProtectionLabs.Id;
            SubGroupId = scheduleProtectionLabs.SuGroupId;
            Date = scheduleProtectionLabs.Date.ToString("dd.MM.yyyy");
        }

        public int ScheduleProtectionLabId { get; set; }

        public int SubGroupId { get; set; }

        public int SubGroup { get; set; }

        public string Date { get; set; }
    }
}
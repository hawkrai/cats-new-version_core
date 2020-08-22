using System.Collections.Generic;

namespace WebAPI.Controllers.Services.Models.Lectures
{
    public class CalendarResult : ResultViewData
    {
        public List<CalendarViewData> Calendar { get; set; }
    }
}
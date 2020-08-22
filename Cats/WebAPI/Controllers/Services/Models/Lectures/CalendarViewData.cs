using LMP.Models;

namespace WebAPI.Controllers.Services.Models.Lectures
{
    public class CalendarViewData
    {
        public CalendarViewData(LecturesScheduleVisiting visiting)
        {
            SubjectId = visiting.SubjectId;
            Date = visiting.Date.ToString("dd/MM/yyy");
            Id = visiting.Id;
        }

        public int SubjectId { get; set; }

        public string Date { get; set; }

        public int Id { get; set; }
    }
}
using System.Linq;
using LMP.Models;

namespace WebAPI.Controllers.Services.Models.Labs
{
    public class LabVisitingMarkViewData
    {
        public LabVisitingMarkViewData()
        {
        }

        public LabVisitingMarkViewData(Student student, int scheduleId)
        {
            ScheduleProtectionLabId = scheduleId;
            StudentId = student.Id;
            StudentName = student.FullName;
            if (student.ScheduleProtectionLabMarks.Any(e => e.ScheduleProtectionLabId == scheduleId))
            {
                Comment =
                    student.ScheduleProtectionLabMarks.FirstOrDefault(e => e.ScheduleProtectionLabId == scheduleId)
                        .Comment;
                Mark =
                    student.ScheduleProtectionLabMarks.FirstOrDefault(e => e.ScheduleProtectionLabId == scheduleId)
                        .Mark;
                LabVisitingMarkId = student.ScheduleProtectionLabMarks
                    .FirstOrDefault(e => e.ScheduleProtectionLabId == scheduleId).Id;
            }
            else
            {
                Comment = string.Empty;
                Mark = string.Empty;
                LabVisitingMarkId = 0;
            }
        }

        public int ScheduleProtectionLabId { get; set; }

        public int StudentId { get; set; }

        public string StudentName { get; set; }

        public string Comment { get; set; }

        public string Mark { get; set; }

        public int LabVisitingMarkId { get; set; }
    }
}
using System.Linq;
using LMP.Models;

namespace WebAPI.Controllers.Services.Models.Practicals
{
    public class PracticalVisitingMarkViewData
    {
        public PracticalVisitingMarkViewData()
        {
        }

        public PracticalVisitingMarkViewData(Student student, int scheduleId)
        {
            ScheduleProtectionPracticalId = scheduleId;
            StudentId = student.Id;
            StudentName = student.FullName;
            if (student.ScheduleProtectionPracticalMarks.Any(e => e.ScheduleProtectionPracticalId == scheduleId))
            {
                Comment =
                    student.ScheduleProtectionPracticalMarks
                        .FirstOrDefault(e => e.ScheduleProtectionPracticalId == scheduleId)
                        .Comment;
                Mark =
                    student.ScheduleProtectionPracticalMarks
                        .FirstOrDefault(e => e.ScheduleProtectionPracticalId == scheduleId).Mark;
                PracticalVisitingMarkId = student.ScheduleProtectionPracticalMarks
                    .FirstOrDefault(e => e.ScheduleProtectionPracticalId == scheduleId).Id;
            }
            else
            {
                Comment = string.Empty;
                Mark = string.Empty;
                PracticalVisitingMarkId = 0;
            }
        }

        public int ScheduleProtectionPracticalId { get; set; }

        public int StudentId { get; set; }

        public string StudentName { get; set; }

        public string Comment { get; set; }

        public string Mark { get; set; }

        public int PracticalVisitingMarkId { get; set; }
    }
}
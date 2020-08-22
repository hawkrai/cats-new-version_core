using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Application.Core;
using Application.Infrastructure.KnowledgeTestsManagement;
using LMP.Models;
using LMP.Models.KnowledgeTesting;
using WebAPI.Controllers.Services.Models.Labs;
using WebAPI.Controllers.Services.Models.Practicals;

namespace WebAPI.Controllers.Services.Models.CoreModels
{
    public class StudentsViewData
    {
        public StudentsViewData()
        {
        }

        public StudentsViewData(IReadOnlyCollection<TestPassResult> test, Student student,
            IEnumerable<ScheduleProtectionLabs> scheduleProtectionLabs = null,
            IEnumerable<ScheduleProtectionPractical> scheduleProtectionPracticals = null,
            IEnumerable<LMP.Models.Labs> labs = null, IEnumerable<Practical> practicals = null,
            List<UserlabFilesViewData> userLabsFile = null)
        {
            StudentId = student.Id;
            FullName = student.FullName;
            Login = student.User.UserName;
            GroupId = student.GroupId;
            LabVisitingMark = new List<LabVisitingMarkViewData>();
            PracticalVisitingMark = new List<PracticalVisitingMarkViewData>();
            StudentLabMarks = new List<StudentLabMarkViewData>();
            StudentPracticalMarks = new List<StudentPracticalMarkViewData>();

            if (test != null && test.Any() && test.Any(e => e.Points != null))
            {
                var sum = (double) test.Where(e => e.Points != null).Sum(e => e.Points);
                TestMark = Math.Round(sum / test.Count(e => e.Points != null), 1)
                    .ToString(CultureInfo.InvariantCulture);
            }

            FileLabs = userLabsFile;

            if (labs != null)
                foreach (var lab in labs)
                    if (student.StudentLabMarks.Any(e => e.LabId == lab.Id))
                    {
                        var model =
                            student.StudentLabMarks.FirstOrDefault(e =>
                                e.LabId == lab.Id && !string.IsNullOrEmpty(e.Mark)) != null
                                ? student.StudentLabMarks.FirstOrDefault(e =>
                                    e.LabId == lab.Id && !string.IsNullOrEmpty(e.Mark))
                                : student.StudentLabMarks.FirstOrDefault(e => e.LabId == lab.Id);

                        StudentLabMarks.Add(new StudentLabMarkViewData
                        {
                            LabId = lab.Id,
                            Mark = model.Mark,
                            StudentId = StudentId,
                            Comment = model.Comment,
                            Date = model.Date,
                            StudentLabMarkId = model.Id
                        });
                    }
                    else
                    {
                        StudentLabMarks.Add(new StudentLabMarkViewData
                        {
                            LabId = lab.Id,
                            Mark = string.Empty,
                            StudentId = StudentId,
                            Comment = string.Empty,
                            Date = string.Empty,
                            StudentLabMarkId = 0
                        });
                    }

            if (practicals != null)
                foreach (var practical in practicals)
                    if (student.StudentPracticalMarks.Any(e => e.PracticalId == practical.Id))
                        StudentPracticalMarks.Add(new StudentPracticalMarkViewData
                        {
                            PracticalId = practical.Id,
                            Mark = student.StudentPracticalMarks.FirstOrDefault(e => e.PracticalId == practical.Id)
                                .Mark,
                            StudentId = StudentId,
                            StudentPracticalMarkId = student.StudentPracticalMarks
                                .FirstOrDefault(e => e.PracticalId == practical.Id).Id
                        });
                    else
                        StudentPracticalMarks.Add(new StudentPracticalMarkViewData
                        {
                            PracticalId = practical.Id,
                            Mark = string.Empty,
                            StudentId = StudentId,
                            StudentPracticalMarkId = 0
                        });

            if (scheduleProtectionLabs != null)
                foreach (var scheduleProtectionLab in scheduleProtectionLabs)
                    if (student.ScheduleProtectionLabMarks.Any(e =>
                        e.ScheduleProtectionLabId == scheduleProtectionLab.Id))
                        LabVisitingMark.Add(new LabVisitingMarkViewData
                        {
                            Comment = student.ScheduleProtectionLabMarks
                                .FirstOrDefault(e => e.ScheduleProtectionLabId == scheduleProtectionLab.Id).Comment,
                            Mark = student.ScheduleProtectionLabMarks
                                .FirstOrDefault(e => e.ScheduleProtectionLabId == scheduleProtectionLab.Id).Mark,
                            ScheduleProtectionLabId = scheduleProtectionLab.Id,
                            StudentId = StudentId,
                            LabVisitingMarkId = student.ScheduleProtectionLabMarks
                                .FirstOrDefault(e => e.ScheduleProtectionLabId == scheduleProtectionLab.Id).Id
                        });
                    else
                        LabVisitingMark.Add(new LabVisitingMarkViewData
                        {
                            Comment = string.Empty,
                            Mark = string.Empty,
                            ScheduleProtectionLabId = scheduleProtectionLab.Id,
                            StudentId = StudentId,
                            LabVisitingMarkId = 0
                        });

            if (scheduleProtectionPracticals != null)
                foreach (var scheduleProtectionPractical in scheduleProtectionPracticals)
                    if (student.ScheduleProtectionPracticalMarks.Any(e =>
                        e.ScheduleProtectionPracticalId == scheduleProtectionPractical.Id))
                        PracticalVisitingMark.Add(new PracticalVisitingMarkViewData
                        {
                            Comment = student.ScheduleProtectionPracticalMarks.FirstOrDefault(e =>
                                e.ScheduleProtectionPracticalId == scheduleProtectionPractical.Id).Comment,
                            Mark = student.ScheduleProtectionPracticalMarks.FirstOrDefault(e =>
                                e.ScheduleProtectionPracticalId == scheduleProtectionPractical.Id).Mark,
                            ScheduleProtectionPracticalId = scheduleProtectionPractical.Id,
                            StudentId = StudentId,
                            PracticalVisitingMarkId = student.ScheduleProtectionPracticalMarks.FirstOrDefault(e =>
                                e.ScheduleProtectionPracticalId == scheduleProtectionPractical.Id).Id
                        });
                    else
                        PracticalVisitingMark.Add(new PracticalVisitingMarkViewData
                        {
                            Comment = string.Empty,
                            Mark = string.Empty,
                            ScheduleProtectionPracticalId = scheduleProtectionPractical.Id,
                            StudentId = StudentId,
                            PracticalVisitingMarkId = 0
                        });

            double number;
            var summ = StudentLabMarks
                .Where(studentLabMarkViewData => !string.IsNullOrEmpty(studentLabMarkViewData.Mark) &&
                                                 double.TryParse(studentLabMarkViewData.Mark, out number))
                .Sum(studentLabMarkViewData => double.Parse(studentLabMarkViewData.Mark));
            if (StudentLabMarks.Count(e => !string.IsNullOrEmpty(e.Mark)) != 0)
                LabsMarkTotal =
                    Math.Round(
                            summ / StudentLabMarks.Count(e =>
                                !string.IsNullOrEmpty(e.Mark) && double.TryParse(e.Mark, out number)), 1)
                        .ToString(CultureInfo.InvariantCulture);

            summ = StudentPracticalMarks
                .Where(studentPracticalMarkViewData =>
                    !string.IsNullOrEmpty(studentPracticalMarkViewData.Mark) &&
                    double.TryParse(studentPracticalMarkViewData.Mark, out number)).Sum(studentPracticalMarkViewData =>
                    double.Parse(studentPracticalMarkViewData.Mark));

            var countMark =
                StudentPracticalMarks.Count(studentPracticalMarkViewData =>
                    !string.IsNullOrEmpty(studentPracticalMarkViewData.Mark) &&
                    double.TryParse(studentPracticalMarkViewData.Mark, out number));
            if (countMark != 0)
                PracticalMarkTotal = countMark != 0 ? (summ / countMark).ToString(CultureInfo.InvariantCulture) : "0";
        }

        public int StudentId { get; set; }

        public int SubgroupId { get; set; }

        public string FullName { get; set; }

        public string Login { get; set; }

        public int GroupId { get; set; }

        public List<LabVisitingMarkViewData> LabVisitingMark { get; set; }

        public List<PracticalVisitingMarkViewData> PracticalVisitingMark { get; set; }

        public List<StudentLabMarkViewData> StudentLabMarks { get; set; }

        public string LabsMarkTotal { get; set; }

        public string TestMark { get; set; }

        public string PracticalMarkTotal { get; set; }

        public List<StudentPracticalMarkViewData> StudentPracticalMarks { get; set; }

        public List<UserlabFilesViewData> FileLabs { get; set; }

        public ITestPassingService TestPassingService => ApplicationService<ITestPassingService>();

        public bool? Confirmed { get; set; }

        public TService ApplicationService<TService>()
        {
            return UnityWrapper.Resolve<TService>();
        }
    }
}
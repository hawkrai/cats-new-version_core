using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using Application.Core;
using Application.Core.Data;
using Application.Infrastructure.GroupManagement;
using Application.Infrastructure.SubjectManagement;
using LMP.Models;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Controllers.Services.Models.Lectures;

namespace WebAPI.Controllers.Services.Lectures
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class LecturesServiceController : ApiRoutedController
    {
        private readonly LazyDependency<IGroupManagementService> groupManagementService =
            new LazyDependency<IGroupManagementService>();

        private readonly LazyDependency<ISubjectManagementService> subjectManagementService =
            new LazyDependency<ISubjectManagementService>();

        public IGroupManagementService GroupManagementService => groupManagementService.Value;

        public ISubjectManagementService SubjectManagementService => subjectManagementService.Value;

        [HttpGet("GetLectures/{subjectId}")]
        public IActionResult GetLectures(string subjectId)
        {
            try
            {
                var model = SubjectManagementService.GetSubject(int.Parse(subjectId)).Lectures
                    .Select(e => new LecturesViewData(e)).ToList();

                var lectures = model.OrderBy(e => e.Order).ToList();

                return Ok(lectures);
            }
            catch (Exception ex)
            {
                return ServerError500(ex.Message);
            }
        }

        [HttpGet("GetCalendar/{subjectId}")]
        public IActionResult GetCalendar(string subjectId)
        {
            try
            {
                var entities =
                    SubjectManagementService.GetSubject(int.Parse(subjectId))
                        .LecturesScheduleVisitings.ToList().OrderBy(e => e.Date)
                        .ToList();
                var model = entities.Select(e => new CalendarViewData(e)).ToList();

                return Ok(model);
            }
            catch (Exception ex)
            {
                return ServerError500(ex.Message);
            }
        }

        [HttpPost("Save")]
        public IActionResult Save(string subjectId, string id, string theme, string duration, string order,
            string pathFile, string attachments)
        {
            try
            {
                var attachmentsModel = JsonSerializer.Deserialize<List<Attachment>>(attachments).ToList();
                var subject = int.Parse(subjectId);
                SubjectManagementService.SaveLectures(new LMP.Models.Lectures
                {
                    SubjectId = subject,
                    Duration = int.Parse(duration),
                    Theme = theme,
                    Order = int.Parse(order),
                    Attachments = pathFile,
                    Id = int.Parse(id)
                }, attachmentsModel, /*todo #auth WebSecurity.CurrentUserId*/2);

                return Ok();
            }
            catch (Exception ex)
            {
                return ServerError500(ex.Message);
            }
        }

        [HttpDelete("Delete")]
        public IActionResult Delete(int id, string subjectId)
        {
            try
            {
                SubjectManagementService.DeleteLection(new LMP.Models.Lectures {Id = id});
                return Ok();
            }
            catch (Exception ex)
            {
                return ServerError500(ex.Message);
            }
        }

        [HttpPost("SaveDateLectures")]
        public IActionResult SaveDateLectures(string subjectId, string date)
        {
            try
            {
                SubjectManagementService.SaveDateLectures(int.Parse(subjectId),
                    DateTime.ParseExact(date, "dd/MM/yyyy", CultureInfo.InvariantCulture));
                return Ok();
            }
            catch (Exception ex)
            {
                return ServerError500(ex.Message);
            }
        }

        [HttpGet("GetMarksCalendarData")]
        public IActionResult GetMarksCalendarData(string dateId, string subjectId, string groupId)
        {
            try
            {
                var subjectIntId = int.Parse(subjectId);
                var dateIntId = int.Parse(dateId);
                var visitingDate =
                    SubjectManagementService.GetScheduleVisitings(
                            new Query<LecturesScheduleVisiting>(e => e.SubjectId == subjectIntId && e.Id == dateIntId))
                        .FirstOrDefault();

                var group = GroupManagementService.GetGroup(int.Parse(groupId));
                var model = new List<StudentMarkForDateViewData>();
                foreach (var student in group.Students.OrderBy(e => e.FullName))
                    if (student.LecturesVisitMarks.Any(e => e.LecturesScheduleVisitingId == visitingDate.Id))
                        model.Add(new StudentMarkForDateViewData
                        {
                            MarkId = student.LecturesVisitMarks
                                .FirstOrDefault(e => e.LecturesScheduleVisitingId == visitingDate.Id).Id,
                            StudentId = student.Id,
                            Login = student.User.UserName,
                            StudentName = student.FullName,
                            Mark = student.LecturesVisitMarks
                                .FirstOrDefault(e => e.LecturesScheduleVisitingId == visitingDate.Id).Mark
                        });
                    else
                        model.Add(new StudentMarkForDateViewData
                        {
                            MarkId = 0,
                            StudentId = student.Id,
                            StudentName = student.FullName,
                            Login = student.User.UserName,
                            Mark = string.Empty
                        });

                var result = new StudentMarkForDateResult
                {
                    DateId = dateIntId,
                    Date = visitingDate.Date.ToShortDateString(),
                    StudentMarkForDate = model,
                    Message = "Данные успешно загружены",
                    Code = "200"
                };
                return Ok(result);
            }
            catch (Exception ex)
            {
                return ServerError500(ex.Message);
            }
        }

        [HttpPost("SaveMarksCalendarData")]
        public IActionResult SaveMarksCalendarData(List<LecturesMarkVisitingViewData> lecturesMarks)
        {
            try
            {
                foreach (var student in lecturesMarks)
                    SubjectManagementService.SaveMarksCalendarData(student.Marks.Select(e => new LecturesVisitMark
                    {
                        Id = e.MarkId,
                        Mark = e.Mark,
                        LecturesScheduleVisitingId = e.LecuresVisitId,
                        StudentId = student.StudentId
                    }).ToList());
                return Ok();
            }
            catch (Exception ex)
            {
                return ServerError500(ex.Message);
            }
        }

        [HttpPost("SaveMarksCalendarDataSingle")]
        public IActionResult SaveMarksCalendarDataSingle(int markId, string mark, int lecuresVisitId, int studentId)
        {
            try
            {
                SubjectManagementService.SaveMarksCalendarData(new List<LecturesVisitMark>
                {
                    new LecturesVisitMark
                    {
                        Id = markId,
                        Mark = mark,
                        LecturesScheduleVisitingId = lecuresVisitId,
                        StudentId = studentId
                    }
                });

                return Ok();
            }
            catch (Exception ex)
            {
                return ServerError500(ex.Message);
            }
        }

        [HttpDelete("DeleteVisitingDate/{id:int}")]
        public IActionResult DeleteVisitingDate(int id)
        {
            try
            {
                SubjectManagementService.DeleteLectionVisitingDate(id);

                return Ok();
            }
            catch (Exception ex)
            {
                return ServerError500(ex.Message);
            }
        }

        [HttpPost("DeleteVisitingDates")]
        public IActionResult DeleteVisitingDates(List<string> dateIds)
        {
            try
            {
                dateIds.ForEach(e => SubjectManagementService.DeleteLectionVisitingDate(int.Parse(e)));

                return Ok();
            }
            catch (Exception ex)
            {
                return ServerError500(ex.Message);
            }
        }
    }
}
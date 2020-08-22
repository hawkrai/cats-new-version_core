using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Application.Core;
using Application.Infrastructure.SubjectManagement;
using LMP.Models;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Controllers.Services.Models.CoreModels;
using WebAPI.Controllers.Services.Models.Practicals;

namespace WebAPI.Controllers.Services.Practicals
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class PracticalServiceController : ApiRoutedController
    {
        private readonly LazyDependency<ISubjectManagementService> subjectManagementService =
            new LazyDependency<ISubjectManagementService>();

        public ISubjectManagementService SubjectManagementService => subjectManagementService.Value;

        [HttpGet("GetPracticals/{subjectId}")]
        public IActionResult GetLabs(string subjectId)
        {
            try
            {
                var sub = SubjectManagementService.GetSubject(int.Parse(subjectId));
                var model = new List<PracticalsViewData>();
                if (sub.SubjectModules.Any(e => e.ModuleId == 13))
                    model = sub.Practicals.Select(e => new PracticalsViewData(e)).ToList();

                return Ok(model);
            }
            catch (Exception ex)
            {
                return ServerError500(ex.Message);
            }
        }

        [HttpPost("Save")]
        public IActionResult Save(string subjectId, string id, string theme, string duration, string order,
            string shortName, string pathFile, string attachments)
        {
            try
            {
                var attachmentsModel = JsonSerializer.Deserialize<List<Attachment>>(attachments).ToList();
                var subject = int.Parse(subjectId);
                SubjectManagementService.SavePractical(new Practical
                {
                    SubjectId = subject,
                    Duration = int.Parse(duration),
                    Theme = theme,
                    Order = int.Parse(order),
                    ShortName = shortName,
                    Attachments = pathFile,
                    Id = int.Parse(id)
                }, attachmentsModel, /*todo auth WebSecurity.CurrentUserId*/2);
                return Ok();
            }
            catch (Exception ex)
            {
                return ServerError500(ex.Message);
            }
        }

        [HttpPost("Delete")]
        public IActionResult Delete(string id, string subjectId)
        {
            try
            {
                SubjectManagementService.DeletePracticals(int.Parse(id));
                return Ok();
            }
            catch (Exception e)
            {
                return ServerError500(e.Message);
            }
        }

        [HttpPost("SaveScheduleProtectionDate")]
        public IActionResult SaveScheduleProtectionDate(string groupId, string date, string subjectId)
        {
            try
            {
                SubjectManagementService.SaveScheduleProtectionPracticalDate(new ScheduleProtectionPractical
                {
                    GroupId = int.Parse(groupId),
                    Date = DateTime.Parse(date),
                    SubjectId = int.Parse(subjectId)
                });
                return Ok();
            }
            catch (Exception ex)
            {
                return ServerError500(ex.Message);
            }
        }

        [HttpPost("SavePracticalsVisitingData")]
        public IActionResult SavePracticalsVisitingData(List<StudentsViewData> students)
        {
            try
            {
                foreach (var studentsViewData in students)
                    SubjectManagementService.SavePracticalVisitingData(studentsViewData.PracticalVisitingMark.Select(
                        e => new ScheduleProtectionPracticalMark
                        {
                            Comment = e.Comment,
                            Mark = e.Mark,
                            ScheduleProtectionPracticalId = e.ScheduleProtectionPracticalId,
                            Id = e.PracticalVisitingMarkId,
                            StudentId = e.StudentId
                        }).ToList());

                return Ok();
            }
            catch (Exception ex)
            {
                return ServerError500(ex.Message);
            }
        }

        [HttpPost("SaveStudentPracticalsMark")]
        public IActionResult SaveStudentPracticalsMark(List<StudentsViewData> students)
        {
            try
            {
                foreach (var studentsViewData in students)
                    SubjectManagementService.SavePracticalMarks(studentsViewData.StudentPracticalMarks.Select(e =>
                        new StudentPracticalMark
                        {
                            Mark = e.Mark,
                            PracticalId = e.PracticalId,
                            Id = e.StudentPracticalMarkId,
                            StudentId = e.StudentId
                        }).ToList());

                return Ok();
            }
            catch (Exception ex)
            {
                return ServerError500(ex.Message);
            }
        }

        [HttpDelete("DeleteVisitingDate/{id}")]
        public IActionResult DeleteVisitingDate(string id)
        {
            try
            {
                SubjectManagementService.DeletePracticalsVisitingDate(int.Parse(id));

                return Ok();
            }
            catch (Exception ex)
            {
                return ServerError500(ex.Message);
            }
        }
    }
}
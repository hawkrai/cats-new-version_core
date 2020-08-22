using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Application.Core;
using Application.Core.Data;
using Application.Core.Extensions;
using Application.Infrastructure.FilesManagement;
using Application.Infrastructure.GroupManagement;
using Application.Infrastructure.KnowledgeTestsManagement;
using Application.Infrastructure.LecturerManagement;
using Application.Infrastructure.StudentManagement;
using Application.Infrastructure.SubjectManagement;
using LMP.Models;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Controllers.Services.Models;
using WebAPI.Controllers.Services.Models.CoreModels;
using WebAPI.Controllers.Services.Models.Labs;
using WebAPI.Controllers.Services.Models.Lectures;
using WebAPI.Controllers.Services.Models.Parental;
using WebAPI.Controllers.Services.Models.Practicals;

namespace WebAPI.Controllers.Services
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class CoreServiceController : ApiRoutedController
    {
        private readonly LazyDependency<IFilesManagementService> filesManagementService =
            new LazyDependency<IFilesManagementService>();

        private readonly LazyDependency<IGroupManagementService> groupManagementService =
            new LazyDependency<IGroupManagementService>();

        private readonly LazyDependency<ILecturerManagementService> lecturerManagementService =
            new LazyDependency<ILecturerManagementService>();

        private readonly LazyDependency<IStudentManagementService> studentManagementService =
            new LazyDependency<IStudentManagementService>();

        private readonly LazyDependency<ISubjectManagementService> subjectManagementService =
            new LazyDependency<ISubjectManagementService>();

        private readonly LazyDependency<ITestPassingService> testPassingService =
            new LazyDependency<ITestPassingService>();

        public IGroupManagementService GroupManagementService => groupManagementService.Value;

        public IStudentManagementService StudentManagementService => studentManagementService.Value;

        public ITestPassingService TestPassingService => testPassingService.Value;

        public ISubjectManagementService SubjectManagementService => subjectManagementService.Value;

        public IFilesManagementService FilesManagementService => filesManagementService.Value;

        public ILecturerManagementService LecturerManagementService => lecturerManagementService.Value;

        [HttpPost("DisjoinLector")]
        public IActionResult DisjoinLector(string subjectId, string lectorId)
        {
            try
            {
                LecturerManagementService.DisjoinLector(int.Parse(subjectId),
                    int.Parse(lectorId), /*todo #auth WebSecurity.CurrentUserId*/2);

                return Ok();
            }
            catch (Exception ex)
            {
                return ServerError500(ex.Message);
            }
        }

        [HttpGet("GetJoinedLector/{subjectId}")]
        public IActionResult GetJoinedLector(string subjectId)
        {
            try
            {
                var lectors =
                    LecturerManagementService.GetJoinedLector(
                        int.Parse(subjectId), /*todo #auth WebSecurity.CurrentUserId*/2);

                var result = lectors.Select(e => new LectorViewData(e)).ToList();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return ServerError500(ex.Message);
            }
        }

        [HttpPost("JoinLector")]
        public IActionResult JoinLector(int subjectId, int lectorId)
        {
            try
            {
                LecturerManagementService.Join(subjectId, lectorId, /*todo #auth WebSecurity.CurrentUserId*/2);

                return Ok();
            }
            catch (Exception ex)
            {
                return ServerError500(ex.Message);
            }
        }

        [HttpGet("GetNoAdjointLectors/{subjectId}")]
        public IActionResult GetNoAdjointLectors(string subjectId)
        {
            try
            {
                var lectors = LecturerManagementService.GetLecturers().Where(e =>
                    e.Id != /*todo #auth WebSecurity.CurrentUserId*/2 && !e.SubjectLecturers.Any(x =>
                        x.SubjectId == int.Parse(subjectId) && x.Owner == /*todo #auth WebSecurity.CurrentUserId*/2));

                var result = lectors.Select(e => new LectorViewData(e)).ToList();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return ServerError500(ex.Message);
            }
        }

        [HttpGet("GetSubjectsByOwnerUser")]
        public IActionResult GetSubjectsByOwnerUser()
        {
            try
            {
                var subjects =
                    SubjectManagementService.GetSubjectsByLectorOwner( /*todo #auth WebSecurity.CurrentUserId*/2);

                var result = subjects.Select(e => new SubjectViewData {Id = e.Id, Name = e.Name}).ToList();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return ServerError500(ex.Message);
            }
        }

        [HttpPut("СonfirmationStudent/{studentId}")]
        public IActionResult СonfirmationStudent(string studentId)
        {
            try
            {
                StudentManagementService.СonfirmationStudent(int.Parse(studentId));

                return Ok();
            }
            catch (Exception ex)
            {
                return ServerError500(ex.Message);
            }
        }

        [HttpPut("UnConfirmationStudent/{studentId}")]
        public IActionResult UnConfirmationStudent(string studentId)
        {
            try
            {
                StudentManagementService.UnConfirmationStudent(int.Parse(studentId));

                return Ok();
            }
            catch (Exception ex)
            {
                return ServerError500(ex.Message);
            }
        }

        [HttpGet("GetStudentsByGroupId/{groupId}")]
        public IActionResult GetStudentsByGroupId(string groupId)
        {
            try
            {
                var students = GroupManagementService.GetGroup(int.Parse(groupId)).Students.OrderBy(e => e.FullName);

                var result = students.Select(e => new StudentsViewData
                        {StudentId = e.Id, FullName = e.FullName, Confirmed = e.Confirmed == null || e.Confirmed.Value})
                    .ToList();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return ServerError500(ex.Message);
            }
        }

        [HttpGet("GetStudentsByStudentGroupId/{subjectId}/{groupId}")]
        public IActionResult GetStudentsByStudentGroupId(string groupId, string subjectId)
        {
            try
            {
                var subGroups = SubjectManagementService.GetSubGroupsV3(int.Parse(subjectId), int.Parse(groupId));
                var Students = new List<StudentsViewData>();
                var subGroupIndex = 0;
                foreach (var subGroup in subGroups)
                {
                    Students.AddRange(subGroup.SubjectStudents.Select(e => new StudentsViewData
                    {
                        StudentId = e.Student.Id,
                        FullName = e.Student.FullName,
                        Confirmed = e.Student.Confirmed == null || e.Student.Confirmed.Value,
                        SubgroupId = subGroupIndex,
                        Login = e.Student.User.UserName
                    }));
                    subGroupIndex++;
                }

                return Ok(Students);
            }
            catch (Exception ex)
            {
                return ServerError500(ex.Message);
            }
        }

        [HttpGet("GetAllGroupsLite")]
        public IActionResult GetAllGroupsLite()
        {
            try
            {
                var groups = GroupManagementService.GetLecturesGroups( /*todo #auth WebSecurity.CurrentUserId*/2);

                var groupsViewModel = new List<GroupsViewData>();

                foreach (var group in groups.DistinctBy(e => e.Id))
                {
                    var students = StudentManagementService.GetGroupStudents(group.Id)
                        .Count(e => e.Confirmed != null && !e.Confirmed.Value);

                    groupsViewModel.Add(new GroupsViewData
                    {
                        CountUnconfirmedStudents = students,
                        GroupId = group.Id,
                        GroupName = students > 0 ? group.Name + " - (" + students + ")" : group.Name
                    });
                }

                return Ok(groupsViewModel);
            }
            catch (Exception ex)
            {
                return ServerError500(ex.Message);
            }
        }

        [HttpGet("GetOnlyGroups/{subjectId}")]
        public IActionResult GetOnlyGroups(string subjectId)
        {
            try
            {
                var id = int.Parse(subjectId);
                var groups =
                    GroupManagementService.GetGroups(new Query<Group>(e =>
                        e.SubjectGroups.Any(x => x.SubjectId == id)));
                var result = groups.Select(e => new GroupsViewData {GroupId = e.Id, GroupName = e.Name}).ToList();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return ServerError500(ex.Message);
            }
        }

        [HttpGet("GetGroupsV2/{subjectId}")]
        public IActionResult GetGroupsV2(string subjectId)
        {
            try
            {
                var id = int.Parse(subjectId);
                var groups = GroupManagementService.GetGroups(new Query<Group>(e =>
                    e.SubjectGroups.Any(x => x.SubjectId == id && x.IsActiveOnCurrentGroup)));


                var groupsViewData = new List<GroupsViewData>();

                foreach (var group in groups)
                {
                    var subGroups = SubjectManagementService.GetSubGroupsV2(id, group.Id);
                    groupsViewData.Add(new GroupsViewData
                    {
                        GroupId = group.Id,
                        GroupName = group.Name,
                        SubGroupsOne = subGroups.Any(x => x.Name == "first")
                            ? new SubGroupsViewData
                            {
                                Name = "Подгруппа 1",
                                SubGroupId = subGroups.First(e => e.Name == "first").Id
                            }
                            : new SubGroupsViewData(),
                        SubGroupsTwo = subGroups.Any(x => x.Name == "second")
                            ? new SubGroupsViewData
                            {
                                Name = "Подгруппа 2",
                                SubGroupId = subGroups.First(e => e.Name == "second").Id
                            }
                            : new SubGroupsViewData(),
                        SubGroupsThird = subGroups.Any(x => x.Name == "third")
                            ? new SubGroupsViewData
                            {
                                Name = "Подгруппа 3",
                                SubGroupId = subGroups.First(e => e.Name == "third").Id
                            }
                            : new SubGroupsViewData()
                    });
                }

                var result = groupsViewData.OrderBy(e => e.GroupName).ToList();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return ServerError500(ex.Message);
            }
        }

        [HttpGet("GetGroupsV3/{subjectId}")]
        public IActionResult GetGroupsV3(string subjectId)
        {
            try
            {
                var id = int.Parse(subjectId);
                var groups = GroupManagementService.GetGroups(new Query<Group>(e =>
                    e.SubjectGroups.Any(x => x.SubjectId == id && !x.IsActiveOnCurrentGroup)));


                var groupsViewData = new List<GroupsViewData>();

                foreach (var group in groups)
                {
                    var subGroups = SubjectManagementService.GetSubGroupsV2(id, group.Id);
                    groupsViewData.Add(new GroupsViewData
                    {
                        GroupId = group.Id,
                        GroupName = group.Name,
                        SubGroupsOne = subGroups.Any(x => x.Name == "first")
                            ? new SubGroupsViewData
                            {
                                Name = "Подгруппа 1",
                                SubGroupId = subGroups.First(e => e.Name == "first").Id
                            }
                            : new SubGroupsViewData(),
                        SubGroupsTwo = subGroups.Any(x => x.Name == "second")
                            ? new SubGroupsViewData
                            {
                                Name = "Подгруппа 2",
                                SubGroupId = subGroups.First(e => e.Name == "second").Id
                            }
                            : new SubGroupsViewData(),
                        SubGroupsThird = subGroups.Any(x => x.Name == "third")
                            ? new SubGroupsViewData
                            {
                                Name = "Подгруппа 3",
                                SubGroupId = subGroups.First(e => e.Name == "third").Id
                            }
                            : new SubGroupsViewData()
                    });
                }

                return Ok(groupsViewData);
            }
            catch (Exception ex)
            {
                return ServerError500(ex.Message);
            }
        }

        [HttpGet("GetLecturesMarkVisitingV2")]
        public IActionResult GetLecturesMarkVisitingV2(int subjectId, int groupId)
        {
            try
            {
                var groups = GroupManagementService.GetGroup(groupId);

                var lecturesVisitingData = SubjectManagementService
                    .GetScheduleVisitings(new Query<LecturesScheduleVisiting>(e => e.SubjectId == subjectId))
                    .OrderBy(e => e.Date);

                var lecturesVisiting = new List<LecturesMarkVisitingViewData>();

                foreach (var student in groups.Students.Where(e => e.Confirmed == null || e.Confirmed.Value)
                    .OrderBy(e => e.FullName))
                {
                    var data = new List<MarkViewData>();

                    foreach (var lecturesScheduleVisiting in lecturesVisitingData.OrderBy(e => e.Date))
                        if (
                            student.LecturesVisitMarks.Any(
                                e => e.LecturesScheduleVisitingId == lecturesScheduleVisiting.Id))
                            data.Add(new MarkViewData
                            {
                                Date = lecturesScheduleVisiting.Date.ToShortDateString(),
                                LecuresVisitId = lecturesScheduleVisiting.Id,
                                Mark = student.LecturesVisitMarks.FirstOrDefault(e =>
                                    e.LecturesScheduleVisitingId == lecturesScheduleVisiting.Id).Mark,
                                MarkId = student.LecturesVisitMarks.FirstOrDefault(e =>
                                    e.LecturesScheduleVisitingId == lecturesScheduleVisiting.Id).Id
                            });
                        else
                            data.Add(new MarkViewData
                            {
                                Date = lecturesScheduleVisiting.Date.ToShortDateString(),
                                LecuresVisitId = lecturesScheduleVisiting.Id,
                                Mark = string.Empty,
                                MarkId = 0
                            });

                    lecturesVisiting.Add(new LecturesMarkVisitingViewData
                    {
                        StudentId = student.Id,
                        StudentName = student.FullName,
                        Login = student.User.UserName,
                        Marks = data
                    });
                }

                var dataResulet = new List<LecturesGroupsVisitingViewData>
                {
                    new LecturesGroupsVisitingViewData
                    {
                        GroupId =
                            groupId,
                        LecturesMarksVisiting
                            =
                            lecturesVisiting
                    }
                };

                return Ok(dataResulet);
            }
            catch (Exception ex)
            {
                return ServerError500(ex.Message);
            }
        }

        [HttpGet("GetGroupsByUser/{userId}")]
        public IActionResult GetGroupsByUser(string userId)
        {
            try
            {
                var groups = GroupManagementService.GetLecturesGroups(int.Parse(userId));

                var groupsViewModel = new List<GroupsViewData>();

                foreach (var group in groups.DistinctBy(e => e.Id))
                    groupsViewModel.Add(new GroupsViewData
                    {
                        GroupId = group.Id,
                        GroupName = group.Name
                    });

                var result = groupsViewModel.ToList();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return ServerError500(ex.Message);
            }
        }

        [HttpGet("GetGroups")]
        public IActionResult GetGroups(string subjectId, string groupId)
        {
            try
            {
                var id = int.Parse(subjectId);

                Query<Group> query;

                if (!string.IsNullOrEmpty(groupId))
                {
                    var groupdId = int.Parse(groupId);
                    query = (Query<Group>) new Query<Group>(e =>
                            e.SubjectGroups.Any(x => x.SubjectId == id && x.GroupId == groupdId))
                        .Include(e => e.Students.Select(x => x.LecturesVisitMarks))
                        .Include(e => e.Students.Select(x => x.StudentPracticalMarks))
                        .Include(e => e.Students.Select(x => x.User))
                        .Include(e => e.Students.Select(x => x.ScheduleProtectionPracticalMarks))
                        .Include(e => e.ScheduleProtectionPracticals);
                }
                else
                {
                    query = (Query<Group>) new Query<Group>(e => e.SubjectGroups.Any(x => x.SubjectId == id))
                        .Include(e => e.Students.Select(x => x.LecturesVisitMarks))
                        .Include(e => e.Students.Select(x => x.StudentPracticalMarks))
                        .Include(e => e.Students.Select(x => x.User))
                        .Include(e => e.Students.Select(x => x.ScheduleProtectionPracticalMarks))
                        .Include(e => e.ScheduleProtectionPracticals);
                }

                var groups =
                    GroupManagementService.GetGroups(query).ToList();

                var model = new List<GroupsViewData>();

                var labsData = SubjectManagementService.GetSubject(int.Parse(subjectId)).Labs.OrderBy(e => e.Order)
                    .ToList();
                var practicalsData = new List<Practical>();
                var isPractModule =
                    SubjectManagementService.GetSubject(int.Parse(subjectId)).SubjectModules.Any(e => e.ModuleId == 13);
                if (isPractModule)
                    practicalsData = SubjectManagementService.GetSubject(int.Parse(subjectId)).Practicals
                        .OrderBy(e => e.Order).ToList();

                foreach (var group in groups)
                {
                    var subGroups = SubjectManagementService.GetSubGroups(id, group.Id);

                    var subjectIntId = int.Parse(subjectId);

                    var lecturesVisitingData = SubjectManagementService
                        .GetScheduleVisitings(new Query<LecturesScheduleVisiting>(e => e.SubjectId == subjectIntId))
                        .OrderBy(e => e.Date);

                    var lecturesVisiting = new List<LecturesMarkVisitingViewData>();

                    var scheduleProtectionPracticals =
                        group.ScheduleProtectionPracticals
                            .Where(e => e.SubjectId == subjectIntId && e.GroupId == group.Id)
                            .ToList().OrderBy(e => e.Date)
                            .ToList();

                    foreach (var student in group.Students.Where(e => e.Confirmed == null || e.Confirmed.Value)
                        .OrderBy(e => e.FullName))
                    {
                        var data = new List<MarkViewData>();

                        foreach (var lecturesScheduleVisiting in lecturesVisitingData.OrderBy(e => e.Date))
                            if (
                                student.LecturesVisitMarks.Any(
                                    e => e.LecturesScheduleVisitingId == lecturesScheduleVisiting.Id))
                                data.Add(new MarkViewData
                                {
                                    Date = lecturesScheduleVisiting.Date.ToShortDateString(),
                                    LecuresVisitId = lecturesScheduleVisiting.Id,
                                    Mark = student.LecturesVisitMarks.FirstOrDefault(e =>
                                        e.LecturesScheduleVisitingId == lecturesScheduleVisiting.Id).Mark,
                                    MarkId = student.LecturesVisitMarks.FirstOrDefault(e =>
                                        e.LecturesScheduleVisitingId == lecturesScheduleVisiting.Id).Id
                                });
                            else
                                data.Add(new MarkViewData
                                {
                                    Date = lecturesScheduleVisiting.Date.ToShortDateString(),
                                    LecuresVisitId = lecturesScheduleVisiting.Id,
                                    Mark = string.Empty,
                                    MarkId = 0
                                });

                        lecturesVisiting.Add(new LecturesMarkVisitingViewData
                        {
                            StudentId = student.Id,
                            StudentName = student.FullName,
                            Login = student.User.UserName,
                            Marks = data
                        });
                    }

                    //first subGroupLabs
                    var labsFirstSubGroup = labsData.Select(e => new LabsViewData
                    {
                        Theme = e.Theme,
                        Order = e.Order,
                        Duration = e.Duration,
                        ShortName = e.ShortName,
                        LabId = e.Id,
                        SubjectId = e.SubjectId,
                        ScheduleProtectionLabsRecomend = subGroups.Any()
                            ? subGroups.FirstOrDefault(x => x.Name == "first").ScheduleProtectionLabs
                                .OrderBy(x => x.Date)
                                .Select(x => new ScheduleProtectionLab
                                    {ScheduleProtectionId = x.Id, Mark = string.Empty}).ToList()
                            : new List<ScheduleProtectionLab>()
                    }).ToList();

                    var durationCount = 0;

                    foreach (var lab in labsFirstSubGroup)
                    {
                        var mark = 10;
                        durationCount += lab.Duration / 2;
                        for (var i = 0; i < lab.ScheduleProtectionLabsRecomend.Count; i++)
                            if (i + 1 > durationCount - lab.Duration / 2)
                            {
                                lab.ScheduleProtectionLabsRecomend[i].Mark =
                                    mark.ToString(CultureInfo.InvariantCulture);

                                if (i + 1 >= durationCount)
                                    if (mark != 1)
                                        mark -= 1;
                            }
                    }

                    //second subGroupLabs
                    var labsSecondSubGroup = labsData.Select(e => new LabsViewData
                    {
                        Theme = e.Theme,
                        Order = e.Order,
                        Duration = e.Duration,
                        ShortName = e.ShortName,
                        LabId = e.Id,
                        SubjectId = e.SubjectId,
                        ScheduleProtectionLabsRecomend = subGroups.Any()
                            ? subGroups.FirstOrDefault(x => x.Name == "second")
                                .ScheduleProtectionLabs.OrderBy(x => x.Date)
                                .Select(x => new ScheduleProtectionLab
                                    {ScheduleProtectionId = x.Id, Mark = string.Empty})
                                .ToList()
                            : new List<ScheduleProtectionLab>()
                    }).ToList();
                    durationCount = 0;
                    foreach (var lab in labsSecondSubGroup)
                    {
                        var mark = 10;
                        durationCount += lab.Duration / 2;
                        for (var i = 0; i < lab.ScheduleProtectionLabsRecomend.Count; i++)
                            if (i + 1 > durationCount - lab.Duration / 2)
                            {
                                lab.ScheduleProtectionLabsRecomend[i].Mark =
                                    mark.ToString(CultureInfo.InvariantCulture);

                                if (i + 1 >= durationCount)
                                    if (mark != 1)
                                        mark -= 1;
                            }
                    }

                    //second subGroupLabs
                    var labsThirdSubGroup = labsData.Select(e => new LabsViewData
                    {
                        Theme = e.Theme,
                        Order = e.Order,
                        Duration = e.Duration,
                        ShortName = e.ShortName,
                        LabId = e.Id,
                        SubjectId = e.SubjectId,
                        ScheduleProtectionLabsRecomend = subGroups.Any()
                            ? subGroups.FirstOrDefault(x => x.Name == "third")
                                .ScheduleProtectionLabs.OrderBy(x => x.Date)
                                .Select(x => new ScheduleProtectionLab
                                    {ScheduleProtectionId = x.Id, Mark = string.Empty})
                                .ToList()
                            : new List<ScheduleProtectionLab>()
                    }).ToList();
                    durationCount = 0;
                    foreach (var lab in labsThirdSubGroup)
                    {
                        var mark = 10;
                        durationCount += lab.Duration / 2;
                        for (var i = 0; i < lab.ScheduleProtectionLabsRecomend.Count; i++)
                            if (i + 1 > durationCount - lab.Duration / 2)
                            {
                                lab.ScheduleProtectionLabsRecomend[i].Mark =
                                    mark.ToString(CultureInfo.InvariantCulture);

                                if (i + 1 >= durationCount)
                                    if (mark != 1)
                                        mark -= 1;
                            }
                    }

                    model.Add(new GroupsViewData
                    {
                        GroupId = group.Id,
                        GroupName = group.Name,
                        LecturesMarkVisiting = lecturesVisiting,
                        ScheduleProtectionPracticals = scheduleProtectionPracticals.Select(e =>
                            new ScheduleProtectionPracticalViewData
                            {
                                GroupId = e.GroupId,
                                Date = e.Date.ToShortDateString(),
                                SubjectId = e.SubjectId,
                                ScheduleProtectionPracticalId = e.Id
                            }).ToList(),
                        Students = group.Students.Where(e => e.Confirmed == null || e.Confirmed.Value)
                            .OrderBy(e => e.LastName).Select(e =>
                                new StudentsViewData(TestPassingService.GetStidentResults(subjectIntId, e.User.Id), e,
                                    null, scheduleProtectionPracticals, null, practicalsData)).ToList(),
                        SubGroupsOne = subGroups.Any()
                            ? new SubGroupsViewData
                            {
                                GroupId = group.Id,
                                Name = "Подгруппа 1",
                                Labs = labsFirstSubGroup,
                                ScheduleProtectionLabs = subGroups.FirstOrDefault(x => x.Name == "first")
                                    .ScheduleProtectionLabs.OrderBy(e => e.Date)
                                    .Select(e => new ScheduleProtectionLabsViewData(e)).ToList(),
                                SubGroupId = subGroups.FirstOrDefault(x => x.Name == "first").Id,
                                Students = subGroups.FirstOrDefault(x => x.Name == "first").SubjectStudents
                                    .Where(e => e.Student.Confirmed == null || e.Student.Confirmed.Value)
                                    .OrderBy(e => e.Student.LastName).Select(e =>
                                        new StudentsViewData(
                                            TestPassingService.GetStidentResults(subjectIntId, e.StudentId), e.Student,
                                            subGroups.FirstOrDefault(x => x.Name == "first").ScheduleProtectionLabs
                                                .OrderBy(x => x.Date).ToList(), null, labsData)).ToList()
                            }
                            : null,
                        SubGroupsTwo = subGroups.Any()
                            ? new SubGroupsViewData
                            {
                                GroupId = group.Id,
                                Name = "Подгруппа 2",
                                Labs = labsSecondSubGroup,
                                ScheduleProtectionLabs = subGroups.FirstOrDefault(x => x.Name == "second")
                                    .ScheduleProtectionLabs.OrderBy(e => e.Date)
                                    .Select(e => new ScheduleProtectionLabsViewData(e)).ToList(),
                                SubGroupId = subGroups.FirstOrDefault(x => x.Name == "second").Id,
                                Students = subGroups.FirstOrDefault(x => x.Name == "second").SubjectStudents
                                    .Where(e => e.Student.Confirmed == null || e.Student.Confirmed.Value)
                                    .OrderBy(e => e.Student.LastName).Select(e =>
                                        new StudentsViewData(
                                            TestPassingService.GetStidentResults(subjectIntId, e.StudentId), e.Student,
                                            subGroups.FirstOrDefault(x => x.Name == "second").ScheduleProtectionLabs
                                                .OrderBy(x => x.Date).ToList(), null, labsData)).ToList()
                            }
                            : null,
                        SubGroupsThird = subGroups.Any()
                            ? new SubGroupsViewData
                            {
                                GroupId = group.Id,
                                Name = "Подгруппа 3",
                                Labs = labsThirdSubGroup,
                                ScheduleProtectionLabs = subGroups.FirstOrDefault(x => x.Name == "third")
                                    .ScheduleProtectionLabs.OrderBy(e => e.Date)
                                    .Select(e => new ScheduleProtectionLabsViewData(e)).ToList(),
                                SubGroupId = subGroups.FirstOrDefault(x => x.Name == "third").Id,
                                Students = subGroups.FirstOrDefault(x => x.Name == "third").SubjectStudents
                                    .Where(e => e.Student.Confirmed == null || e.Student.Confirmed.Value)
                                    .OrderBy(e => e.Student.LastName).Select(e =>
                                        new StudentsViewData(
                                            TestPassingService.GetStidentResults(subjectIntId, e.StudentId), e.Student,
                                            subGroups.FirstOrDefault(x => x.Name == "third").ScheduleProtectionLabs
                                                .OrderBy(x => x.Date).ToList(), null, labsData)).ToList()
                            }
                            : null
                    });
                }

                if (!isPractModule)
                    foreach (var groupsViewData in model)
                    foreach (var student in groupsViewData.Students.Where(e => e.Confirmed == null || e.Confirmed.Value)
                    )
                    {
                        student.PracticalVisitingMark = new List<PracticalVisitingMarkViewData>();
                        student.PracticalMarkTotal = "-";
                    }

                return Ok(model);
            }
            catch (Exception ex)
            {
                return ServerError500(ex.Message);
            }
        }

        [HttpGet("GetLecturers/All")]
        public IActionResult GetLecturers()
        {
            var lecturers = LecturerManagementService.GetLecturers(e => e.LastName).Select(e => new LectorViewData(e))
                .ToList();
            return Ok(lecturers);
        }
    }
}
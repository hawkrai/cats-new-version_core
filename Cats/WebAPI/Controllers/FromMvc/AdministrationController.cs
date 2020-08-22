using Application.Core;
using Application.Infrastructure.GroupManagement;
using Application.Infrastructure.LecturerManagement;
using Application.Infrastructure.StudentManagement;
using Application.Infrastructure.SubjectManagement;
using Application.Infrastructure.UserManagement;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Net;
using WebAPI.ViewModels.AccountViewModels;
using WebAPI.ViewModels.AdministrationViewModels;

namespace WebAPI.Controllers.FromMvc
{
    public class AdministrationController : ApiRoutedController
    {
        public IStudentManagementService StudentManagementService => UnityWrapper.Resolve<IStudentManagementService>();

        public IUsersManagementService UsersManagementService => UnityWrapper.Resolve<IUsersManagementService>();

        public ILecturerManagementService LecturerManagementService =>
            UnityWrapper.Resolve<ILecturerManagementService>();

        public IGroupManagementService GroupManagementService => UnityWrapper.Resolve<IGroupManagementService>();

        public ISubjectManagementService SubjectManagementService => UnityWrapper.Resolve<ISubjectManagementService>();

        /// <summary>
        ///     Ok
        /// </summary>
        [HttpGet("UserActivity")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public IActionResult UserActivity()
        {
            var activityModel = new UserActivityViewModel();

            return Ok(activityModel);
        }

        /// <summary>
        ///     Ok
        /// </summary>
        [HttpGet("Students")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public IActionResult Students()
        {
            var students = StudentManagementService.GetStudents();

            var result = students.Select(s => new ModifyStudentViewModel(s) {Avatar = null});

            return Ok(result);
        }

        /// <summary>
        ///     Ok
        /// </summary>
        [HttpGet("Student/{id:int}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public IActionResult GetStudent(int id)
        {
            var student = StudentManagementService.GetStudent(id);

            if (student != null)
            {
                var model = new ModifyStudentViewModel(student);

                return Ok(model);
            }

            return BadRequest();
        }

        /// <summary>
        ///     Not tested
        /// </summary>
        [HttpPost("SaveStudent")]
        public IActionResult SaveStudent(ModifyStudentViewModel model)
        {
            try
            {
                var user = UsersManagementService.GetUser(model.Id);

                if (user != null)
                {
                    model.ModifyStudent();
                    return Ok();
                }

                return BadRequest();
            }
            catch (Exception ex)
            {
                return ServerError500(ex.Message);
            }
        }

        /// <summary>
        ///     Not tested
        /// </summary>
        [HttpPost("AddProfessor")]
        public IActionResult AddProfessorJson(RegisterViewModel model)
        {
            try
            {
                var user = UsersManagementService.GetUserByName(model.Name, model.Surname, model.Patronymic);

                if (user != null)
                {
                    model.RegistrationUser(new[] {"lector"});
                    return Ok();
                }

                return BadRequest();
            }
            catch (Exception ex)
            {
                return ServerError500(ex.Message);
            }
        }

        /// <summary>
        ///     Fail
        /// </summary>
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("Professor/{id:int}")]
        public IActionResult GetProfessorJson(int id)
        {
            var lecturer = LecturerManagementService.GetLecturer(id);

            if (lecturer != null)
            {
                var model = new ModifyLecturerViewModel(lecturer);
                return Ok(model);
            }

            return BadRequest();
        }

        /// <summary>
        ///     Not tested
        /// </summary>
        [HttpPost("SaveProfessor")]
        public IActionResult SaveProfessorJson(ModifyLecturerViewModel model)
        {
            try
            {
                model.ModifyLecturer();

                return Ok();
            }
            catch (Exception ex)
            {
                return ServerError500(ex.Message);
            }
        }

        /// <summary>
        ///     Not tested
        /// </summary>
        [HttpPost("AddGroup")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public IActionResult AddGroupJson(GroupViewModel model)
        {
            try
            {
                if (!model.CheckGroupName()) return BadRequest();

                model.AddGroup();
                return Ok();
            }
            catch (Exception ex)
            {
                return ServerError500(ex.Message);
            }
        }

        /// <summary>
        ///     Ok
        /// </summary>
        [HttpGet("Group/{id:int}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public IActionResult GetGroupJson(int id)
        {
            var group = GroupManagementService.GetGroup(id);

            if (group != null)
            {
                var model = new GroupViewModel(group);
                return Ok(model);
            }

            return BadRequest();
        }

        /// <summary>
        ///     Not tested
        /// </summary>
        [HttpPost("SaveGroup")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public IActionResult SaveGroupJson(GroupViewModel model)
        {
            try
            {
                model.ModifyGroup();
                return Ok();
            }
            catch (Exception ex)
            {
                return ServerError500(ex.Message);
            }
        }

        /// <summary>
        ///      Ok
        /// </summary>
        [HttpGet("Professors")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public IActionResult GetProfessorsJson()
        {
            var lecturers = LecturerManagementService.GetLecturers();

            var responseModel = lecturers.Select(l => LecturerViewModel.FormLecturers(l, null));

            return Ok(responseModel);
        }

        /// <summary>
        ///     Ok
        /// </summary>
        [HttpGet("Groups")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public IActionResult GetGroupsJson()
        {
            var groups = GroupManagementService.GetGroups();

            var responseModel = groups.Select(l => GroupViewModel.FormGroup(l, null));

            return Ok(responseModel);
        }

        /// <summary>
        ///     Fail
        /// </summary>
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("Subjects/{id:int}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public IActionResult GetSubjectsJson(int id)
        {
            var subjects = SubjectManagementService.GetSubjectsByStudent(id).OrderBy(subject => subject.Name).ToList();
            var student = StudentManagementService.GetStudent(id);

            if (subjects.Count > 0)
            {
                var model = ListSubjectViewModel.FormSubjects(subjects, student.FullName);
                return Ok(model);
            }

            return BadRequest();
        }

        /// <summary>
        ///     Fail due ef Include
        /// </summary>
        [HttpGet("GetResetPasswordModel/{id:int}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public IActionResult GetResetPasswordModelJson(int id)
        {
            try
            {
                var user = UsersManagementService.GetUser(id);

                var resetPassModel = new ResetPasswordViewModel(user);

                return Ok(resetPassModel);
            }
            catch (Exception ex)
            {
                return ServerError500(ex.Message);
            }
        }

        /// <summary>
        ///     Not tested
        /// </summary>
        [HttpPost("ResetPassword")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Conflict)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public IActionResult ResetPasswordJson(ResetPasswordViewModel model)
        {
            var resetResult = model.ResetPassword();

            if (!resetResult) return Conflict();

            return Ok();
        }

        /// <summary>
        ///     Not tested
        /// </summary>
        [HttpDelete("DeleteUser/{id:int}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public IActionResult DeleteUserJson(int id)
        {
            try
            {
                var deleted = UsersManagementService.DeleteUser(id);

                if (deleted) return Ok();
            }
            catch (Exception ex)
            {
                return ServerError500(ex.Message);
            }

            return BadRequest();
        }

        /// <summary>
        ///     Ok
        /// </summary>
        [HttpGet("Attendance/{id:int}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public IActionResult AttendanceJson(int id)
        {
            var user = UsersManagementService.GetUser(id);

            if (user?.Attendance != null)
            {
                var data = user.AttendanceList.GroupBy(e => e.Date)
                    .Select(d => new {day = d.Key.ToString("d"), count = d.Count()});

                return Ok(new {resultMessage = user.FullName, attendance = data});
            }

            return BadRequest();
        }

        /// <summary>
        ///     Ok
        /// </summary>
        [HttpDelete("DeleteStudent/{id:int}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Conflict)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public IActionResult DeleteStudentJson(int id)
        {
            try
            {
                var student = StudentManagementService.GetStudent(id);

                if (student != null)
                {
                    var result = StudentManagementService.DeleteStudent(id);

                    if (result) return Ok();

                    return Conflict();
                }

                return BadRequest();
            }
            catch (Exception ex)
            {
                return ServerError500(ex.Message);
            }
        }

        /// <summary>
        ///     Ok
        /// </summary>
        [HttpDelete("DeleteLecturer/{id:int}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Conflict)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public IActionResult DeleteLecturerJson(int id)
        {
            try
            {
                var lecturer = LecturerManagementService.GetLecturer(id);

                if (lecturer != null)
                {
                    if (lecturer.SubjectLecturers != null && lecturer.SubjectLecturers.Any() &&
                        lecturer.SubjectLecturers.All(e => e.Subject.IsArchive))
                        foreach (var lecturerSubjectLecturer in lecturer.SubjectLecturers)
                            LecturerManagementService.DisjoinOwnerLector(lecturerSubjectLecturer.SubjectId, id);

                    var result = LecturerManagementService.DeleteLecturer(id);

                    if (result) return Ok();

                    return Conflict();
                }

                return BadRequest();
            }
            catch (Exception ex)
            {
                return ServerError500(ex.Message);
            }
        }

        /// <summary>
        ///     Fail
        /// </summary>
        [HttpDelete("DeleteGroup/{id:int}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Conflict)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public IActionResult DeleteGroupJson(int id)
        {
            try
            {
                var group = GroupManagementService.GetGroup(id);
                if (group != null)
                {
                    if (group.Students != null && group.Students.Count > 0) return Conflict();

                    GroupManagementService.DeleteGroup(id);
                    return Ok();
                }

                return BadRequest();
            }
            catch (Exception ex)
            {
                return ServerError500(ex.Message);
            }
        }
    }
}
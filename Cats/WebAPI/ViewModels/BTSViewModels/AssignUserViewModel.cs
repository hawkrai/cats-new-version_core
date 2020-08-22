using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using Application.Core;
using Application.Infrastructure.GroupManagement;
using Application.Infrastructure.LecturerManagement;
using Application.Infrastructure.ProjectManagement;
using Application.Infrastructure.StudentManagement;
using Application.Infrastructure.UserManagement;
using LMP.Data;
using LMP.Data.Infrastructure;
using LMP.Data.Repositories.RepositoryContracts;
using LMP.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebAPI.ViewModels.BTSViewModels
{
    public class AssignUserViewModel : Controller
    {
        private readonly LazyDependency<IGroupManagementService> _groupManagementService =
            new LazyDependency<IGroupManagementService>();

        private readonly LazyDependency<IProjectManagementService> _projectManagementService =
            new LazyDependency<IProjectManagementService>();

        private readonly LazyDependency<IStudentManagementService> _studentManagementService =
            new LazyDependency<IStudentManagementService>();

        private readonly LazyDependency<IStudentsRepository> _studentsRepository =
            new LazyDependency<IStudentsRepository>();

        public AssignUserViewModel()
        {
        }

        public AssignUserViewModel(int id, int projectId)
        {
            ProjectId = projectId;

            if (id == 0) return;

            var projectUser = ProjectManagementService.GetProjectUser(id);
            ProjectId = projectUser.ProjectId;
            RoleId = projectUser.ProjectRoleId;
            UserId = projectUser.UserId;
            Id = projectUser.Id;
        }

        public IGroupManagementService GroupManagementService => _groupManagementService.Value;

        public IProjectManagementService ProjectManagementService => _projectManagementService.Value;

        public IStudentsRepository StudentsRepository => _studentsRepository.Value;

        public IStudentManagementService StudentManagementService => _studentManagementService.Value;

        [Required(ErrorMessage = "Поле Группа обязательно для заполнения")]
        [DisplayName("Группа")]
        public int GroupId { get; set; }

        [Required(ErrorMessage = "Поле ФИО обязательно для заполнения")]
        [DisplayName("ФИО")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Поле Роль обязательно для заполнения")]
        [DisplayName("Роль")]
        public int RoleId { get; set; }

        [Required(ErrorMessage = "Поле ФИО обязательно для заполнения")]
        [DisplayName("ФИО")]
        public int LecturerId { get; set; }

        [Required(ErrorMessage = "Поле Роль обязательно для заполнения")]
        [DisplayName("Роль")]
        public int LecturerRoleId { get; set; }

        public int ProjectId { get; set; }

        public int Id { get; set; }

        public List<Group> GetAssignedGroups(int userId)
        {
            var groups = new LmPlatformRepositoriesContainer().ProjectsRepository.GetGroups(userId);

            return groups;
        }

        public IList<SelectListItem> GetGroups()
        {
            var groups = new List<Group>();

            var user = new UsersManagementService().GetUser(/*todo #auth WebSecurity.CurrentUserId*/2);
            groups = user != null ? GetAssignedGroups(/*todo #auth WebSecurity.CurrentUserId*/2) : new GroupManagementService().GetGroups();

            return groups.Select(v => new SelectListItem
            {
                Text = v.Name,
                Value = v.Id.ToString(CultureInfo.InvariantCulture)
            }).ToList();
        }

        public List<SelectListItem> GetStudents(int groupId)
        {
            var students = StudentManagementService.GetGroupStudents(groupId).OrderBy(e => e.FirstName);

            return students.Select(v => new SelectListItem
            {
                Text = v.FullName,
                Value = v.Id.ToString(CultureInfo.InvariantCulture)
            }).ToList();
        }

        public IList<SelectListItem> GetLecturers()
        {
            var lecturers = new LecturerManagementService().GetLecturers();

            var lecturerList = new List<Lecturer>();

            foreach (var lecturer in lecturers)
            {
                if (ProjectManagementService.IsUserAssignedOnProject(lecturer.Id, ProjectId) == false)
                {
                    lecturerList.Add(lecturer);
                }
            }

            return lecturerList.Select(v => new SelectListItem
            {
                Text = v.LastName + " " + v.FirstName + " " + v.MiddleName,
                Value = v.Id.ToString(CultureInfo.InvariantCulture)
            }).ToList();
        }

        public IList<SelectListItem> GetRoles()
        {
            var roles = new LmPlatformModelsContext().ProjectRoles.ToList();
            return roles.Select(e => new SelectListItem
            {
                Text = e.Name,
                Value = e.Id.ToString(CultureInfo.InvariantCulture)
            }).ToList();
        }

        public void SaveAssignment()
        {
            var projectUser = new ProjectUser
            {
                UserId = UserId,
                ProjectId = Id,
                ProjectRoleId = RoleId
            };

            ProjectManagementService.AssingRole(projectUser);
        }
    }
}
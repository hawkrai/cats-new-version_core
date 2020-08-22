using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using Application.Core;
using Application.Infrastructure.AccountManagement;
using Application.Infrastructure.GroupManagement;
using Application.Infrastructure.LecturerManagement;
using Application.Infrastructure.StudentManagement;
using Application.Infrastructure.UserManagement;
using Application.SearchEngine.SearchMethods;
using FluentValidation;
using LMP.Data.Repositories.RepositoryContracts;
using LMP.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebAPI.ViewModels.AccountViewModels
{
	public class RegisterViewModel
	{
		private readonly LazyDependency<IAccountManagementService> _accountRegistrationService =
			new LazyDependency<IAccountManagementService>();

		private readonly LazyDependency<IGroupManagementService> _groupManagementService =
			new LazyDependency<IGroupManagementService>();

		private readonly LazyDependency<ILecturerManagementService> _lecturerManagementService =
			new LazyDependency<ILecturerManagementService>();

		private readonly LazyDependency<IStudentManagementService> _studentManagementService =
			new LazyDependency<IStudentManagementService>();

		private readonly LazyDependency<IStudentsRepository> _studentsRepository =
			new LazyDependency<IStudentsRepository>();

		private readonly LazyDependency<IUsersManagementService> _usersManagementService =
			new LazyDependency<IUsersManagementService>();

		private IStudentsRepository StudentsRepository => _studentsRepository.Value;

		private IStudentManagementService StudentManagementService => _studentManagementService.Value;

		private ILecturerManagementService LecturerManagementService => _lecturerManagementService.Value;

		private IGroupManagementService GroupManagementService => _groupManagementService.Value;

		private IAccountManagementService AccountRegistrationService => _accountRegistrationService.Value;

		private IUsersManagementService UsersManagementService => _usersManagementService.Value;

		public string Name { get; set; }

		public string Surname { get; set; }

        public string Patronymic { get; set; }

		public bool IsSecretary { get; set; }

		public bool IsLecturerHasGraduateStudents { get; set; }

		public string UserName { get; set; }

		public string Password { get; set; }

		public string ConfirmPassword { get; set; }

		public string Group { get; set; }

		public string Code { get; set; }

		public IList<SelectListItem> GetGroups()
		{
			var groups = GroupManagementService.GetGroups();

			return groups.Select(v => new SelectListItem
			{
				Text = v.Name,
				Value = v.Id.ToString(CultureInfo.InvariantCulture)
			}).OrderBy(e => e.Text).ToList();
		}

		public void RegistrationUser(IList<string> roles)
		{
			AccountRegistrationService.CreateAccount(UserName, Password, roles);
			if (roles.Contains("student")) SaveStudent();

			if (roles.Contains("lector")) SaveLecturer();
		}

		private void SaveStudent()
		{
			var user = UsersManagementService.GetUser(UserName);
			var student = StudentManagementService.Save(new Student
			{
				Id = user.Id,
				FirstName = Name,
				LastName = Surname,
				MiddleName = Patronymic,
				GroupId = int.Parse(Group),
				Confirmed = false
			});
			student.User = user;
			student.Group = GroupManagementService.GetGroup(student.GroupId);
			new StudentSearchMethod().AddToIndex(student);
		}

		private void SaveLecturer()
		{
			var user = UsersManagementService.GetUser(UserName);

			var lecturer = LecturerManagementService.Save(
				new Lecturer
				{
					Id = user.Id,
					FirstName = Name,
					LastName = Surname,
					MiddleName = Patronymic,
					IsSecretary = IsSecretary,
					IsLecturerHasGraduateStudents = IsLecturerHasGraduateStudents,
					IsActive = true
				});

			lecturer.User = user;

			new LecturerSearchMethod().AddToIndex(lecturer);
		}
	}

    public class RegisterViewModelValidation : AbstractValidator<RegisterViewModel>
    {
        public RegisterViewModelValidation()
        {
            this.RuleFor(m => m.Name)
                .MaximumLength(50)
                .NotEmpty();

            this.RuleFor(m => m.Surname)
                .MaximumLength(50)
                .NotEmpty();

            this.RuleFor(m => m.Patronymic)
                .MaximumLength(50);

            this.RuleFor(m => m.UserName)
                .NotNull();

            this.RuleFor(m => m.Password)
                .MaximumLength(100)
                .MinimumLength(6)
                .NotNull();

            this.RuleFor(m => m.Password)
                .NotNull()
                .Equal(m => m.Password);
        }
    }
}
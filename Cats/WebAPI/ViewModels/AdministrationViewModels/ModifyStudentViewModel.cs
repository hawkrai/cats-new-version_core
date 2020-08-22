using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using Application.Core;
using Application.Infrastructure.GroupManagement;
using Application.Infrastructure.StudentManagement;
using FluentValidation;
using LMP.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebAPI.ViewModels.AdministrationViewModels
{
    public class ModifyStudentViewModel
    {
        private readonly LazyDependency<IGroupManagementService> _groupManagementService =
            new LazyDependency<IGroupManagementService>();

        private readonly LazyDependency<IStudentManagementService> _studentManagementService =
            new LazyDependency<IStudentManagementService>();

        private IStudentManagementService StudentManagementService => _studentManagementService.Value;

        private IGroupManagementService GroupManagementService => _groupManagementService.Value;

        public ModifyStudentViewModel()
        {
        }

        public ModifyStudentViewModel(Student student)
        {
            if (student == null) return;

            Group = student.GroupId == 0
                ? StudentManagementService.GetStudent(student.Id).GroupId.ToString(CultureInfo.InvariantCulture)
                : student.GroupId.ToString(CultureInfo.InvariantCulture);

            Id = student.Id;
            Name = student.FirstName;
            Surname = student.LastName;
            Patronymic = student.MiddleName;
            Email = student.Email;
            UserName = student.User.UserName;
            Avatar = student.User.Avatar;
            SkypeContact = student.User.SkypeContact;
            Phone = student.User.Phone;
            About = student.User.About;
            Email = student.User.Email;
        }

        public string Avatar { get; set; }

        public string Name { get; set; }

        public string Surname { get; set; }

        public string Patronymic { get; set; }

        public string UserName { get; set; }

        public bool IsPasswordReset { get; set; }

        public string Password { get; set; }

        public string ConfirmPassword { get; set; }

        public string Group { get; set; }

        public string Email { get; set; }

        public int Id { get; set; }

        public string FullName => $"{Name} {Surname} ({UserName})";

        public string SkypeContact { get; set; }

        public string Phone { get; set; }

        public string About { get; set; }

        public IList<SelectListItem> GetGroups()
        {
            var groups = GroupManagementService.GetGroups();

            return groups.Select(v => new SelectListItem
            {
                Text = v.Name,
                Value = v.Id.ToString(CultureInfo.InvariantCulture)
            }).ToList();
        }

        public void ModifyStudent()
        {
            var groupId = int.Parse(Group);

            StudentManagementService.UpdateStudent(
                new Student
                {
                    Id = Id,
                    FirstName = Name,
                    LastName = Surname,
                    MiddleName = Patronymic,
                    Email = Email,
                    Confirmed = true,
                    GroupId = groupId,
                    Group = GroupManagementService.GetGroup(groupId),
                    User = new User
                    {
                        Avatar = Avatar,
                        UserName = UserName,
                        About = About,
                        SkypeContact = SkypeContact,
                        Phone = Phone,
                        Email = Email,
                        Id = Id
                    }
                });
        }

        public bool ResetPassword()
        {
            //todo #
            //var token = WebSecurity.GeneratePasswordResetToken(UserName, 1);
            //var result = WebSecurity.ResetPassword(token, Password);
            //return result;
            return true;
        }
    }

    public class ModifyStudentViewModelValidator : AbstractValidator<ModifyStudentViewModel>
    {
        public ModifyStudentViewModelValidator()
        {
            this.RuleFor(u => u.Name)
                .MaximumLength(50)
                .NotEmpty();

            this.RuleFor(u => u.Surname)
                .MaximumLength(50)
                .NotEmpty();

            this.RuleFor(u => u.Patronymic)
                .MaximumLength(50);

            this.RuleFor(u => u.Password)
                .MaximumLength(100)
                .MinimumLength(6);

            this.RuleFor(u => u.ConfirmPassword)
                .Equal(u => u.Password);
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using Application.Core;
using Application.Core.Data;
using Application.Infrastructure.DPManagement;
using Application.Infrastructure.DTO;
using Application.Infrastructure.GroupManagement;
using Application.Infrastructure.LecturerManagement;
using Application.Infrastructure.SubjectManagement;
using FluentValidation;
using LMP.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebAPI.ViewModels.AdministrationViewModels
{
    public class ModifyLecturerViewModel
    {
        private readonly LazyDependency<ICorrelationService> _correlationService =
            new LazyDependency<ICorrelationService>();

        private readonly LazyDependency<IGroupManagementService> _groupManagementService =
            new LazyDependency<IGroupManagementService>();

        private readonly LazyDependency<ILecturerManagementService> _lecturerManagementService =
            new LazyDependency<ILecturerManagementService>();

        private readonly LazyDependency<ISubjectManagementService> _subjectManagementService =
            new LazyDependency<ISubjectManagementService>();

        public ModifyLecturerViewModel()
        {
            Groups = new MultiSelectList(new List<Correlation>(CorrelationService.GetCorrelation("Group", null)), "Id",
                "Name");
        }

        public ModifyLecturerViewModel(Lecturer lecturer)
        {
            if (lecturer != null)
            {
                LecturerId = lecturer.Id;
                Name = lecturer.FirstName;
                Skill = lecturer.Skill;
                Surname = lecturer.LastName;
                Patronymic = lecturer.MiddleName;
                UserName = lecturer.User.UserName;
                Avatar = lecturer.User.Avatar;
                SkypeContact = lecturer.User.SkypeContact;
                Phone = lecturer.User.Phone;
                About = lecturer.User.About;
                Email = lecturer.User.Email;

                IsSecretary = lecturer.IsSecretary;
                IsLecturerHasGraduateStudents = lecturer.IsLecturerHasGraduateStudents;
                IsActive = lecturer.IsActive;

                var groups = CorrelationService.GetCorrelation("Group", lecturer.Id);
                if (lecturer.SecretaryGroups != null)
                    Groups = new MultiSelectList(groups, "Id", "Name",
                        lecturer.SecretaryGroups.Select(x => x.Id).ToList());
            }
        }

        private ILecturerManagementService LecturerManagementService => _lecturerManagementService.Value;

        private ISubjectManagementService SubjectManagementService => _subjectManagementService.Value;

        private IGroupManagementService GroupManagementService => _groupManagementService.Value;

        private ICorrelationService CorrelationService => _correlationService.Value;

        public int LecturerId { get; set; }

        public string Avatar { get; set; }

        public string SkypeContact { get; set; }

        public string Phone { get; set; }

        public bool IsActive { get; set; }

        public string Skill { get; set; }

        public string About { get; set; }

        public string Name { get; set; }

        public string Surname { get; set; }

        public string Patronymic { get; set; }

        public string UserName { get; set; }

        public bool IsPasswordReset { get; set; }

        public string Password { get; set; }

        public string ConfirmPassword { get; set; }

        public string Email { get; set; }

        public string FullName => $"{Surname} {Name} {Patronymic}";

        public bool IsSecretary { get; set; }

        public List<int> SelectedGroupIds { get; set; }

        public MultiSelectList Groups { get; set; }

        public bool IsLecturerHasGraduateStudents { get; set; }

        public IList<SelectListItem> GetSubjects()
        {
            return null;
        }

        public void ModifyLecturer()
        {
            var selectedGroups = SelectedGroupIds != null && SelectedGroupIds.Count > 0
                ? GroupManagementService.GetGroups(new Query<Group>(x => SelectedGroupIds.Contains(x.Id)))
                : new List<Group>();

            foreach (var group in GroupManagementService.GetGroups(new Query<Group>(x => x.SecretaryId == LecturerId)))
            {
                group.SecretaryId = null;
                GroupManagementService.UpdateGroup(group);
            }

            if (IsSecretary)
                foreach (var group in selectedGroups)
                {
                    group.SecretaryId = LecturerId;
                    GroupManagementService.UpdateGroup(group);
                }

            LecturerManagementService.UpdateLecturer(new Lecturer
            {
                Id = LecturerId,
                FirstName = Name,
                LastName = Surname,
                Skill = Skill,
                MiddleName = Patronymic,
                IsSecretary = IsSecretary,
                IsLecturerHasGraduateStudents = IsLecturerHasGraduateStudents,
                SecretaryGroups = selectedGroups,
                IsActive = IsActive,
                User = new User
                {
                    Id = LecturerId,
                    Avatar = Avatar,
                    UserName = UserName,
                    About = About,
                    SkypeContact = SkypeContact,
                    Phone = Phone,
                    Email = Email
                }
            });
        }
    }

    public class ModifyLecturerViewModelValidator : AbstractValidator<ModifyLecturerViewModel>
    {
        public ModifyLecturerViewModelValidator()
        {
            this.RuleFor(u => u.Patronymic)
                .MaximumLength(50);

            this.RuleFor(u => u.ConfirmPassword)
                .Equal(u => u.Password);

            this.RuleFor(u => u.Password)
                .MaximumLength(100)
                .MinimumLength(6);

            this.RuleFor(u => u.Name)
                .MaximumLength(50)
                .NotEmpty();

            this.RuleFor(u => u.Surname)
                .MaximumLength(50)
                .NotEmpty();
        }
    }
}
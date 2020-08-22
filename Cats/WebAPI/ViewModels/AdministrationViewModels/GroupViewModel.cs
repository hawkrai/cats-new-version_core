using System;
using System.Collections.Generic;
using System.Linq;
using Application.Core;
using Application.Core.UI.HtmlHelpers;
using Application.Infrastructure.GroupManagement;
using FluentValidation;
using LMP.Models;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebAPI.ViewModels.AdministrationViewModels
{
    public class GroupViewModel : BaseNumberedGridItem
    {
        private readonly LazyDependency<IGroupManagementService> _groupManagementService = new LazyDependency<IGroupManagementService>();

        private IGroupManagementService GroupManagementService => _groupManagementService.Value;

        public string Name { get; set; }

        public string StartYear { get; set; }

        public string GraduationYear { get; set; }

        public int StudentsCount { get; set; }

        public HtmlString HtmlLinks { get; set; }

        public int Id { get; set; }

        public GroupViewModel()
        {
        }

        public GroupViewModel(Group group)
        {
            Id = group.Id;
            Name = group.Name;
            StartYear = group.StartYear;
            GraduationYear = group.GraduationYear;
        }

        public IList<SelectListItem> GetYears()
        {
            var actualYear = DateTime.Now.Year;
            var yearsList = new List<SelectListItem>();

            for (var year = actualYear - 10; year < actualYear + 10; year++)
            {
                yearsList.Add(new SelectListItem()
                    {
                        Text = year.ToString(),
                        Value = year.ToString(),
                    });
            }

            return yearsList;
        }

        public static GroupViewModel FormGroup(Group group, string htmlLinks)
        {
            return new GroupViewModel
            {
                Id = group.Id,
                Name = group.Name,
                StudentsCount = group.Students.Count(),
                StartYear = group.StartYear,
                GraduationYear = group.GraduationYear,
                HtmlLinks = new HtmlString(htmlLinks)
            };
        }

        public void AddGroup()
        {
            GroupManagementService.AddGroup(GetGroupFromViewModel());
        }

        public void ModifyGroup()
        {
            GroupManagementService.UpdateGroup(new Group()
            {
                Id = Id,
                Name = Name,
                GraduationYear = GraduationYear,
                StartYear = StartYear
            });
        }

        public bool CheckGroupName()
        {
            var group = GroupManagementService.GetGroupByName(this.Name);
            return @group == null;
        }

        private Group GetGroupFromViewModel()
        {
            return new Group()
              {
                  Name = Name,
                  GraduationYear = GraduationYear,
                  StartYear = StartYear
              };
        }
    }

    public class GroupViewModelValidator : AbstractValidator<GroupViewModel>
    {
        public GroupViewModelValidator()
        {
            this.RuleFor(m => m.StartYear)
                .NotEmpty();

            this.RuleFor(m => m.GraduationYear)
                .NotEmpty()
                .GreaterThan(m => m.StartYear);

            this.RuleFor(m => m.Name)
                .MaximumLength(10)
                .NotEmpty();
        }
    }
}

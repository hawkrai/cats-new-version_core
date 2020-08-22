using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Application.Core;
using Application.Core.Data;
using Application.Infrastructure.GroupManagement;
using Application.Infrastructure.SubjectManagement;
using LMP.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebAPI.ViewModels.SubjectModulesViewModel.ModulesViewModel;

namespace WebAPI.ViewModels.SubjectModulesViewModel
{
    public class ModulesScheduleProtectionLabsViewModel : ModulesBaseViewModel
    {
        private readonly LazyDependency<IGroupManagementService> _groupManagementService =
            new LazyDependency<IGroupManagementService>();

        private readonly LazyDependency<ISubjectManagementService> _subjectManagementService =
            new LazyDependency<ISubjectManagementService>();

        public ModulesScheduleProtectionLabsViewModel(int subjectId, Module module)
            : base(subjectId, module)
        {
            var subject = SubjectManagementService.GetSubject(subjectId);
            var firstOrDefault = subject.SubjectGroups.FirstOrDefault();
            var defaultOr = firstOrDefault?.SubGroups.FirstOrDefault();
            if (defaultOr != null) SubGroupId = defaultOr.Id.ToString(CultureInfo.InvariantCulture);

            var subjectGroup = subject.SubjectGroups.FirstOrDefault();
            if (subjectGroup != null) GroupId = subjectGroup.GroupId.ToString(CultureInfo.InvariantCulture);

            ScheduleProtectionLabs = subject.Labs
                .Select(e => new ScheduleProtectionLabsDataViewModel(e, int.Parse(SubGroupId))).ToList();

            var groups = GroupManagementService.GetGroups(
                new Query<Group>(e => e.SubjectGroups.Any(x => x.SubjectId == subjectId)).Include(e => e.Students));
            FillGroupsList(groups);
        }

        public ISubjectManagementService SubjectManagementService => _subjectManagementService.Value;

        public IGroupManagementService GroupManagementService => _groupManagementService.Value;

        public IList<ScheduleProtectionLabsDataViewModel> ScheduleProtectionLabs { get; set; }

        public string GroupId { get; set; }

        public string SubGroupId { get; set; }

        public List<SelectListItem> GroupsList { get; set; }

        private void FillGroupsList(IEnumerable<Group> groups)
        {
            GroupsList = new List<SelectListItem>();
            GroupsList = groups.Select(e => new SelectListItem
            {
                Selected = false,
                Value = e.Id.ToString(CultureInfo.InvariantCulture),
                Text = e.Name
            }).ToList();

            if (GroupsList.Any() && GroupsList != null) GroupId = GroupsList.First().Value;
        }
    }
}
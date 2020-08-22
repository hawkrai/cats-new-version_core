using System.Collections.Generic;
using System.Linq;
using Application.Core;
using Application.Core.Data;
using Application.Infrastructure.SubjectManagement;
using LMP.Models;

namespace WebAPI.ViewModels.SubjectViewModels
{
    public class SubjectWorkingViewModel
    {
        private readonly LazyDependency<IModulesManagementService> _modulesManagementService =
            new LazyDependency<IModulesManagementService>();

        private readonly LazyDependency<ISubjectManagementService> _subjectManagementService =
            new LazyDependency<ISubjectManagementService>();

        public SubjectWorkingViewModel(int subjectId)
        {
            SubjectId = subjectId;
            Subject = SubjectManagementService.GetSubject(new Query<Subject>(e => e.Id == subjectId)
                .Include(e => e.SubjectModules.Select(x => x.Module))
                .Include(e => e.SubjectLecturers.Select(x => x.Lecturer.User)));
            SubjectName = Subject.Name;
            Modules = Subject.SubjectModules.OrderBy(e => e.Module.Order).Select(e => new ModulesViewModel(e.Module))
                .ToList();
            NotVisibleModules = ModulesManagementService.GetModules().Where(e => !e.Visible)
                .Select(e => new ModulesViewModel(e)).ToList();
        }

        public ISubjectManagementService SubjectManagementService => _subjectManagementService.Value;

        public IModulesManagementService ModulesManagementService => _modulesManagementService.Value;

        public IList<ModulesViewModel> Modules { get; set; }

        public IList<ModulesViewModel> NotVisibleModules { get; set; }

        public int SubjectId { get; set; }

        public string SubjectName { get; set; }

        public Subject Subject { get; set; }

        public SubGroupEditingViewModel SubGroups => new SubGroupEditingViewModel(SubjectId);

        public SubGroupEditingViewModel SubGroup(int groupId)
        {
            return new SubGroupEditingViewModel(SubjectId, groupId);
        }
    }
}
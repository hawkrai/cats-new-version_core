using System.Collections.Generic;
using System.Linq;
using Application.Core;
using Application.Infrastructure.SubjectManagement;
using LMP.Models;
using WebAPI.ViewModels.SubjectModulesViewModel.ModulesViewModel;

namespace WebAPI.ViewModels.SubjectModulesViewModel
{
    public class ModulesLecturesViewModel : ModulesBaseViewModel
    {
        private readonly LazyDependency<ISubjectManagementService> _subjectManagementService =
            new LazyDependency<ISubjectManagementService>();

        public ModulesLecturesViewModel(int subjectId, Module module) : base(subjectId, module)
        {
            LecturesData =
                SubjectManagementService.GetSubject(subjectId).Lectures.Select(e => new LecturesDataViewModel(e))
                    .ToList();
        }

        public ISubjectManagementService SubjectManagementService => _subjectManagementService.Value;

        public IList<LecturesDataViewModel> LecturesData { get; set; }
    }
}
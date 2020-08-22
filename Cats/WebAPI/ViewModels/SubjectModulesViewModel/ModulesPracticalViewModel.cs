using System.Collections.Generic;
using System.Linq;
using Application.Core;
using Application.Infrastructure.SubjectManagement;
using LMP.Models;
using WebAPI.ViewModels.SubjectModulesViewModel.ModulesViewModel;

namespace WebAPI.ViewModels.SubjectModulesViewModel
{
    public class ModulesPracticalViewModel : ModulesBaseViewModel
    {
        private readonly LazyDependency<ISubjectManagementService> subjectManagementService =
            new LazyDependency<ISubjectManagementService>();

        public ModulesPracticalViewModel(int subjectId, Module module)
            : base(subjectId, module)
        {
            PracticalsData =
                SubjectManagementService.GetSubject(subjectId).Practicals.Select(e => new PracticalsDataViewModel(e))
                    .ToList();
        }

        public ISubjectManagementService SubjectManagementService => subjectManagementService.Value;

        public IList<PracticalsDataViewModel> PracticalsData { get; set; }
    }
}
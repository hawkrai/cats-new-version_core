using System.Collections.Generic;
using System.Linq;
using Application.Core;
using Application.Infrastructure.SubjectManagement;
using LMP.Models;
using WebAPI.ViewModels.SubjectModulesViewModel.ModulesViewModel;

namespace WebAPI.ViewModels.SubjectModulesViewModel
{
    public class ModulesLabsViewModel : ModulesBaseViewModel
    {
        private readonly LazyDependency<ISubjectManagementService> subjectManagementService =
            new LazyDependency<ISubjectManagementService>();

        public ModulesLabsViewModel(int subjectId, Module module)
            : base(subjectId, module)
        {
            LabsData =
                SubjectManagementService.GetSubject(subjectId).Labs.Select(e => new LabsDataViewModel(e)).ToList();
        }

        public ISubjectManagementService SubjectManagementService => subjectManagementService.Value;

        public IList<LabsDataViewModel> LabsData { get; set; }
    }
}
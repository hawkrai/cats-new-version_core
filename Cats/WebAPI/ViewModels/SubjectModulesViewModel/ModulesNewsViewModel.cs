using System.Collections.Generic;
using System.Linq;
using Application.Core;
using Application.Infrastructure.SubjectManagement;
using LMP.Models;
using WebAPI.ViewModels.SubjectModulesViewModel.ModulesViewModel;

namespace WebAPI.ViewModels.SubjectModulesViewModel
{
    public class ModulesNewsViewModel : ModulesBaseViewModel
    {
        private readonly LazyDependency<ISubjectManagementService> _subjectManagementService =
            new LazyDependency<ISubjectManagementService>();

        public ModulesNewsViewModel(int subjectId, Module module) : base(subjectId, module)
        {
            News = SubjectManagementService.GetSubject(subjectId).SubjectNewses.Select(e => new NewsDataViewModel(e))
                .ToList();
        }

        public ISubjectManagementService SubjectManagementService => _subjectManagementService.Value;

        public List<NewsDataViewModel> News { get; set; }
    }
}
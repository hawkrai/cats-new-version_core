using System.Collections.Generic;
using System.Linq;
using Application.Core;
using Application.Infrastructure.SubjectManagement;

namespace WebAPI.ViewModels.SubjectViewModels
{
    public class SubjectManagementViewModel
    {
        private readonly LazyDependency<ISubjectManagementService> _subjectManagementService =
            new LazyDependency<ISubjectManagementService>();

        public SubjectManagementViewModel(int modelId)
        {
            var model = SubjectManagementService.GetSubject(modelId);
            SubjectName = model.Name;
        }

        public SubjectManagementViewModel(string userId)
        {
            Subjects = SubjectManagementService.GetUserSubjects(int.Parse(userId)).Where(e => !e.IsArchive)
                .Select(e => new SubjectViewModel(e)).ToList();

            //Subjects = SubjectManagementService.GetSubjectsByLectorOwner(int.Parse(userId)).Where(e => !e.IsArchive).Select(e => new SubjectViewModel(e)).ToList();
        }

        public ISubjectManagementService SubjectManagementService => _subjectManagementService.Value;

        public List<SubjectModuleListIetmViewModel> NavList { get; set; }

        public string SubjectName { get; set; }

        public List<SubjectViewModel> Subjects { get; set; }
    }
}
using System.Collections.Generic;
using System.Linq;
using Application.Core;
using Application.Infrastructure.StudentManagement;
using Application.Infrastructure.SubjectManagement;
using LMP.Models;

namespace WebAPI.ViewModels.ParentalViewModels
{
    public class ParentalViewModel
    {
        private readonly LazyDependency<IStudentManagementService> _studentManagementService =
            new LazyDependency<IStudentManagementService>();

        private readonly LazyDependency<ISubjectManagementService> _subjectManagementService =
            new LazyDependency<ISubjectManagementService>();

        public ParentalViewModel()
        {
        }

        public ParentalViewModel(Group group)
        {
            Group = group;
            Subjects = SubjectManagementService.GetGroupSubjects(group.Id);
            Students = StudentManagementService.GetGroupStudents(group.Id).ToList();
        }

        public IStudentManagementService StudentManagementService => _studentManagementService.Value;

        public ISubjectManagementService SubjectManagementService => _subjectManagementService.Value;

        public Group Group { get; set; }

        public List<Student> Students { get; set; }

        public List<Subject> Subjects { get; set; }
    }
}
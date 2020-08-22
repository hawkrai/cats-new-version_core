using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using Application.Core;
using Application.Core.Data;
using Application.Infrastructure.CPManagement;
using Application.Infrastructure.FoldersManagement;
using Application.Infrastructure.GroupManagement;
using Application.Infrastructure.KnowledgeTestsManagement;
using Application.Infrastructure.MaterialsManagement;
using Application.Infrastructure.StudentManagement;
using Application.Infrastructure.SubjectManagement;
using LMP.Data.Infrastructure;
using LMP.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace WebAPI.ViewModels.SubjectViewModels
{
    public class SubjectEditViewModel
    {
        private readonly LazyDependency<ICPManagementService> _cpManagementService =
            new LazyDependency<ICPManagementService>();

        private readonly LazyDependency<IFoldersManagementService> _foldersManagementService =
            new LazyDependency<IFoldersManagementService>();

        private readonly LazyDependency<IGroupManagementService> _groupManagementService =
            new LazyDependency<IGroupManagementService>();

        private readonly LazyDependency<IMaterialsManagementService> _materialsManagementService =
            new LazyDependency<IMaterialsManagementService>();

        private readonly LazyDependency<IModulesManagementService> _modulesManagementService =
            new LazyDependency<IModulesManagementService>();

        private readonly LazyDependency<IStudentManagementService> _studentManagementService =
            new LazyDependency<IStudentManagementService>();

        private readonly LazyDependency<ISubjectManagementService> _subjectManagementService =
            new LazyDependency<ISubjectManagementService>();

        private readonly LazyDependency<ITestsManagementService> _testsManagementService =
            new LazyDependency<ITestsManagementService>();

        private readonly LazyDependency<ICpContext> context = new LazyDependency<ICpContext>();

        public SubjectEditViewModel()
        {
            Modules = new Collection<ModulesViewModel>();
        }

        public SubjectEditViewModel(int subjectId)
        {
            SubjectId = subjectId;
            Color = "#ffffff";
            Title = SubjectId == 0 ? "Создание предмета" : "Редактирование предмета";
            Modules = ModulesManagementService.GetModules().Where(e => e.Visible).Select(e => new ModulesViewModel(e))
                .ToList();
            FillSubjectGroups();
            if (subjectId != 0)
            {
                var subject = SubjectManagementService.GetSubject(subjectId);
                Color = subject.Color;
                SubjectId = subjectId;
                ShortName = subject.ShortName;
                DisplayName = subject.Name;
                foreach (var module in Modules)
                    if (subject.SubjectModules.Any(e => e.ModuleId == module.ModuleId))
                        module.Checked = true;

                SelectedGroups = new List<int>();
                foreach (var group in Groups)
                {
                    var groupId = int.Parse(group.Value);
                    if (subject.SubjectGroups.Any(e => e.GroupId == groupId && e.IsActiveOnCurrentGroup))
                        SelectedGroups.Add(groupId);
                }
            }
        }

        private IStudentManagementService StudentManagementService => _studentManagementService.Value;

        private ITestsManagementService TestsManagementService => _testsManagementService.Value;

        private ICPManagementService CPManagementService => _cpManagementService.Value;

        private ICpContext Context => context.Value;

        public IGroupManagementService GroupManagementService => _groupManagementService.Value;

        public ISubjectManagementService SubjectManagementService => _subjectManagementService.Value;

        public IModulesManagementService ModulesManagementService => _modulesManagementService.Value;

        public IFoldersManagementService FoldersManagementService => _foldersManagementService.Value;

        public IMaterialsManagementService MaterialsManagementService => _materialsManagementService.Value;

        public ICollection<ModulesViewModel> Modules { get; set; }

        //todo # [SubjectName]
        [Required(ErrorMessage = "Название предмета не может быть пустым")]
        [Display(Name = "Название предмета",
            Description = "Введите название предмета. Название будет использоваться при отображени предмета.")]
        public string DisplayName { get; set; }

        //todo # [SubjectShortName]
        [Required(ErrorMessage = "Аббревиатура не может быть пустой")]
        [Display(Name = "Аббревиатура",
            Description = "Аббревиатура представляет собой сокрощенное название предмета. Например: Базы данных - БД")]
        public string ShortName { get; set; }

        public string Title { get; set; }

        public List<SelectListItem> Groups { get; set; }

        public int SubjectId { get; set; }

        public string Color { get; set; }

        public List<int> SelectedGroups { get; set; }

        private void FillSubjectGroups()
        {
            var groups = GroupManagementService.GetGroups();
            Groups = groups.Select(e => new SelectListItem
            {
                Text = e.Name,
                Value = e.Id.ToString(CultureInfo.InvariantCulture),
                Selected = false
            }).ToList();
        }

        public void Save(int userId, string color)
        {
            var subject = new Subject
            {
                Id = SubjectId,
                Name = DisplayName,
                ShortName = ShortName,
                Color = color,
                SubjectModules = new Collection<SubjectModule>(),
                SubjectLecturers = new Collection<SubjectLecturer>()
            };

            foreach (var module in Modules)
                if (module.Checked)
                {
                    if (module.Type == ModuleType.Labs)
                    {
                        subject.SubjectModules.Add(new SubjectModule
                        {
                            ModuleId = ModulesManagementService.GetModules()
                                .First(e => e.ModuleType == ModuleType.ScheduleProtection).Id,
                            SubjectId = SubjectId
                        });
                        subject.SubjectModules.Add(new SubjectModule
                        {
                            ModuleId = ModulesManagementService.GetModules()
                                .First(e => e.ModuleType == ModuleType.StatisticsVisits).Id,
                            SubjectId = SubjectId
                        });
                        subject.SubjectModules.Add(new SubjectModule
                        {
                            ModuleId = ModulesManagementService.GetModules()
                                .First(e => e.ModuleType == ModuleType.Results).Id,
                            SubjectId = SubjectId
                        });
                    }

                    subject.SubjectModules.Add(new SubjectModule
                    {
                        ModuleId = module.ModuleId,
                        SubjectId = SubjectId
                    });
                }

            subject.SubjectLecturers.Add(new SubjectLecturer
            {
                SubjectId = SubjectId,
                LecturerId = userId
            });

            var selectedGroupdsOld = new List<SubjectGroup>();

            if (SubjectId != 0)
                selectedGroupdsOld = SubjectManagementService
                    .GetSubject(new Query<Subject>(e => e.Id == SubjectId).Include(e => e.SubjectGroups)).SubjectGroups
                    .ToList();

            var oldGroupIds = selectedGroupdsOld.Select(x => x.GroupId);

            if (SelectedGroups != null)
            {
                UpdateNewAssignedGroups(Modules.Where(e => e.Checked),
                    SelectedGroups.Where(e => !oldGroupIds.Contains(e)).ToList());
                subject.SubjectGroups = SelectedGroups.Select(e => new SubjectGroup
                {
                    GroupId = e,
                    SubjectId = SubjectId,
                    IsActiveOnCurrentGroup = true
                }).ToList();
            }
            else
            {
                subject.SubjectGroups = new Collection<SubjectGroup>();
            }

            foreach (var subjectSubjectGroup in selectedGroupdsOld)
            {
                if (subject.SubjectGroups.Any(e => e.GroupId == subjectSubjectGroup.GroupId)) continue;
                
                TestsManagementService.UnlockAllTestForGroup(subjectSubjectGroup.GroupId);
                SubjectManagementService.DeleteNonReceivedUserFiles(subjectSubjectGroup.GroupId);
            }

            var acp = Context.AssignedCourseProjects.Include("Student")
                .Where(x => x.CourseProject.SubjectId == subject.Id);

            foreach (var a in acp)
            {
                var flag = subject.SubjectGroups.Any(s => s.GroupId == a.Student.GroupId);

                if (!flag) Context.AssignedCourseProjects.Remove(a);
                CPManagementService.DeletePercenageAndVisitStatsForUser(a.StudentId);
            }

            var cpg = Context.CourseProjectGroups.Where(x => x.CourseProject.SubjectId == subject.Id);

            foreach (var a in cpg)
            {
                var flag = subject.SubjectGroups.Any(s => s.GroupId == a.GroupId);

                if (!flag) Context.CourseProjectGroups.Remove(a);
            }

            Context.SaveChanges();

            var sub = SubjectManagementService.SaveSubject(subject);
            SubjectId = sub.Id;
            CreateOrUpdateSubGroups();

            foreach (var module in sub.SubjectModules)
                if (module.ModuleId == 9)
                    MaterialsManagementService.CreateRootFolder(module.Id, sub.Name);
        }

        private void CreateOrUpdateSubGroups()
        {
            var subject = SubjectManagementService.GetSubject(
                new Query<Subject>(e => e.Id == SubjectId).Include(e => e.SubjectGroups.Select(x => x.SubGroups)));
            foreach (var subjectGroup in subject.SubjectGroups)
                if (!subjectGroup.SubGroups.Any())
                {
                    var students = StudentManagementService.GetGroupStudents(subjectGroup.GroupId)
                        .Where(e => e.Confirmed == null || e.Confirmed.Value);
                    SubjectManagementService.SaveSubGroup(SubjectId, subjectGroup.GroupId,
                        students.Select(e => e.Id).ToList(), new List<int>(), new List<int>());
                }
        }

        private void UpdateNewAssignedGroups(IEnumerable<ModulesViewModel> modules, List<int> groupIds)
        {
            if (modules.Any(e => e.Type == ModuleType.YeManagment))
                CPManagementService.SetSelectedGroupsToCourseProjects(SubjectId, groupIds);
        }
    }
}
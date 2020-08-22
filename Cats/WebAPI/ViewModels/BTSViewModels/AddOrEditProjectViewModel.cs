using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Application.Core;
using Application.Infrastructure.ProjectManagement;
using LMP.Data.Repositories.RepositoryContracts;
using LMP.Models;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.ViewModels.BTSViewModels
{
    public class AddOrEditProjectViewModel : Controller
    {
        private readonly LazyDependency<IProjectManagementService> _projectManagementService =
            new LazyDependency<IProjectManagementService>();

        private readonly LazyDependency<IProjectsRepository> _projectsRepository =
            new LazyDependency<IProjectsRepository>();

        public AddOrEditProjectViewModel()
        {
            CreatorId = /*todo #auth WebSecurity.CurrentUserId*/2;
        }

        public AddOrEditProjectViewModel(int projectId)
        {
            ProjectId = projectId;

            if (projectId != 0)
            {
                var project = ProjectManagementService.GetProject(projectId);
                CreatorId = project.CreatorId;
                Details = project.Details;
                DateOfChange = project.DateOfChange;
                ProjectId = project.Id;
                Title = project.Title;
            }
        }

        public IProjectsRepository ProjectsRepository => _projectsRepository.Value;

        public IProjectManagementService ProjectManagementService => _projectManagementService.Value;

        public int ProjectId { get; set; }

        [Required(ErrorMessage = "Поле Тема проекта обязательно для заполнения")]
        [StringLength(300, ErrorMessage = "Тема проекта должна быть не менее 3 символов и не более 300.",
            MinimumLength = 3)]
        [DataType(DataType.Text)]
        [DisplayName("Тема проекта")]
        public string Title { get; set; }

        [StringLength(1000, ErrorMessage = "Описание проекта не должно превышать 1000 символов.")]
        [DataType(DataType.MultilineText)]
        [DisplayName("Описание проекта")]
        public string Details { get; set; }

        [DisplayName("Дата изменения")] 
        public DateTime DateOfChange { get; set; }

        [DisplayName("Создатель")] 
        int CreatorId { get; set; }

        [DisplayName("Создатель")]
        public User Creator { get; set; }

        [DisplayName("Создатель")] 
        public string CreatorName { get; set; }

        public void Update(int projectId)
        {
            ProjectManagementService.UpdateProject(new Project
            {
                Id = projectId,
                Title = Title
            });
        }

        public void Save(int creatorId)
        {
            var project = new Project
            {
                Id = ProjectId,
                Title = Title,
                Details = Details,
                CreatorId = creatorId,
                DateOfChange = DateTime.Today
            };

            ProjectManagementService.SaveProject(project);

            if (ProjectId == 0)
            {
                ProjectManagementService.AssingRole(new ProjectUser
                {
                    UserId = creatorId,
                    ProjectId = project.Id,
                    ProjectRoleId = 3
                });
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using Application.Core;
using Application.Infrastructure.ProjectManagement;
using LMP.Data;
using LMP.Data.Infrastructure;
using LMP.Data.Repositories.RepositoryContracts;
using LMP.Models;
using LMP.Models.BTS;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebAPI.ViewModels.BTSViewModels
{
    public class ProjectsViewModel
    {
        private static List<Bug> _bugs;
        private static List<BugStatus> _statuses;
        private static List<BugSeverity> _severities;

        private readonly LazyDependency<IProjectManagementService> _projectManagementService =
            new LazyDependency<IProjectManagementService>();

        private readonly LazyDependency<IProjectsRepository> _projectsRepository =
            new LazyDependency<IProjectsRepository>();

        public ProjectsViewModel(int projectId)
        {
            CreateStaticLists();
            var model = ProjectManagementService.GetProject(projectId);
            ProjectId = projectId;
            SetParams(model);
        }

        public IProjectsRepository ProjectsRepository => _projectsRepository.Value;

        public IProjectManagementService ProjectManagementService => _projectManagementService.Value;

        public int ProjectId { get; set; }

        [DisplayName("Тема проекта")] 
        public string Title { get; set; }

        [DisplayName("Описание проекта")] 
        public string Details { get; set; }

        [DisplayName("Дата создания")] 
        public DateTime CreationDate { get; set; }

        [DisplayName("Создатель")]
        public int CreatorId { get; set; }

        [DisplayName("Создатель")]
        public User Creator { get; set; }

        [DisplayName("Создатель")] 
        public string CreatorName { get; set; }

        public string CommentText { get; set; }

        public int BugQuentity { get; set; }

        public List<BugQuentity> QuentityOfBugsByType { get; set; }

        public List<BugQuentity> QuentityOfBugsBySeverity { get; set; }

        public string BugStatusesJson { get; set; }

        public void CreateStaticLists()
        {
            _bugs = new List<Bug>();
            _statuses = new List<BugStatus>();
            _severities = new List<BugSeverity>();
        }

        public void SetParams(Project model)
        {
            Title = model.Title;
            CreationDate = model.DateOfChange;
            Details = model.Details ?? string.Empty;
            GetCreatorName(model.CreatorId);
            SetBugStatistics(ProjectId);
        }

        public bool IsProjectManager()
        {
            var projectUser = new ProjectManagementService().GetProjectUsers(ProjectId)
                .Count(e => e.UserId == /*todo #auth WebSecurity.CurrentUserId*/2 && e.ProjectRoleId == 3);

            return projectUser != 0;
        }

        public void GetCreatorName(int id)
        {
            CreatorName = ProjectManagementService.GetCreatorName(id);
        }

        public IList<SelectListItem> GetProjectNames()
        {
            var projects = ProjectManagementService.GetProjects();

            return projects.Select(e => new SelectListItem
            {
                Text = e.Title,
                Value = e.Id.ToString(CultureInfo.InvariantCulture)
            }).ToList();
        }

        public string GetCommentUserName(int userId)
        {
            return new ProjectManagementService().GetCreatorName(userId);
        }

        public List<ProjectComment> GetProjectComments()
        {
            var commentList = ProjectManagementService.GetProjectComments(ProjectId).ToList();
            return commentList;
        }

        public void SetBugStatistics(int id)
        {
            GetProjectBugs(id);
            GetBugPropertyList();
            GetBugQuentity();

            if (BugQuentity != 0)
            {
                GetQuentityOfBugsByType();
                GetQuentityOfBugsBySeverity();

                var dictionary = QuentityOfBugsByType
                    .Where(quentity => quentity.Quentity != 0)
                    .ToDictionary(quentity => quentity.Name, quentity => quentity.Quentity * 100 / BugQuentity);


                BugStatusesJson = JsonSerializer.Serialize(dictionary);
            }
            else
            {
                BugStatusesJson = JsonSerializer.Serialize(" ");
            }
        }

        private void GetBugPropertyList()
        {
            var context = new LmPlatformModelsContext();
            _statuses = context.BugStatuses.ToList();
            _severities = context.BugSeverities.ToList();
        }

        private int GetStatusIdByName(string name)
        {
            var context = new LmPlatformModelsContext();
            var statuses = context.BugStatuses.ToList();
            var id = 0;

            foreach (var n in statuses.Where(n => n.Name == name))
            {
                id = n.Id;
            }

            return id;
        }

        private int GetSeverityIdByName(string name)
        {
            var context = new LmPlatformModelsContext();
            var severities = context.BugSeverities.ToList();
            var id = 0;

            foreach (var n in severities.Where(n => n.Name == name))
            {
                id = n.Id;
            }

            return id;
        }

        private int GetQuentity(int n)
        {
            return _bugs.Count(b => b.StatusId == GetStatusIdByName(_statuses[n].Name));
        }

        private int GetQuentityBySeverity(int n)
        {
            return _bugs.Count(b => b.SeverityId == GetSeverityIdByName(_severities[n].Name));
        }

        private void GetQuentityOfBugsByType()
        {
            QuentityOfBugsByType = new List<BugQuentity>();

            for (var i = 0; i < _statuses.Count; i++)
            {
                QuentityOfBugsByType.Add(new BugQuentity
                {
                    Name = _statuses[i].Name,
                    Quentity = GetQuentity(i)
                });
            }
        }

        public void GetQuentityOfBugsBySeverity()
        {
            QuentityOfBugsBySeverity = new List<BugQuentity>();

            for (var i = 0; i < _severities.Count; i++)
            {
                QuentityOfBugsBySeverity.Add(new BugQuentity
                {
                    Name = _severities[i].Name,
                    Quentity = GetQuentityBySeverity(i)
                });
            }
        }

        private void GetBugQuentity()
        {
            BugQuentity = _bugs?.Count() ?? 0;
        }

        public void GetProjectBugs(int id)
        {
            var context = new LmPlatformRepositoriesContainer().BugsRepository.GetAll();
            foreach (var b in context)
                if (b.ProjectId == id)
                    _bugs.Add(b);
        }

        public void SaveComment(string comment)
        {
            var currentUserId = /*todo #auth WebSecurity.CurrentUserId*/2;
            var newComment = new ProjectComment
            {
                CommentText = comment,
                ProjectId = ProjectId,
                UserId = currentUserId,
                CommentingDate = DateTime.Now
            };
            ProjectManagementService.SaveComment(newComment);
        }
    }
}
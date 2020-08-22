using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using Application.Core;
using Application.Infrastructure.BugManagement;
using Application.Infrastructure.ProjectManagement;
using Application.Infrastructure.UserManagement;
using LMP.Data.Infrastructure;
using LMP.Data.Repositories.RepositoryContracts;
using LMP.Models;
using LMP.Models.BTS;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebAPI.ViewModels.BTSViewModels
{
    public class AddOrEditBugViewModel : Controller
    {
        private readonly LazyDependency<IBugManagementService> _bugManagementService =
            new LazyDependency<IBugManagementService>();

        private readonly LazyDependency<IBugsRepository> _bugsRepository = new LazyDependency<IBugsRepository>();

        public AddOrEditBugViewModel()
        {
            CreatorId = /*todo #auth WebSecurity.CurrentUserId*/2;
        }

        public AddOrEditBugViewModel(int bugId)
        {
            BugId = bugId;
            SavedAssignedDeveloperId = AssignedDeveloperId;

            if (bugId != 0)
            {
                var bug = BugManagementService.GetBug(bugId);
                BugId = bugId;
                CreatorId = bug.ReporterId;
                ProjectId = bug.ProjectId;
                SeverityId = bug.SeverityId;
                StatusId = bug.StatusId;
                SymptomId = bug.SymptomId;
                Steps = bug.Steps;
                ExpectedResult = bug.ExpectedResult;
                Description = bug.Description;
                Summary = bug.Summary;
                ReportingDate = bug.ReportingDate;
                ModifyingDate = bug.ModifyingDate;
                AssignedDeveloperId = bug.AssignedDeveloperId;
                SavedAssignedDeveloperId = bug.AssignedDeveloperId;
                EditorId = bug.EditorId;
            }
        }

        public IBugsRepository BugsRepository => _bugsRepository.Value;

        public IBugManagementService BugManagementService => _bugManagementService.Value;

        public int BugId { get; set; }

        [Required(ErrorMessage = "Поле Название обязательно для заполнения")]
        [StringLength(100, ErrorMessage = "Название не может иметь размер больше 100 символов")]
        [DataType(DataType.Text)]
        [DisplayName("Название")]
        public string Summary { get; set; }

        [Required(ErrorMessage = "Поле Описание обязательно для заполнения")]
        [DataType(DataType.MultilineText)]
        [DisplayName("Описание")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Поле Шаги выполнения обязательно для заполнения")]
        [DataType(DataType.MultilineText)]
        [DisplayName("Шаги выполнения")]
        public string Steps { get; set; }

        [Required(ErrorMessage = "Поле Ожидаемый результат обязательно для заполнения")]
        [DataType(DataType.MultilineText)]
        [DisplayName("Ожидаемый результат")]
        public string ExpectedResult { get; set; }

        [DisplayName("Симптом")] 
        public int SymptomId { get; set; }

        [DisplayName("Проект")]
        public int ProjectId { get; set; }

        [DisplayName("Важность")] 
        int SeverityId { get; set; }

        [DisplayName("Статус")] 
        public int StatusId { get; set; }

        public int EditorId { get; set; }

        [DisplayName("Назначенный разработчик")]
        public int AssignedDeveloperId { get; set; }

        public int SavedAssignedDeveloperId { get; set; }

        public int CreatorId { get; set; }

        public DateTime ReportingDate { get; set; }

        public DateTime ModifyingDate { get; set; }

        public IList<SelectListItem> GetDeveloperNames()
        {
            var _context = new UsersManagementService();
            var context = new ProjectManagementService();
            var projectUsers = context.GetProjectUsers(ProjectId).ToList().Where(e => e.ProjectRoleId == 1);

            var users = new List<User>();

            var currProjectUser =
                context.GetProjectUsers(ProjectId).Single(e => e.UserId == /*todo #auth WebSecurity.CurrentUserId*/2);
            if (currProjectUser.ProjectRoleId == 1)
            {
                users.Add(_context.GetUser(currProjectUser.UserId));
            }
            else
            {
                users.AddRange(projectUsers.Select(user => _context.GetUser(user.UserId)));
            }

            return users.Select(e => new SelectListItem
            {
                Text = context.GetCreatorName(e.Id),
                Value = e.Id.ToString(CultureInfo.InvariantCulture)
            }).ToList();
        }

        public IList<SelectListItem> GetStatusNames()
        {
            var allStatuses = new LmPlatformModelsContext().BugStatuses.ToList();
            var statuses = new List<BugStatus>();

            var userRoleOnProject =
                new ProjectManagementService().GetProjectsOfUser(/*todo #auth WebSecurity.CurrentUserId*/2)
                    .Single(e => e.ProjectId == ProjectId).ProjectRoleId;

            switch (userRoleOnProject)
            {
                //Роль "Разработчик"
                case 1:
                    switch (StatusId)
                    {
                        //Статус "Обнаружена"
                        case 1:
                            statuses.Add(allStatuses[0]);
                            statuses.Add(allStatuses[1]);
                            statuses.Add(allStatuses[5]);
                            break;

                        //Статус "Назначена"
                        case 2:
                            statuses.Add(allStatuses[1]);
                            statuses.Add(allStatuses[2]);
                            statuses.Add(allStatuses[5]);
                            break;

                        //Статус "Исправлена"
                        case 3:
                            statuses.Add(allStatuses[2]);
                            break;

                        //Статус "Проверена"
                        case 4:
                            statuses.Add(allStatuses[3]);
                            statuses.Add(allStatuses[6]);
                            break;

                        //Статус "Отложена"
                        case 5:
                            statuses.Add(allStatuses[4]);
                            break;

                        //Статус "Отклонена"
                        case 6:
                            statuses.Add(allStatuses[5]);
                            statuses.Add(allStatuses[6]);
                            break;

                        //Статус "Открыта заново"
                        case 8:
                            if (AssignedDeveloperId == 0) statuses.Add(allStatuses[1]);

                            statuses.Add(allStatuses[7]);
                            break;
                    }

                    break;

                //Роль "Тестировщик"
                case 2:
                    switch (StatusId)
                    {
                        //Статус "Обнаружена"
                        case 1:
                            statuses.Add(allStatuses[0]);
                            statuses.Add(allStatuses[1]);
                            statuses.Add(allStatuses[4]);
                            statuses.Add(allStatuses[5]);
                            break;

                        //Статус "Назначена"
                        case 2:
                            statuses.Add(allStatuses[1]);
                            break;

                        //Статус "Исправлена"
                        case 3:
                            statuses.Add(allStatuses[2]);
                            statuses.Add(allStatuses[3]);
                            statuses.Add(allStatuses[4]);
                            statuses.Add(allStatuses[7]);
                            break;

                        //Статус "Проверена"
                        case 4:
                            statuses.Add(allStatuses[3]);
                            break;

                        //Статус "Отложена"
                        case 5:
                            if (AssignedDeveloperId != 0) statuses.Add(allStatuses[2]);

                            statuses.Add(allStatuses[4]);
                            statuses.Add(allStatuses[5]);
                            statuses.Add(allStatuses[7]);
                            break;

                        //Статус "Отклонена"
                        case 6:
                            statuses.Add(allStatuses[5]);
                            statuses.Add(allStatuses[7]);
                            break;

                        //Статус "Открыта заново"
                        case 8:
                            if (AssignedDeveloperId != 0) statuses.Add(allStatuses[2]);

                            statuses.Add(allStatuses[4]);
                            statuses.Add(allStatuses[5]);
                            statuses.Add(allStatuses[7]);
                            break;
                    }

                    break;

                //Роль "Руководитель проекта"
                case 3:
                    switch (StatusId)
                    {
                        //Статус "Обнаружена"
                        case 1:
                            statuses.Add(allStatuses[0]);
                            statuses.Add(allStatuses[1]);
                            statuses.Add(allStatuses[4]);
                            statuses.Add(allStatuses[5]);
                            break;

                        //Статус "Назначена"
                        case 2:
                            statuses.Add(allStatuses[1]);
                            break;

                        //Статус "Исправлена"
                        case 3:
                            statuses.Add(allStatuses[2]);
                            statuses.Add(allStatuses[4]);
                            break;

                        //Статус "Проверена"
                        case 4:
                            statuses.Add(allStatuses[3]);
                            statuses.Add(allStatuses[6]);
                            break;

                        //Статус "Отложена"
                        case 5:
                            if (AssignedDeveloperId != 0) statuses.Add(allStatuses[2]);

                            statuses.Add(allStatuses[4]);
                            statuses.Add(allStatuses[5]);
                            statuses.Add(allStatuses[7]);
                            break;

                        //Статус "Отклонена"
                        case 6:
                            statuses.Add(allStatuses[5]);
                            statuses.Add(allStatuses[6]);
                            statuses.Add(allStatuses[7]);
                            break;

                        //Статус "Открыта заново"
                        case 8:
                            statuses.Add(allStatuses[1]);
                            statuses.Add(allStatuses[4]);
                            statuses.Add(allStatuses[5]);
                            statuses.Add(allStatuses[7]);
                            break;
                    }

                    break;
            }

            return statuses.Select(e => new SelectListItem
            {
                Text = e.Name,
                Value = e.Id.ToString(CultureInfo.InvariantCulture)
            }).ToList();
        }

        public IList<SelectListItem> GetSeverityNames()
        {
            var severities = new LmPlatformModelsContext().BugSeverities.ToList();
            return severities.Select(e => new SelectListItem
            {
                Text = e.Name,
                Value = e.Id.ToString(CultureInfo.InvariantCulture)
            }).ToList();
        }

        public IList<SelectListItem> GetSymptomNames()
        {
            var symptoms = new LmPlatformModelsContext().BugSymptoms.ToList();
            return symptoms.Select(e => new SelectListItem
            {
                Text = e.Name,
                Value = e.Id.ToString(CultureInfo.InvariantCulture)
            }).ToList();
        }

        public IList<SelectListItem> GetProjectNames()
        {
            var projectUsers = new ProjectManagementService().GetProjectsOfUser(/*todo #auth WebSecurity.CurrentUserId*/2);
            var projects = projectUsers.Select(projectUser => new ProjectManagementService().GetProject(projectUser.ProjectId)).ToList();

            return projects.Select(e => new SelectListItem
            {
                Text = e.Title,
                Value = e.Id.ToString(CultureInfo.InvariantCulture)
            }).OrderBy(e => e.Text).ToList();
        }

        public void Save(int reporterId, int projectId)
        {
            if (projectId != 0) ProjectId = projectId;

            var bug = new Bug
            {
                Id = BugId,
                ReporterId = CreatorId,
                ProjectId = ProjectId,
                SeverityId = SeverityId,
                StatusId = StatusId,
                SymptomId = SymptomId,
                Steps = Steps,
                ExpectedResult = ExpectedResult,
                Description = Description,
                Summary = Summary,
                ModifyingDate = DateTime.Today,
                EditorId = reporterId,
                AssignedDeveloperId = AssignedDeveloperId
            };

            if (BugId == 0)
            {
                bug.ReportingDate = DateTime.Today;
                bug.StatusId = 1;
                bug.AssignedDeveloperId = 0;
                bug.ReporterId = /*todo #auth WebSecurity.CurrentUserId*/2;
            }
            else
            {
                bug.ReportingDate = BugManagementService.GetBug(BugId).ReportingDate;
                if (StatusId != 2 && SavedAssignedDeveloperId != AssignedDeveloperId)
                    bug.AssignedDeveloperId = SavedAssignedDeveloperId;
            }

            if (bug.StatusId == 1 || bug.StatusId == 8) bug.AssignedDeveloperId = 0;

            BugManagementService.SaveBug(bug);
        }

        public void SaveBugLog(BugLog bugLog)
        {
            new BugManagementService().SaveBugLog(bugLog);
        }
    }
}
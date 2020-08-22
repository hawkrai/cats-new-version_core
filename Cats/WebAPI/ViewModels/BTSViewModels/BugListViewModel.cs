using System.ComponentModel;
using Application.Core.UI.HtmlHelpers;
using Application.Infrastructure.ProjectManagement;
using LMP.Data.Infrastructure;
using Microsoft.AspNetCore.Html;

namespace WebAPI.ViewModels.BTSViewModels
{
    public class BugListViewModel : BaseNumberedGridItem
    {
        private static LmPlatformModelsContext context = new LmPlatformModelsContext();

        public BugListViewModel()
        {
        }

        public BugListViewModel(int id)
        {
            CurrentProjectId = id;
            CurrentProjectName = string.Empty;

            if (id == 0) return;

            var project = new ProjectManagementService().GetProject(id);
            CurrentProjectName = project.Title;
        }

        [DisplayName("ID")]
        public int Id { get; set; }

        [DisplayName("Название")]
        public string Summary { get; set; }

        [DisplayName("Важность")] 
        public string Severity { get; set; }

        [DisplayName("Статус")]
        public string Status { get; set; }

        [DisplayName("Назначенный разработчик")]
        public string AssignedDeveloperName { get; set; }

        [DisplayName("Проект")]
        public string Project { get; set; }

        [DisplayName("Дата последнего изменения")]
        public string ModifyingDate { get; set; }

        [DisplayName("Действие")]
        public HtmlString Action { get; set; }

        public string Steps { get; set; }

        public string Symptom { get; set; }

        public string ReporterName { get; set; }

        public string ReportingDate { get; set; }

        public string AssignedDeveloperId { get; set; }

        public int ProjectId { get; set; }

        public int StatusId { get; set; }

        public int CurrentProjectId { get; set; }

        public string CurrentProjectName { get; set; }

        public bool IsAssigned { get; set; }
    }
}
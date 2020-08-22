using Application.Infrastructure.ProjectManagement;
using LMP.Models;

namespace WebAPI.Controllers.Services.Models.BTS
{
    public class BugViewData
    {
        public BugViewData(Bug bug)
        {
            Id = bug.Id;
            Summary = bug.Summary;
            Severity = bug.Severity.Name;
            Status = bug.Status.Name;
            AssignedDeveloper = GetAssignedDeveloper(bug);
            ProjectTitle = bug.Project.Title;
            ModifyingDate = bug.ModifyingDate.ToShortDateString();
            Reporter = bug.Reporter.FullName;
        }

        public int Id { get; set; }

        public string Summary { get; set; }

        public string Severity { get; set; }

        public string Status { get; set; }

        public string AssignedDeveloper { get; set; }

        public string ProjectTitle { get; set; }

        public string ModifyingDate { get; set; }

        public string Reporter { get; set; }

        private string GetAssignedDeveloper(Bug bug)
        {
            if (bug.AssignedDeveloperId == 0)
                return "отсутствует";
            //TODO: Make developers abble to be loaded in repository
            return new ProjectManagementService().GetCreatorName(bug.AssignedDeveloperId);
        }
    }
}
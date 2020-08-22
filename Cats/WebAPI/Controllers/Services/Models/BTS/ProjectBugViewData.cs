using LMP.Models;

namespace WebAPI.Controllers.Services.Models.BTS
{
    public class ProjectBugViewData
    {
        public ProjectBugViewData(Bug bug)
        {
            Id = bug.Id;
            Status = bug.Status.Name;
            Severity = bug.Severity.Name;
        }

        public int Id { get; set; }

        public string Severity { get; set; }

        public string Status { get; set; }
    }
}
using System.Collections.Generic;
using LMP.Models;

namespace WebAPI.Controllers.Services.Models.BTS
{
    public class ProjectViewData
    {
        public ProjectViewData(Project project, bool extended = true, bool withBugs = false, bool withMembers = false)
        {
            Id = project.Id;
            Title = project.Title;
            CreatorName = project.Creator.FullName;
            if (extended)
            {
                Description = project.Details;
                DateOfChange = project.DateOfChange.ToShortDateString();
                UserQuentity = project.ProjectUsers.Count;
            }

            if (withMembers)
            {
                Members = new List<ProjectMemberViewData>();
                foreach (var projectUser in project.ProjectUsers) Members.Add(new ProjectMemberViewData(projectUser));
            }

            if (!withBugs) return;

            Bugs = new List<ProjectBugViewData>();
            foreach (var bug in project.Bugs) Bugs.Add(new ProjectBugViewData(bug));
        }

        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string DateOfChange { get; set; }

        public string CreatorName { get; set; }

        public int UserQuentity { get; set; }

        public List<ProjectMemberViewData> Members { get; set; }

        public List<ProjectBugViewData> Bugs { get; set; }
    }
}
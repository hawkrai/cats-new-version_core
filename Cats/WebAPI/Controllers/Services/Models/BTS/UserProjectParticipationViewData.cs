using System.Linq;
using LMP.Models;

namespace WebAPI.Controllers.Services.Models.BTS
{
    public class UserProjectParticipationViewData
    {
        public UserProjectParticipationViewData(Project project, int userId)
        {
            ProjectId = project.Id;
            Title = project.Title;
            CreatorName = project.Creator.FullName;
            UserRole = project.ProjectUsers.First(e => e.UserId == userId).ProjectRole.Name;
        }

        public int ProjectId { get; set; }

        public string Title { get; set; }

        public string CreatorName { get; set; }

        public string UserRole { get; set; }
    }
}
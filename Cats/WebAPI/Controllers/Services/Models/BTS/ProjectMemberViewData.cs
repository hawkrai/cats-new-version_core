using LMP.Models;

namespace WebAPI.Controllers.Services.Models.BTS
{
    public class ProjectMemberViewData
    {
        public ProjectMemberViewData(ProjectUser projectUser)
        {
            Id = projectUser.Id;
            UserId = projectUser.User.Id;
            Name = projectUser.User.FullName;
            Role = projectUser.ProjectRole.Name;
        }

        public int Id { get; set; }

        public int UserId { get; set; }

        public string Name { get; set; }

        public string Role { get; set; }
    }
}
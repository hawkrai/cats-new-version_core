using LMP.Models.BTS;

namespace WebAPI.Controllers.Services.Models.BTS
{
    public class ProjectMatrixRequirementViewData
    {
        public ProjectMatrixRequirementViewData(ProjectMatrixRequirement requirement)
        {
            Id = requirement.Id;
            Number = requirement.Number;
            Name = requirement.Name;
            Covered = requirement.Covered;
        }

        public int Id { get; set; }

        public string Number { get; set; }

        public string Name { get; set; }

        public bool Covered { get; set; }
    }
}
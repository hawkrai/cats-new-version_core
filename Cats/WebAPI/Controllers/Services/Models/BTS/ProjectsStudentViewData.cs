using System.Collections.Generic;
using LMP.Models;

namespace WebAPI.Controllers.Services.Models.BTS
{
    public class StudentParticipationViewData
    {
        public StudentParticipationViewData(Student student)
        {
            Id = student.Id;
            FullName = student.FullName;
            Participations = new List<UserProjectParticipationViewData>();
            foreach (var projectUser in student.User.ProjectUsers)
                Participations.Add(new UserProjectParticipationViewData(projectUser.Project, student.Id));
        }

        public int Id { get; set; }

        public string FullName { get; set; }

        public List<UserProjectParticipationViewData> Participations { get; set; }
    }
}
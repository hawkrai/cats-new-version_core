using System.Linq;
using LMP.Models;

namespace WebAPI.ViewModels.ParentalViewModels
{
    public class PlanViewModel : ParentalViewModel
    {
        public PlanViewModel(Group group, int subjectId)
            : base(group)
        {
            PlanSubject = Subjects.First(s => s.Id == subjectId);
        }

        public Subject PlanSubject { get; set; }
    }
}
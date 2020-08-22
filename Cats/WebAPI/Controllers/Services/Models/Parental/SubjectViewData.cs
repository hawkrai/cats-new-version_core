using LMP.Models;

namespace WebAPI.Controllers.Services.Models.Parental
{
    public class SubjectViewData
    {
        public SubjectViewData()
        {
        }

        public SubjectViewData(Subject subject)
        {
            Id = subject.Id;
            Name = subject.Name;
            ShortName = subject.ShortName;
            IsNeededCopyToBts = subject.IsNeededCopyToBts;
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public string ShortName { get; set; }

        public bool IsNeededCopyToBts { get; set; }
    }
}
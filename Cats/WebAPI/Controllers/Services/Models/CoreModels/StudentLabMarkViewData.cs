namespace WebAPI.Controllers.Services.Models.CoreModels
{
    public class StudentLabMarkViewData
    {
        public int LabId { get; set; }

        public int StudentId { get; set; }

        public string Mark { get; set; }

        public int StudentLabMarkId { get; set; }

        public string Comment { get; set; }

        public string Date { get; set; }
    }
}
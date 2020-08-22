using System.Collections.Generic;

namespace WebAPI.Controllers.Services.Models.Lectures
{
    public class StudentMarkForDateResult : ResultViewData
    {
        public string Date { get; set; }

        public int DateId { get; set; }

        public List<StudentMarkForDateViewData> StudentMarkForDate { get; set; }
    }
}
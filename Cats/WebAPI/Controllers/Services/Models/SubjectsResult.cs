using System.Collections.Generic;
using WebAPI.Controllers.Services.Models.Parental;

namespace WebAPI.Controllers.Services.Models
{
    public class SubjectsResult : ResultViewData
    {
        public List<SubjectViewData> Subjects { get; set; }
    }
}
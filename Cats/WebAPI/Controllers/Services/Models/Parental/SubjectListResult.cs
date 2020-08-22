using System.Collections.Generic;

namespace WebAPI.Controllers.Services.Models.Parental
{
    public class SubjectListResult : ResultViewData
    {
        public List<SubjectViewData> Subjects { get; set; }
    }
}
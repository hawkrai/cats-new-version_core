using System.Collections.Generic;

namespace WebAPI.Controllers.Services.Models.CoreModels
{
    public class StudentsMarksResult : ResultViewData
    {
        public List<StudentMark> Students { get; set; }
    }
}
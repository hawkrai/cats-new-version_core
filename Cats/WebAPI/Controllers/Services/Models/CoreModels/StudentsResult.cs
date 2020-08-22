using System.Collections.Generic;

namespace WebAPI.Controllers.Services.Models.CoreModels
{
    public class StudentsResult : ResultViewData
    {
        public List<StudentsViewData> Students { get; set; }
    }
}
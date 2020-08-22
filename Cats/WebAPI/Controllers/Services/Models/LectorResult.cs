using System.Collections.Generic;

namespace WebAPI.Controllers.Services.Models
{
    public class LectorResult : ResultViewData
    {
        public List<LectorViewData> Lectors { get; set; }
    }
}
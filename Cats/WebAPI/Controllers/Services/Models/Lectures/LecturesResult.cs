using System.Collections.Generic;

namespace WebAPI.Controllers.Services.Models.Lectures
{
    public class LecturesResult : ResultViewData
    {
        public List<LecturesViewData> Lectures { get; set; }
    }
}
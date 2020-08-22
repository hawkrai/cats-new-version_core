using System.Collections.Generic;

namespace WebAPI.Controllers.Services.Models.Practicals
{
    public class PracticalsResult : ResultViewData
    {
        public List<PracticalsViewData> Practicals { get; set; }
    }
}
using System.Collections.Generic;

namespace WebAPI.Controllers.Services.Models.BTS
{
    public class BugsResult : ResultViewData
    {
        public List<BugViewData> Bugs { get; set; }

        public int TotalCount { get; set; }
    }
}
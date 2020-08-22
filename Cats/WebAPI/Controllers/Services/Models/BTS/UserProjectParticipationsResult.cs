using System.Collections.Generic;

namespace WebAPI.Controllers.Services.Models.BTS
{
    public class UserProjectParticipationsResult : ResultViewData
    {
        public List<UserProjectParticipationViewData> Projects { get; set; }

        public int TotalCount { get; set; }
    }
}
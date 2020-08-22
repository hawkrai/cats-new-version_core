using System.Collections.Generic;

namespace WebAPI.Controllers.Services.Models.CoreModels
{
    public class GroupsResult : ResultViewData
    {
        public List<GroupsViewData> Groups { get; set; }
    }
}
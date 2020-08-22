using System.Collections.Generic;

namespace WebAPI.Controllers.Services.Models.Labs
{
    public class UserLabFilesResult : ResultViewData
    {
        public List<UserlabFilesViewData> UserLabFiles { get; set; }
    }
}
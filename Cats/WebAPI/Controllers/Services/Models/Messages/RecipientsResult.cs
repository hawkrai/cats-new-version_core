using System.Collections.Generic;

namespace WebAPI.Controllers.Services.Models.Messages
{
    public class RecipientsResult : ResultViewData
    {
        public List<RecipientViewData> Recipients { get; set; }
    }
}
using System.Collections.Generic;

namespace WebAPI.Controllers.Services.Models.Messages
{
    public class MessagesResult : ResultViewData
    {
        public List<MessagesViewData> InboxMessages { get; set; }

        public List<MessagesViewData> OutboxMessages { get; set; }
    }
}
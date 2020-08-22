using System.Collections.Generic;
using System.Globalization;
using LMP.Models;

namespace WebAPI.Controllers.Services.Models.Messages
{
    public class DisplayMessageViewData : MessagesViewData
    {
        public DisplayMessageViewData(UserMessages userMessage)
            : base(userMessage)
        {
            Body = userMessage.Message.Text;
            Attachments = userMessage.Message.Attachments;
            Date = userMessage.Date.ToString("F", new CultureInfo("ru-RU"));
        }

        public string Body { get; set; }

        public IEnumerable<Attachment> Attachments { get; set; }
    }
}
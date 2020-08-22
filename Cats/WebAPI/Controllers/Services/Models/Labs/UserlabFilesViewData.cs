using System.Collections.Generic;
using LMP.Models;

namespace WebAPI.Controllers.Services.Models.Labs
{
    public class UserlabFilesViewData
    {
        public int Id { get; set; }

        public string Comments { get; set; }

        public string PathFile { get; set; }

        public string Date { get; set; }

        public List<Attachment> Attachments { get; set; }

        public bool IsReceived { get; set; }

        public bool IsReturned { get; set; }

        public bool IsCoursProject { get; set; }
    }
}
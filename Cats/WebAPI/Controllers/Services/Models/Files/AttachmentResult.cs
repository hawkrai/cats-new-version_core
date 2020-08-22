using System.Collections.Generic;
using LMP.Models;

namespace WebAPI.Controllers.Services.Models.Files
{
    public class AttachmentResult : ResultViewData
    {
        public List<Attachment> Files { get; set; }

        public string ServerPath { get; set; }
    }
}
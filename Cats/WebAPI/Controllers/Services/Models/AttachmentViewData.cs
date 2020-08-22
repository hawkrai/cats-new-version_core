using LMP.Models;

namespace WebAPI.Controllers.Services.Models
{
    public class AttachmentViewData
    {
        public string Name { get; set; }

        public string FileName { get; set; }

        public string PathName { get; set; }

        public AttachmentType AttachmentType { get; set; }
    }
}
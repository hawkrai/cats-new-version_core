using LMP.Models;

namespace WebAPI.Controllers.Services.Models.BTS
{
    public class ProjectFileViewData
    {
        public ProjectFileViewData(Attachment attachment)
        {
            Id = attachment.Id;
            Name = attachment.Name;
            FileName = attachment.FileName;
            PathName = attachment.PathName;
            AttachmentType = attachment.AttachmentType.ToString();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public string AttachmentType { get; set; }

        public string FileName { get; set; }

        public string PathName { get; set; }
    }
}
using System.Collections.Generic;
using LMP.Models;

namespace WebAPI.ViewModels.SubjectViewModels
{
    public class SubjectAttachmentsViewModel
    {
        public List<Attachment> Lectures { get; set; }

        public List<Attachment> Labs { get; set; }

        public List<Attachment> Practicals { get; set; }
    }
}
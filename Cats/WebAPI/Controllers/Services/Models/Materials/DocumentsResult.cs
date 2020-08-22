using System.Collections.Generic;

namespace WebAPI.Controllers.Services.Models.Materials
{
    public class DocumentsResult : ResultViewData
    {
        public DocumentsViewData Document { get; set; }

        public List<DocumentsViewData> Documents { get; set; }
    }
}
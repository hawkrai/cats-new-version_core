using System.Collections.Generic;

namespace WebAPI.Controllers.Services.Models.Materials
{
    public class DocumentsViewData
    {
        private List<LMP.Models.Materials> dc;

        public DocumentsViewData(LMP.Models.Materials materials)
        {
            Id = materials.Id;
            Name = materials.Name;
            Text = materials.Text;
        }

        public DocumentsViewData(List<LMP.Models.Materials> dc)
        {
            // TODO: Complete member initialization
            this.dc = dc;
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public string Text { get; set; }
    }
}
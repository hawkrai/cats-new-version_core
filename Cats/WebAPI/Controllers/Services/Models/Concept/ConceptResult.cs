using System.Collections.Generic;

namespace WebAPI.Controllers.Services.Models.Concept
{
    public class ConceptResult : ResultViewData
    {
        public ConceptViewData Concept { get; set; }

        public List<ConceptViewData> Concepts { get; set; }

        public IEnumerable<ConceptViewData> Children { get; set; }

        public string SubjectName { get; set; }
    }
}
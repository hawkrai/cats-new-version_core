using System.Collections.Generic;
using LMP.Models;

namespace Application.Infrastructure.CTO
{
    public class SubjectData
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string ShortName { get; set; }

        public bool IsNeededCopyToBts { get; set; }

        public IEnumerable<Group> Groups { get; set; }
    }
}
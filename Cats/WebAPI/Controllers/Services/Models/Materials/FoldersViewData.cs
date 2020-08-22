using System.Collections.Generic;
using LMP.Models;

namespace WebAPI.Controllers.Services.Models.Materials
{
    public class FoldersViewData
    {
        private List<Folders> fl;

        public FoldersViewData(Folders folders)
        {
            Id = folders.Id;
            Name = folders.Name;
            Pid = folders.Pid;
        }

        public FoldersViewData(List<Folders> fl)
        {
            // TODO: Complete member initialization
            this.fl = fl;
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public int Pid { get; set; }
    }
}
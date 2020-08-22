using System.Collections.Generic;
using WebAPI.Controllers.Services.Models.Labs;

namespace WebAPI.Controllers.Services.Models.CoreModels
{
    public class StudentMark
    {
        public int StudentId { get; set; }

        public string FullName { get; set; }

        public string Login { get; set; }

        public List<StudentLabMarkViewData> Marks { get; set; }

        public List<LabVisitingMarkViewData> LabVisitingMark { get; set; }

        public List<UserlabFilesViewData> FileLabs { get; set; }

        public string LabsMarkTotal { get; set; }

        public string TestMark { get; set; }

        public int SubGroup { get; set; }
    }
}
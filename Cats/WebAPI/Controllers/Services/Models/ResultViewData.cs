using System.Collections.Generic;

namespace WebAPI.Controllers.Services.Models
{
    public class ResultViewData
    {
        public string Message { get; set; }

        public string Code { get; set; }

        public List<ResultPlag> DataD { get; set; }
    }

    public class ResultPSubjectViewData
    {
        public string Message { get; set; }

        public string Code { get; set; }

        public List<ResultPlagSubject> DataD { get; set; }
    }

    public class ResultPlag
    {
        public string doc { get; set; }

        public string coeff { get; set; }

        public string author { get; set; }

        public string subjectName { get; set; }

        public string groupName { get; set; }
    }

    public class ResultPlagSubjectClu
    {
        public ResultPlagSubject[] clusters { get; set; }
    }

    public class ResultPlagSubject
    {
        public string[] docs { get; set; }

        public List<ResultPlag> correctDocs { get; set; }
    }
}
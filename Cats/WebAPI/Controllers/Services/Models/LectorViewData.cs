using LMP.Models;

namespace WebAPI.Controllers.Services.Models
{
    public class LectorViewData
    {
        public LectorViewData(Lecturer lecturer, bool withUsername = false)
        {
            LectorId = lecturer.Id;
            FullName = lecturer.FullName;
            if (withUsername) UserName = lecturer.User.UserName;
        }

        public int LectorId { get; set; }

        public string UserName { get; set; }

        public string FullName { get; set; }
    }
}
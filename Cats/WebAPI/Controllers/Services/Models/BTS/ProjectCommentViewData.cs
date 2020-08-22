using LMP.Models;

namespace WebAPI.Controllers.Services.Models.BTS
{
    public class ProjectCommentViewData
    {
        public ProjectCommentViewData(ProjectComment comment)
        {
            Id = comment.Id;
            UserName = comment.User.FullName;
            Text = comment.CommentText;
            Time = comment.CommentingDate.ToString("HH:mm dd.MM.yyyy");
        }

        public int Id { get; set; }

        public string UserName { get; set; }

        public string Text { get; set; }

        public string Time { get; set; }
    }
}
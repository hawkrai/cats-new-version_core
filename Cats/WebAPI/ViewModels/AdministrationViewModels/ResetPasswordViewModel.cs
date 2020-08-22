using FluentValidation;
using LMP.Models;

namespace WebAPI.ViewModels.AdministrationViewModels
{
    public class ResetPasswordViewModel
    {
        public ResetPasswordViewModel(User user)
        {
            FullName = user.Student != null ? user.Student.FullName : user.Lecturer.FullName;
            Login = user.UserName;
        }

        public ResetPasswordViewModel()
        {
        }

        public string FullName { get; set; }

        public string Login { get; set; }

        public string Password { get; set; }

        public string ConfirmPassword { get; set; }

        public bool ResetPassword()
        {
            //todo #
            //var token = WebSecurity.GeneratePasswordResetToken(Login, 1);
            //var isReseted = WebSecurity.ResetPassword(token, Password);
            //return isReseted;
            return true;
        }
    }

    public class ResetPasswordViewModelValidation : AbstractValidator<ResetPasswordViewModel>
    {
        public ResetPasswordViewModelValidation()
        {
            this.RuleFor(m => m.Password)
                .MaximumLength(100)
                .MinimumLength(6)
                .NotEmpty();

            this.RuleFor(m => m.ConfirmPassword)
                .Equal(m => m.Password);

            this.RuleFor(m => m.FullName);

            this.RuleFor(m => m.Login)
                .NotEmpty();
        }
    }
}
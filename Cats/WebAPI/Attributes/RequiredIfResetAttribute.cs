using System.ComponentModel.DataAnnotations;
using WebAPI.ViewModels.AdministrationViewModels;

namespace WebAPI.Attributes
{
    public class PasswordRequiredIfResetAttribute : ValidationAttribute
    {
        public PasswordRequiredIfResetAttribute()
        {
            ErrorMessage = "Для сброса пароля нужно ввести и подтвердить новый пароль";
        }

        public override bool IsValid(object value)
        {
            if (!(value is ModifyStudentViewModel viewModel) || !viewModel.IsPasswordReset) return true;

            return viewModel.IsPasswordReset && !string.IsNullOrWhiteSpace(viewModel.Password);
        }
    }
}
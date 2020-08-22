using Application.Core;
using Application.Infrastructure.UserManagement;

namespace WebAPI.ViewModels.AdministrationViewModels
{
    public class AttendanceViewModel
    {
        private readonly LazyDependency<IUsersManagementService> _userManagementService =
            new LazyDependency<IUsersManagementService>();

        public AttendanceViewModel()
        {
            InitializeActivity();
        }

        public IUsersManagementService UserManagementService => _userManagementService.Value;

        private void InitializeActivity()
        {
        }
    }
}
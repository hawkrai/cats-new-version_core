using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Application.Core;
using Application.Core.Constants;
using Application.Infrastructure.UserManagement;
using LMP.Models;

namespace WebAPI.ViewModels.AdministrationViewModels
{
    public class UserActivityViewModel
    {
        private readonly LazyDependency<IUsersManagementService> _userManagementService =
            new LazyDependency<IUsersManagementService>();

        private IEnumerable<User> users;

        public UserActivityViewModel()
        {
            InitializeActivity();
        }

        private IUsersManagementService UserManagementService => _userManagementService.Value;

        public string UserActivityJson { get; set; }

        public int TotalUsersCount { get; set; }

        public int TotalStudentsCount { get; set; }

        public int TotalLecturersCount { get; set; }

        public int ServiceAccountsCount { get; set; }

        private IEnumerable<User> Users => users ??= UserManagementService.GetUsers(true);

        private void InitializeActivity()
        {
            TotalUsersCount = Users.Count();

            TotalStudentsCount =
                Users.Count(u =>
                    u.Membership != null &&
                    u.Membership.Roles.Select(r => r.Role.RoleName).Contains(Constants.Roles.Student));
                
            TotalLecturersCount =
                Users.Count(u =>
                    u.Membership != null &&
                    u.Membership.Roles.Select(r => r.Role.RoleName).Contains(Constants.Roles.Lector));

            ServiceAccountsCount =
                Users.Count(u =>
                    u.Membership != null && u.Membership.Roles.Select(r => r.Role.RoleName).Contains(Constants.Roles.Admin));

            var today = DateTime.Now;

            var dayActivity = Users.Count(u =>
                u.LastLogin != null && DateTime.Compare(u.LastLogin.Value, today.AddDays(-1)) >= 0);
            var weekActivity =
                Users.Count(u => u.LastLogin != null && DateTime.Compare(u.LastLogin.Value, today.AddDays(-7)) >= 0) -
                dayActivity;
            var monthActivity =
                Users.Count(u => u.LastLogin != null && DateTime.Compare(u.LastLogin.Value, today.AddMonths(-1)) >= 0) -
                weekActivity - dayActivity;
            var inactive = TotalUsersCount - dayActivity - weekActivity - monthActivity;

            var dictionary = new Dictionary<string, int>
            {
                {"Сутки", dayActivity},
                {"Неделя", weekActivity},
                {"Месяц", monthActivity},
                {"Ранее", inactive}
            };

            UserActivityJson = JsonSerializer.Serialize(dictionary);
        }
    }
}
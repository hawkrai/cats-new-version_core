using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.Infrastructure.AccountManagement
{
    // todo #
	public class AccountManagementService : IAccountManagementService
	{
		public bool Login(string login, string password, bool rememberMe)
		{
			bool result;

			try
            {
                result = true;// WebSecurity.Login(login, password, rememberMe);
			}
			catch (ArgumentException exception)
			{
				result = false;
			}

			return result;
		}

		public void Logout()
		{
			//WebSecurity.Logout();
		}

		public void CreateAccount(string login, string password, IList<string> roles, object properties = null, bool requireConfirmationToken = false)
		{
            //WebSecurity.CreateUserAndAccount(login, password, properties, requireConfirmationToken);
            
			if (roles.Any())
			{
				//Roles.AddUserToRoles(login, roles.ToArray());	
			}
		}

		public bool ChangePassword(string userName, string oldPassword, string newPassword)
        {
            return true; //WebSecurity.ChangePassword(userName, oldPassword, newPassword);
        }

        public bool DeleteAccount(string login)
        {
            var result = true;//((SimpleMembershipProvider)Membership.Provider).DeleteAccount(login);

            try
            {
                result = true; //Membership.Provider.DeleteUser(login, true);
            }
            catch (Exception e)
            {
                return false;
            }

            return result;
	    }
	}
}
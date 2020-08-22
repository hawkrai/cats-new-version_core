using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Application.Core;
using Application.Infrastructure.DTO;
using LMP.Data.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace Application.Infrastructure.DPManagement
{
    public class UserService : IUserService
    {
        private readonly LazyDependency<IDpContext> context = new LazyDependency<IDpContext>();

        private IDpContext Context => context.Value;

        public UserData GetUserInfo(int userId)
        {
	        var query = Context.Users.Include(x => x.Lecturer).Include(x => x.Student);

	        var user = query.SingleOrDefault(x => x.Id == userId);

            return new UserData
            {
                UserId = user?.Id ?? 0,
                IsLecturer = user.Lecturer != null,
                IsStudent = user.Student != null,
                IsSecretary = user.Lecturer != null && user.Lecturer.IsSecretary,
                HasChosenDiplomProject = user.Student != null
                                         && Context.AssignedDiplomProjects.Any(x =>
                                             x.StudentId == user.Student.Id && !x.ApproveDate.HasValue),
                HasAssignedDiplomProject = user.Student != null
                                           && Context.AssignedDiplomProjects.Any(x =>
                                               x.StudentId == user.Student.Id && x.ApproveDate.HasValue)
            };
        }
	}
}
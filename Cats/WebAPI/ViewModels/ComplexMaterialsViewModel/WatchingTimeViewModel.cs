using System.Collections.Generic;
using System.Linq;
using Application.Core;
using Application.Core.Data;
using Application.Infrastructure.ConceptManagement;
using Application.Infrastructure.GroupManagement;
using Application.Infrastructure.UserManagement;
using Application.Infrastructure.WatchingTimeManagement;
using LMP.Models;

namespace WebAPI.ViewModels.ComplexMaterialsViewModel
{
    public class WatchingTimeViewModel
    {
        private readonly LazyDependency<IConceptManagementService> _conceptManagementService =
            new LazyDependency<IConceptManagementService>();

        private readonly LazyDependency<IGroupManagementService> _groupManagementService =
            new LazyDependency<IGroupManagementService>();

        private readonly LazyDependency<IUsersManagementService> _userManagementService =
            new LazyDependency<IUsersManagementService>();

        private readonly LazyDependency<IWatchingTimeService> _watchingTimeService =
            new LazyDependency<IWatchingTimeService>();

        public int conceptId;

        public WatchingTimeViewModel(int conceptId)
        {
            this.conceptId = conceptId;
            var concept = ConceptManagementService.GetById(conceptId);
            groups = GroupManagementService.GetGroups(new Query<Group>(e =>
                e.SubjectGroups.Any(x => x.SubjectId == concept.SubjectId)));
            viewRecords = new List<ViewsWorm>();
            var list = new List<WatchingTime>();
            list = WatchingTimeService.GetAllRecords(conceptId);
            foreach (var item in list)
                viewRecords.Add(new ViewsWorm
                    {Name = UsersManagementService.GetUser(item.UserId).FullName, Seconds = item.Time});
        }

        public List<ViewsWorm> viewRecords { get; set; }
        public List<Group> groups { get; set; }

        public IWatchingTimeService WatchingTimeService => _watchingTimeService.Value;

        public IUsersManagementService UsersManagementService => _userManagementService.Value;

        public IGroupManagementService GroupManagementService => _groupManagementService.Value;

        public IConceptManagementService ConceptManagementService => _conceptManagementService.Value;

        public class ViewsWorm
        {
            public string Name { get; set; }
            public int Seconds { get; set; }
        }
    }
}
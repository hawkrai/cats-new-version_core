using Application.Core;
using Application.Infrastructure.MessageManagement;

namespace WebAPI.Controllers.Services.Models.Messages
{
    public class RecipientViewData
    {
        private readonly LazyDependency<IMessageManagementService> messageManagementService =
            new LazyDependency<IMessageManagementService>();

        public RecipientViewData(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public IMessageManagementService MessageManagementService => messageManagementService.Value;

        public int Id { get; set; }

        public string Name { get; set; }
    }
}
using System.Collections.Generic;
using System.Linq;
using Application.Core;
using Application.Infrastructure.MessageManagement;

namespace WebAPI.ViewModels.MessageViewModels
{
    public class MessageListViewModel
    {
        private readonly LazyDependency<IMessageManagementService> _messageManagementService =
            new LazyDependency<IMessageManagementService>();

        private readonly List<DisplayMessageViewModel> _messages;

        public MessageListViewModel()
        {
            _messages = new List<DisplayMessageViewModel>(GetUserMessages());
        }

        public IMessageManagementService MessageManagementService => _messageManagementService.Value;

        private int UserId => /*todo #auth WebSecurity.CurrentUserId*/2;

        public List<DisplayMessageViewModel> IncomingMessages
        {
            get { return _messages.Where(m => m.AuthorId != UserId).OrderByDescending(m => m.Date).ToList(); }
        }

        public List<DisplayMessageViewModel> OutcomingMessages
        {
            get { return _messages.Where(m => m.AuthorId == UserId).OrderByDescending(m => m.Date).ToList(); }
        }

        public List<DisplayMessageViewModel> UnreadMessages
        {
            get
            {
                return _messages.Where(m => m.AuthorId != UserId && !m.IsRead).OrderByDescending(m => m.Date).ToList();
            }
        }

        private IEnumerable<DisplayMessageViewModel> GetUserMessages()
        {
            var userMessages = MessageManagementService.GetUserMessages(UserId)
                .Select(m => new DisplayMessageViewModel(m)).ToList();

            return userMessages;
        }
    }
}
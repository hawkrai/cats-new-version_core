using System;
using System.Collections.Generic;
using System.Linq;
using Application.Core;
using Application.Infrastructure.FilesManagement;
using Application.Infrastructure.MessageManagement;
using LMP.Models;

namespace WebAPI.ViewModels.MessageViewModels
{
    public class DisplayMessageViewModel
    {
        private readonly LazyDependency<IFilesManagementService> _filesManagementService =
            new LazyDependency<IFilesManagementService>();

        private readonly LazyDependency<IMessageManagementService> _messageManagementService =
            new LazyDependency<IMessageManagementService>();

        private IEnumerable<Attachment> _attachments;

        private IEnumerable<User> _recipients;

        public DisplayMessageViewModel()
        {
        }

        public DisplayMessageViewModel(int userMessagesId)
        {
            var userMessages = MessageManagementService.GetUserMessage(userMessagesId);

            InitFields(userMessages);
        }

        public DisplayMessageViewModel(UserMessages userMessages)
        {
            InitFields(userMessages);
        }

        public IMessageManagementService MessageManagementService => _messageManagementService.Value;

        public IFilesManagementService FilesManagementService => _filesManagementService.Value;

        public bool HasAttachments { get; set; }

        public string AuthorName { get; set; }

        public string Subject { get; set; }

        public string PreviewText => !string.IsNullOrEmpty(Text) ? Text.Substring(0, Math.Min(Text.Length, 100)) : Text;

        public string Date => DateTime.ToString(DateTime.Date == DateTime.Now.Date ? "t" : "d");

        public string Text { get; set; }

        public DateTime DateTime { get; set; }

        public bool IsRead { get; set; }

        public int MessageId { get; set; }

        public int UserMessageId { get; set; }

        public int AuthorId { get; set; }

        public IEnumerable<User> Recipients => _recipients ??= MessageManagementService.GetMessageRecipients(MessageId);

        public IEnumerable<Attachment> Attachments
        {
            get
            {
                if (_attachments != null) return _attachments;

                var message = MessageManagementService.GetMessage(MessageId);
                if (message != null) _attachments = MessageManagementService.GetMessage(MessageId).Attachments;

                return _attachments;
            }
        }

        public static DisplayMessageViewModel FormMessageToDisplay(UserMessages userMessages)
        {
            var model = new DisplayMessageViewModel(userMessages);

            return model;
        }

        private void InitFields(UserMessages userMessages)
        {
            AuthorName = userMessages.Author.FullName;
            DateTime = userMessages.Date;
            Text = userMessages.Message.Text;
            MessageId = userMessages.MessageId;
            UserMessageId = userMessages.Id;
            AuthorId = userMessages.AuthorId;
            IsRead = userMessages.IsRead;
            Subject = userMessages.Message.Subject;
            HasAttachments = userMessages.Message.Attachments.Any();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Application.Core;
using Application.Infrastructure.FilesManagement;
using Application.Infrastructure.MessageManagement;
using LMP.Models;

namespace WebAPI.Controllers.Services.Models.Messages
{
    public class MessagesViewData
    {
        private readonly LazyDependency<IFilesManagementService> filesManagementService =
            new LazyDependency<IFilesManagementService>();

        private readonly LazyDependency<IMessageManagementService> messageManagementService =
            new LazyDependency<IMessageManagementService>();

        public MessagesViewData(UserMessages userMessage)
        {
            Id = userMessage.MessageId;
            AthorName = userMessage.Author.FullName;
            AthorId = userMessage.Author.Id.ToString();
            Subject = userMessage.Message.Subject;
            PreviewText = !string.IsNullOrEmpty(userMessage.Message.Text)
                ? userMessage.Message.Text.Substring(0, Math.Min(userMessage.Message.Text.Length, 100))
                : userMessage.Message.Text;
            IsRead = userMessage.IsRead;
            Date = userMessage.Date.ToString(userMessage.Date.Date == DateTime.Now.Date ? "t" : "d",
                new CultureInfo("ru-RU"));
            Recipients = MessageManagementService.GetMessageRecipients(userMessage.MessageId)
                .Select(e => string.IsNullOrEmpty(e.FullName) ? e.UserName : e.FullName);
            AttachmentsCount = userMessage.Message.Attachments.Any() ? userMessage.Message.Attachments.Count : 0;
        }

        public IFilesManagementService FilesManagementService => filesManagementService.Value;

        public IMessageManagementService MessageManagementService => messageManagementService.Value;

        public string AthorName { get; set; }

        public string AthorId { get; set; }

        public string Subject { get; set; }

        public string PreviewText { get; set; }

        public bool IsRead { get; set; }

        public string Date { get; set; }

        public int Id { get; set; }

        public int AttachmentsCount { get; set; }

        public IEnumerable<string> Recipients { get; set; }
    }
}
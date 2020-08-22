using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Application.Core;
using Application.Core.Extensions;
using Application.Infrastructure.MessageManagement;
using LMP.Models;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Controllers.Services.Models.Messages;

namespace WebAPI.Controllers.Services.Messages
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class MessagesServiceController : ApiRoutedController
    {
        private readonly LazyDependency<IMessageManagementService> _messageManagementService =
            new LazyDependency<IMessageManagementService>();

        public IMessageManagementService MessageManagementService => _messageManagementService.Value;

        [HttpGet("GetMessages")]
        public IActionResult GetMessages()
        {
            try
            {
                var userId = /*todo #auth WebSecurity.CurrentUserId*/2;
                var model = MessageManagementService.GetUserMessages(userId).DistinctBy(m => m.MessageId).ToList();
                var result = new
                {
                    InboxMessages = model.Where(m => m.AuthorId != userId).OrderByDescending(e => e.Date)
                        .Select(e => new MessagesViewData(e)).ToList(),
                    OutboxMessages = model.Where(m => m.AuthorId == userId).OrderByDescending(e => e.Date)
                        .Select(e => new MessagesViewData(e)).ToList()
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return ServerError500(ex.Message);
            }
        }

        [HttpGet("GetMessage/{id}")]
        public IActionResult GetMessage(string id)
        {
            try
            {
                var userId = /*todo #auth WebSecurity.CurrentUserId*/2;
                var msgId = int.Parse(id);

                var msg = MessageManagementService.GetUserMessage(msgId, userId);

                if (msg.RecipientId == userId && !msg.IsRead) MessageManagementService.SetRead(msg.Id);

                return Ok(new DisplayMessageViewData(msg));
            }
            catch (Exception ex)
            {
                return ServerError500(ex.Message);
            }
        }

        [HttpGet("GetRecipients")]
        public IActionResult GetRecipients()
        {
            var recipients = new List<RecipientViewData>
            {
                new RecipientViewData(0, "Jack"),
                new RecipientViewData(1, "Mike")
            };

            return Ok(recipients);
        }

        [HttpPost("Save")]
        public IActionResult Save(string subject, string body, string recipients, string attachments)
        {
            try
            {
                var attachmentsList = JsonSerializer.Deserialize<List<Attachment>>(attachments).ToList();
                var recipientsList = JsonSerializer.Deserialize<List<int>>(recipients).ToList();
                var fromId = /*todo #auth WebSecurity.CurrentUserId*/2;

                if (!string.IsNullOrEmpty(subject) && subject.Length > 50) subject = subject.Substring(0, 50);

                var msg = new Message(body, subject);

                if (attachmentsList.Any())
                {
                    msg.AttachmentsPath = Guid.NewGuid();
                    attachmentsList.ForEach(a => a.PathName = msg.AttachmentsPath.ToString());
                    msg.Attachments = attachmentsList;
                }

                MessageManagementService.SaveMessage(msg);

                ////if (ToAdmin)
                ////{
                ////    var admin = UserManagementService.GetAdmin();
                ////    var userMsg = new UserMessages(admin.Id, FromId, msg.Id);
                ////    MessageManagementService.SaveUserMessages(userMsg);
                ////}
                ////else
                ////{
                foreach (var recipientId in recipientsList)
                {
                    var userMsg = new UserMessages(recipientId, fromId, msg.Id);
                    MessageManagementService.SaveUserMessages(userMsg);
                }
                ////}

                return Ok();
            }
            catch (Exception ex)
            {
                return ServerError500(ex.Message);
            }
        }

        [HttpDelete("Delete/{id:int}")]
        public IActionResult Delete(int messageId)
        {
            try
            {
                var userId = /*todo #auth WebSecurity.CurrentUserId*/2;
                var result = MessageManagementService.DeleteMessage(messageId, userId);

                return Ok();
            }
            catch (Exception ex)
            {
                return ServerError500(ex.Message);
            }
        }
    }
}
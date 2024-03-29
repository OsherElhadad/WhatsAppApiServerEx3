﻿using Microsoft.EntityFrameworkCore;
using WhatsAppApiServer.Data;
using WhatsAppApiServer.Models;

namespace WhatsAppApiServer.Services
{
    public class MessagesService : IMessagesService
    {
        private readonly WhatsAppApiContext _context;
        public MessagesService(WhatsAppApiContext contactsContext)
        {
            _context = contactsContext;
        }

        public override async Task<List<Message>?> GetMessages(string userId, string contactId)
        {
            var messages = await _context.Messages.ToListAsync();
            if (messages == null || userId == null)
            {
                return null;
            }

            var userContactMessages = from message in messages where message.UserId == userId && message.ContactId == contactId select message;

            return userContactMessages.ToList();
        }

        public override async Task<Message?> GetMessage(string userId, string contactId, int messageId)
        {
            var userContactMessages = await GetMessages(userId, contactId);

            if (userContactMessages == null)
            {
                return null;
            }

            var userContactMessage = from message in userContactMessages where message.Id == messageId select message;

            if (userContactMessage == null)
            {
                return null;
            }

            return userContactMessage.FirstOrDefault();
        }

        public override async Task<Message?> AddMessage(string userId, string contactId, string content)
        {
            var message = new Message();
            try
            {
                var contact = await _context.Contacts.FirstOrDefaultAsync(u => u.Id == contactId && u.UserId == userId);
                if (contact == null)
                {
                    return null;
                }
                message.Created = DateTime.Now;
                message.Content = content;
                message.Sent = true;
                contact.Last = content;
                contact.Lastdate = message.Created;
                message.Contact = contact;
                message.ContactId = contactId;
                message.UserId = userId;

                if (contact.Messages == null)
                {
                    contact.Messages = new List<Message>();
                }
                contact.Messages.Add(message);
                _context.Messages.Add(message);
                _context.Contacts.Update(contact);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                return null;
            }
            return message;
        }

        public override async Task<Message?> AddMessageTransfer(string userId, string contactId, string content)
        {
            var message = new Message();
            try
            {
                var contact = await _context.Contacts.FirstOrDefaultAsync(u => u.Id == contactId && u.UserId == userId);
                if (contact == null)
                {
                    return null;
                }
                message.Created = DateTime.Now;
                message.Content = content;
                message.Sent = false;
                contact.Last = content;
                contact.Lastdate = message.Created;
                message.Contact = contact;
                message.ContactId = contactId;
                message.UserId = userId;

                if (contact.Messages == null)
                {
                    contact.Messages = new List<Message>();
                }
                contact.Messages.Add(message);
                _context.Messages.Add(message);
                _context.Contacts.Update(contact);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                return null;
            }
            return message;
        }

        public override async Task<bool> UpdateMessage(string userId, string contactId, int messageId, string content)
        {
            if (!MessageExists(userId, contactId, messageId))
            {
                return false;
            }
            try
            {
                var oldMessage = await GetMessage(userId, contactId, messageId);
                if (oldMessage == null)
                {
                    return false;
                }
                oldMessage.Created = DateTime.Now;
                oldMessage.Content = content;
                var contact = await _context.Contacts.FirstOrDefaultAsync(u => u.Id == contactId && u.UserId == userId);
                if (contact == null)
                {
                    return false;
                }
                contact.Lastdate = oldMessage.Created;
                contact.Last = content;

                _context.Contacts.Update(contact);
                _context.Messages.Update(oldMessage);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public override async Task<bool> DeleteMessage(string userId, string contactId, int messageId)
        {
            if (!MessageExists(userId, contactId, messageId))
            {
                return false;
            }
            try
            {
                var userContactMessage = await GetMessage(userId, contactId, messageId);
                if (userContactMessage == null)
                {
                    return false;
                }
                var contact = await _context.Contacts.FirstOrDefaultAsync(u => u.Id == contactId && u.UserId == userId);
                if (contact == null)
                {
                    return false;
                }
                if (contact.Messages != null)
                {
                    contact.Messages.Remove(userContactMessage);
                }
                _context.Messages.Remove(userContactMessage);
                _context.Contacts.Update(contact);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public override async Task<bool> DeleteMessagesOfContact(string? userId, string? contactId)
        {
            if (userId == null || contactId == null)
            {
                return false;
            }
            var contactMessages = await GetMessages(userId, contactId);
            if (contactMessages == null)
            {
                return true;
            }
            foreach (var message in contactMessages)
            {
                if (!await DeleteMessage(userId, contactId, message.Id))
                {
                    return false;
                }
            }
            return true;
        }
        public override bool MessageExists(string userId, string contactId, int messageId)
        {
            return _context.Messages.Any(m => m.Id == messageId && m.ContactId == contactId && m.UserId == userId);
        }
    }
}

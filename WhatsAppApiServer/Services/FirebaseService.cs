using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using WhatsAppApiServer.Models;
using Message = WhatsAppApiServer.Models.Message;

namespace WhatsAppApiServer.Services
{
    public class FirebaseService
    {
        private static Dictionary<string, string> _users = new Dictionary<string, string>();

        public FirebaseService()
        {
        }

        public void AddUserToken(string username, string token)
        {
            _users[username] = token;
        }

        public string? GetToken(string username)
        {
            return _users.GetValueOrDefault(username);
        }

        public void SendInvatation(Contact contact)
        {
            try
            {
                FirebaseApp.Create(new AppOptions()
                {
                    Credential = GoogleCredential.FromFile("private_key.json")
                });

                var message = new FirebaseAdmin.Messaging.Message()
                {
                    Data = new Dictionary<string, string>() {
                    { "type", "Contact" },
                    { "Contact", contact.Id },
                    { "User", contact.UserId },
                },
                    Token = GetToken(contact.UserId),
                    Notification = new FirebaseAdmin.Messaging.Notification()
                    {
                        Title = "New contact",
                        Body = "User: " + contact.Id + " has new contact!"
                    }
                };
                FirebaseMessaging.DefaultInstance.SendAsync(message);
            } catch(Exception e)
            {

            }
        }

        public void SendTransfer(Contact contact, Message message)
        {
            try
            {
                FirebaseApp.Create(new AppOptions()
                {
                    Credential = GoogleCredential.FromFile("private_key.json")
                });

                var msg = new FirebaseAdmin.Messaging.Message()
                {
                    Data = new Dictionary<string, string>() {
                    { "type", "Message" },
                    { "Contact", contact.Id },
                    { "Message", message.Content },
                    { "User", message.UserId }
                },
                    Token = GetToken(contact.UserId),
                    Notification = new FirebaseAdmin.Messaging.Notification()
                    {
                        Title = "New message from " + contact.Id + " to " + message.UserId,
                        Body = "contact: " + contact.Id + " sent to user: " + message.UserId +
                                " new message: " + message.Content,
                    }
                };
                FirebaseMessaging.DefaultInstance.SendAsync(msg);
            } catch(Exception e)
            {

            }
        }
    }
}

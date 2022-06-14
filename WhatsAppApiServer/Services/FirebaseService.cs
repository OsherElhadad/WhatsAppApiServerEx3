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
            FirebaseApp.Create(new AppOptions()
            {
                Credential = GoogleCredential.FromFile("private_key.json")
            });

            var message = new FirebaseAdmin.Messaging.Message()
            {
                Data = new Dictionary<string, string>() {
                    { "Contact", contact.Id },
                },
                Token = GetToken(contact.Id),
                Notification = new FirebaseAdmin.Messaging.Notification()
                {
                    Title = "New contact",
                    Body = "User: " + contact.Id + " has new contact!"
                }
            };
            FirebaseMessaging.DefaultInstance.SendAsync(message);
        }

        public void SendTransfer(Contact contact, Message message)
        {
            FirebaseApp.Create(new AppOptions()
            {
                Credential = GoogleCredential.FromFile("private_key.json")
            });

            var msg = new FirebaseAdmin.Messaging.Message()
            {
                Data = new Dictionary<string, string>() {
                    { "Contact", contact.Id },
                    { "Message", message.Content },
                    { "From", message.UserId }
                },
                Token = GetToken(contact.Id),
                Notification = new FirebaseAdmin.Messaging.Notification()
                {
                    Title = "New message from " + message.UserId + " to " + contact.Id,
                    Body = "contact: " + message.UserId + " sent to user: " + contact.Id +
                            " new message: " + message.Content,
                }
            };
            FirebaseMessaging.DefaultInstance.SendAsync(msg);
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using WhatsAppApiServer.Hubs;
using WhatsAppApiServer.Models;
using WhatsAppApiServer.Services;

namespace WhatsAppApiServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransferController : ControllerBase
    {
        private readonly HubService _hubService;
        private readonly IMessagesService _messagesService;
        private readonly IContactsService _contactsService;
        private readonly IHubContext<MyHub> _myHub;
        private readonly FirebaseService _firebaseService;
        public TransferController(MessagesService messagesService, ContactsService contactsService,
            IHubContext<MyHub> myHub, HubService hubService, FirebaseService firebaseService)
        {
            _messagesService = messagesService;
            _myHub = myHub;
            _contactsService = contactsService;
            _hubService = hubService;
            _firebaseService = firebaseService;
        }

        // POST: Transfer
        [HttpPost]
        public async Task<IActionResult> PostTransfer([Bind("From,To,Content")] Transfer transfer)
        {
            if (transfer == null || transfer.From == null || transfer.To == null || transfer.Content == null)
            {
                return BadRequest();
            }
            var message = await _messagesService.AddMessageTransfer(transfer.To, transfer.From, transfer.Content);
            if (message == null)
            {
                return BadRequest();
            }

            var contact = await _contactsService.GetContact(transfer.To, transfer.From);

            string? connectionID = _hubService.GetConnectionId(transfer.To);

            if (!string.IsNullOrEmpty(connectionID))
            {
                await _myHub.Clients.Client(connectionID).SendAsync("MessageChangeRecieved", contact, message);
            }
            connectionID = _firebaseService.GetToken(transfer.To);
            if (!string.IsNullOrEmpty(connectionID) && contact != null)
            {
                _firebaseService.SendTransfer(contact, message);
            }
            return Created(nameof(PostTransfer), null);
        }
    }
}

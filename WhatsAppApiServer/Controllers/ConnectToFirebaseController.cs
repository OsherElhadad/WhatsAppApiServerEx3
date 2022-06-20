using Microsoft.AspNetCore.Mvc;
using WhatsAppApiServer.Models;
using WhatsAppApiServer.Services;

namespace WhatsAppApiServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConnectToFirebaseController : ControllerBase
    {
        private readonly FirebaseService _firebaseService;
        private readonly IUsersService _usersService;

        public ConnectToFirebaseController(FirebaseService firebaseService, UsersService usersService)
        {
            _firebaseService = firebaseService;
            _usersService = usersService;
        }

        // POST: ConnectToFirebase
        [HttpPost]
        public IActionResult PostConnectToFirebase([Bind("username, token")] UserFBToken userFBToken)
        {
            if (userFBToken == null || userFBToken.username == null ||
                userFBToken.token == null || !_usersService.UserExists(userFBToken.username))
            {
                return NotFound();
            }
            _firebaseService.AddUserToken(userFBToken.username, userFBToken.token);
            return Ok();
        }
    }
}

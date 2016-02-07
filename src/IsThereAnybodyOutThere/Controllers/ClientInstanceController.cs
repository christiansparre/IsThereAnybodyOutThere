using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using IsThereAnybodyOutThere.Messages;
using IsThereAnybodyOutThere.Models.Actors;
using IsThereAnybodyOutThere.Models.Clients;
using Microsoft.AspNet.Mvc;

namespace IsThereAnybodyOutThere.Controllers
{
    public class ClientInstanceController : Controller
    {
        private IActorRef _clientRegistry;

        public ClientInstanceController(ClientRegistryActorRef clientRegistryActorRef)
        {
            _clientRegistry = clientRegistryActorRef.ActorRef;
        }

        [Route("client/{id}")]
        public async Task<IActionResult> Index(string id)
        {
            var clientModels = await _clientRegistry.Ask<IEnumerable<ClientModel>>(new GetClients());

            var client = clientModels.FirstOrDefault(f => f.Id == id);

            if (client == null)
            {
                return HttpNotFound();
            }

            return View(client);
        }
    }
}
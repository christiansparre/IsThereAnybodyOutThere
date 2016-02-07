using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Emit;
using System.Threading.Tasks;
using Akka.Actor;
using IsThereAnybodyOutThere.Messages;
using IsThereAnybodyOutThere.Models;
using IsThereAnybodyOutThere.Models.Actors;
using IsThereAnybodyOutThere.Models.Clients;
using Microsoft.AspNet.Mvc;

namespace IsThereAnybodyOutThere.Controllers
{
    public class ClientsController : Controller
    {
        private readonly ClientRegistryActorRef _clientRegistry;

        public ClientsController(ClientRegistryActorRef clientRegistry)
        {
            _clientRegistry = clientRegistry;
        }

        [Route("clients")]
        public async Task<IActionResult> Index()
        {
            var clients = await GetClients();

            return View(new CommonClientListViewModel { Clients = clients.OrderByDescending(o => o.LastHeartbeatReceivedAtUtc) });
        }

        private async Task<List<ClientModel>> GetClients(Func<ClientModel, bool> filter = null)
        {
            var clientModels = await _clientRegistry.ActorRef.Ask<List<ClientModel>>(new GetClients());

            if (filter != null)
            {
                clientModels = clientModels.Where(filter).ToList();
            }

            return clientModels;
        }

        [Route("clients/user/{username}")]
        public async Task<IActionResult> ByUser(string username)
        {
            ViewBag.Username = username;
            return View(new CommonClientListViewModel { Clients = await GetClients(model => model.Username == username) });

        }

        [Route("clients/application/{name}")]
        public async Task<IActionResult> ByApplication(string name)
        {
            ViewBag.ApplicationName = name;
            return View(new CommonClientListViewModel { Clients = await GetClients(model => model.ApplicationName == name) });

        }

        [Route("clients/machine/{name}")]
        public async Task<IActionResult> ByMachine(string name)
        {
            ViewBag.MachineName = name;
            return View(new CommonClientListViewModel { Clients = await GetClients(model => model.MachineName == name) });

        }
    }
}
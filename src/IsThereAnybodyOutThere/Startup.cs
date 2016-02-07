using Akka.Actor;
using IsThereAnybodyOutThere.Actors;
using IsThereAnybodyOutThere.Models.Actors;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace IsThereAnybodyOutThere
{
    public class Startup
    {
        private IConfigurationRoot _configuration;

        public Startup()
        {
            _configuration = new ConfigurationBuilder()
                .AddJsonFile("config.json")
                .Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IConfiguration>(s => _configuration);


            services.AddMvc();
            services.AddLogging();

            var actorSystem = ActorSystem.Create("IsThereAnybodyOutThere");

            var clientRegistry = actorSystem.ActorOf(Props.Create(() => new ClientRegistry()), "clients");
            var actorRef = actorSystem.ActorOf(Props.Create(() => new WebSocketServerActor(clientRegistry, _configuration)), "websocket");


            services.AddSingleton(s => new ClientRegistryActorRef(clientRegistry));

        }

        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(LogLevel.Debug);

            app.UseStaticFiles();
            app.UseMvc();
        }

        public static void Main(string[] args) => WebApplication.Run<Startup>(args);
    }
}

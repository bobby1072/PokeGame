using Microsoft.Extensions.Configuration;

var builder = DistributedApplication.CreateBuilder(args);


var useReactServer = builder.Configuration.GetValue<bool>("UseReactServer");

if (useReactServer)
{
    builder
        .AddProject<Projects.PokeGame_Core_React_Server>("PokeGame-Core-React-Server")
        .WithExternalHttpEndpoints();
}

builder
    .AddProject<Projects.PokeGame_Core_Api>("PokeGame-Core-Api")
    .WithExternalHttpEndpoints();

builder
    .AddProject<Projects.PokeGame_Core_SignalR>("PokeGame-Core-SignalR")
    .WithExternalHttpEndpoints();


await builder.Build().RunAsync();
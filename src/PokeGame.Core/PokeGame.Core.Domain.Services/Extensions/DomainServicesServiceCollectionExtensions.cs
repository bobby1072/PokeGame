﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PokeGame.Core.Common.Configurations;
using PokeGame.Core.Common.Extensions;
using PokeGame.Core.Common.Services.Extensions;
using PokeGame.Core.Domain.Services.Abstract;
using PokeGame.Core.Domain.Services.Concrete;
using PokeGame.Core.Domain.Services.Pokedex.Commands;
using PokeGame.Core.Domain.Services.Pokedex.Concrete;
using PokeGame.Core.Domain.Services.User.Commands;
using PokeGame.Core.Persistence.Extensions;
using PokeGame.Core.Schemas;
using PokeGame.Core.Schemas.Extensions;
using PokeGame.Core.Schemas.Input;

namespace PokeGame.Core.Domain.Services.Extensions;

public static class DomainServicesServiceCollectionExtensions
{
    public static async Task<IServiceCollection> AddPokeGameApplicationServices(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
    {
        var serviceInfoSection = configuration.GetSection(ServiceInfo.Key);

        if (!serviceInfoSection.Exists())
        {
            throw new ArgumentException("Service info section not found");
        }

        services
            .AddHttpClient()
            .AddLogging()
            .AddCommonServices()
            .AddDomainModelValidators()
            .AddPokeGamePersistence(configuration, environment.IsDevelopment())
            .ConfigureSingletonOptions<ServiceInfo>(serviceInfoSection);

        await services
            .AddPokedexJson();
        
        services
            .AddScoped<IDomainServiceCommandExecutor, DomainServiceCommandExecutor>()
            .AddScoped<IDomainCommand<IReadOnlyCollection<PokedexPokemon>, IReadOnlyCollection<PokedexPokemon>>, CreatePokedexPokemonCommand>()
            .AddScoped<IDomainCommand<SaveUserInput, Schemas.User>, SaveUserCommand>()
            .AddHostedService<PokedexDataMigratorHostedService>();

        return services;
    }
}
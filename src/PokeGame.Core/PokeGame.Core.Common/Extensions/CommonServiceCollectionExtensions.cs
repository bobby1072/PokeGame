﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using PokeGame.Core.Common.Helpers;
using PokeGame.Core.Common.Models;

namespace PokeGame.Core.Common.Extensions;

public static class CommonServiceCollectionExtensions
{
    public static async Task<IServiceCollection> AddPokedexJson(this IServiceCollection services)
    {
        var pokedexJson = await PokedexJsonFileHelper.GetFromDataFolder();
        
        
        services
            .AddSingleton(pokedexJson);
        

        return services;
    }
    
    
    
    public static IServiceCollection ConfigureSingletonOptions<TOptions>(this IServiceCollection services,
        IConfiguration configSection) where TOptions : class
    {
        services
            .Configure<TOptions>(configSection)
            .AddSingleton<TOptions>(sp => sp.GetRequiredService<IOptions<TOptions>>().Value);

        return services;
    }
    public static IServiceCollection ConfigureScopedOptions<TOptions>(this IServiceCollection services,
        IConfiguration configSection) where TOptions : class
    {
        services
            .Configure<TOptions>(configSection)
            .AddScoped<TOptions>(sp => sp.GetRequiredService<IOptionsSnapshot<TOptions>>().Value);

        return services;
    }
}
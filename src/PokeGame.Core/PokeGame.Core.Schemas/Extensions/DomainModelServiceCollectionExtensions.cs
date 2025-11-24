using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using PokeGame.Core.Schemas.Game;
using PokeGame.Core.Schemas.Validators;

namespace PokeGame.Core.Schemas.Extensions;

public static class DomainModelServiceCollectionExtensions
{
    public static IServiceCollection AddDomainModelValidators(this IServiceCollection services)
    {
        services
            .AddSingleton<IValidator<User>, UserValidator>()
            .AddSingleton<IValidator<GameSave>, GameSaveValidator>()
            .AddSingleton<IValidator<OwnedPokemon>, OwnedPokemonValidator>()
            .AddSingleton<IValidator<OwnedItem>, OwnedItemValidator>()
            .AddSingleton<IValidator<GameSaveData>,  GameSaveDataValidator>();
        
        return services;
    }
}
using System.Net;
using BT.Common.Persistence.Shared.Utils;
using BT.Common.Services.Concrete;
using Microsoft.Extensions.Logging;
using PokeGame.Core.Common.Exceptions;
using PokeGame.Core.Domain.Services.Abstract;
using PokeGame.Core.Domain.Services.Game.Abstract;
using PokeGame.Core.Domain.Services.Models;
using PokeGame.Core.Persistence.Repositories.Abstract;
using PokeGame.Core.Schemas.Game;
using PokeGame.Core.Schemas.Game.PokemonRelated;

namespace PokeGame.Core.Domain.Services.Game.Commands;

internal sealed class GetOwnedPokemonByIdCommand
    : IDomainCommand<
        (bool DeepVersion, Guid OwnedPokemonId, Schemas.Game.User CurrentUser),
        DomainCommandResult<OwnedPokemon>
    >
{
    public string CommandName => nameof(GetOwnedPokemonByIdCommand);
    private readonly IGameAndPokeApiResourceManagerService _gameAndPokeApiResourceManagerService;
    private readonly IOwnedPokemonRepository _ownedPokemonRepository;
    private readonly ILogger<GetOwnedPokemonByIdCommand> _logger;

    public GetOwnedPokemonByIdCommand(
        IGameAndPokeApiResourceManagerService gameAndPokeApiResourceManagerService,
        IOwnedPokemonRepository ownedPokemonRepository,
        ILogger<GetOwnedPokemonByIdCommand> logger
    )
    {
        _gameAndPokeApiResourceManagerService = gameAndPokeApiResourceManagerService;
        _ownedPokemonRepository = ownedPokemonRepository;
        _logger = logger;
    }

    public async Task<DomainCommandResult<OwnedPokemon>> ExecuteAsync(
        (bool DeepVersion, Guid OwnedPokemonId, Schemas.Game.User CurrentUser) input,
        CancellationToken ct = default
    )
    {
        using var activity = TelemetryHelperService.ActivitySource.StartActivity(CommandName);
        activity?.SetTag("deepVersion", input.DeepVersion);
        activity?.SetTag("ownedPokemonId", input.OwnedPokemonId.ToString());
        activity?.SetTag("userId", input.CurrentUser.Id?.ToString());

        _logger.LogInformation(
            "About to get OwnedPokemon by id for user with id: {UserId}",
            input.CurrentUser.Id
        );

        var foundOwnedPokemon =
            await EntityFrameworkUtils.TryDbOperation(
                () =>
                    _ownedPokemonRepository.GetOne(
                        input.OwnedPokemonId,
                        relations: nameof(OwnedPokemon.GameSave)
                    ),
                _logger
            ) ?? throw new PokeGameApiServerException("Failed to fetch owned pokemon");

        if (foundOwnedPokemon.Data is null || !foundOwnedPokemon.IsSuccessful)
        {
            throw new PokeGameApiUserException(
                HttpStatusCode.BadRequest,
                "Failed to fetch owned pokemon with that id"
            );
        }

        if (foundOwnedPokemon.Data.GameSave?.UserId != input.CurrentUser.Id!)
        {
            throw new PokeGameApiUserException(
                HttpStatusCode.Unauthorized,
                "User does not have permission to get owned pokemon"
            );
        }
        if (!input.DeepVersion)
        {
            _logger.LogInformation(
                "Fetched shallow OwnedPokemon from db by id: {OwnedPokemonId}",
                input.OwnedPokemonId
            );

            return new DomainCommandResult<OwnedPokemon> { CommandResult = foundOwnedPokemon.Data };
        }
        else
        {
            _logger.LogInformation("Fetching deep OwnedPokemon from Api");

            var deepPokemon = await _gameAndPokeApiResourceManagerService.GetDeepOwnedPokemon(
                [foundOwnedPokemon.Data]
            );

            return new DomainCommandResult<OwnedPokemon> { CommandResult = deepPokemon.Single() };
        }
    }
}

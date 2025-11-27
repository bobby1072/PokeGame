using System.Net;
using BT.Common.FastArray.Proto;
using BT.Common.Persistence.Shared.Utils;
using Microsoft.Extensions.Logging;
using PokeGame.Core.Common.Exceptions;
using PokeGame.Core.Domain.Services.Abstract;
using PokeGame.Core.Domain.Services.Game.Abstract;
using PokeGame.Core.Domain.Services.Models;
using PokeGame.Core.Persistence.Repositories.Abstract;
using PokeGame.Core.Schemas.Game;

namespace PokeGame.Core.Domain.Services.Game.Commands;

internal sealed class GetOwnedPokemonInDeckCommand: IDomainCommand<(bool DeepVersion, Guid GameSaveId, Schemas.Game.User CurrentUser), DomainCommandResult<IReadOnlyCollection<OwnedPokemon>>>
{
    public string CommandName => nameof(GetOwnedPokemonInDeckCommand);
    private readonly IGameAndPokeApiResourceManagerService _gameAndPokeApiResourceManagerService;
    private readonly IGameSaveRepository _gameSaveRepository;
    private readonly IOwnedPokemonRepository _ownedPokemonRepository;
    private readonly ILogger<GetOwnedPokemonInDeckCommand> _logger;

    public GetOwnedPokemonInDeckCommand(IGameAndPokeApiResourceManagerService gameAndPokeApiResourceManagerService,
        IGameSaveRepository gameSaveRepository,
        IOwnedPokemonRepository ownedPokemonRepository,
        ILogger<GetOwnedPokemonInDeckCommand> logger)
    {
        _gameAndPokeApiResourceManagerService = gameAndPokeApiResourceManagerService;
        _gameSaveRepository = gameSaveRepository;
        _ownedPokemonRepository = ownedPokemonRepository;
        _logger = logger;
    }
    
    public async Task<DomainCommandResult<IReadOnlyCollection<OwnedPokemon>>> ExecuteAsync((bool DeepVersion, Guid GameSaveId, Schemas.Game.User CurrentUser) input, CancellationToken ct = default)
    {
        _logger.LogInformation("About to get owned pokemon in deck for user with id: {UserId}", input.CurrentUser.Id);

        var foundGameSaveData = await EntityFrameworkUtils.TryDbOperation(() =>
            _gameSaveRepository.GetOne(input.GameSaveId, relations: nameof(GameSave.GameSaveData)), _logger)
                ?? throw new PokeGameApiServerException("Failed to fetch game save data");

        if (foundGameSaveData.Data is null || !foundGameSaveData.IsSuccessful)
        {
            throw new PokeGameApiUserException(HttpStatusCode.BadRequest,"Failed to find game save data");
        }

        if (foundGameSaveData.Data.UserId != input.CurrentUser.Id)
        {
            throw new PokeGameApiUserException(HttpStatusCode.Unauthorized, "User does not have permission to access this deck");
        }
        if (foundGameSaveData.Data.GameSaveData?.GameData.DeckPokemon.Count is null or < 1)
        {
            throw new PokeGameApiUserException(HttpStatusCode.BadRequest, "Empty pokemon deck for game save");
        }

        _logger.LogInformation("Going to fetch {PokemonInDeckCount} OwnedPokemon from deck for game save: {GameSaveId}",
            foundGameSaveData.Data.GameSaveData.GameData.DeckPokemon.Count,
            input.GameSaveId);
        
        if (!input.DeepVersion)
        {
            _logger.LogInformation("Simply getting shallow owned pokemon in deck from db");
            var allPokemon = await GetShallowDeck(foundGameSaveData.Data.GameSaveData.GameData.DeckPokemon.FastArraySelect(x => (Guid?)x.OwnedPokemonId)
                .ToArray());
            
            return new DomainCommandResult<IReadOnlyCollection<OwnedPokemon>>
            {
                CommandResult = allPokemon
            };
        }
        else
        {
            _logger.LogInformation("Fetching deep owned pokemon in deck from db and poke api");
            var allPokemon = await _gameAndPokeApiResourceManagerService.GetFullOwnedPokemon(foundGameSaveData.Data.GameSaveData.GameData.DeckPokemon.FastArraySelect(x => (Guid)x.OwnedPokemonId).ToArray());

            return new DomainCommandResult<IReadOnlyCollection<OwnedPokemon>>
            {
                CommandResult = allPokemon
            };
        }
    }

    private async Task<IReadOnlyCollection<OwnedPokemon>> GetShallowDeck(IReadOnlyCollection<Guid?> deckPokemonIds)
    {
        var foundPokemon =
            await EntityFrameworkUtils.TryDbOperation(() => _ownedPokemonRepository.GetMany(entityIds: deckPokemonIds), _logger)
                ?? throw new PokeGameApiServerException("Failed to fetch own pokemon in deck");

        if (foundPokemon.Data.Count < 1 || !foundPokemon.IsSuccessful)
        {
            throw new PokeGameApiServerException("Failed to fetch own pokemon in deck");
        }
        
        return foundPokemon.Data;
    }
}
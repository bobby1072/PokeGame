using System.Net;
using BT.Common.FastArray.Proto;
using BT.Common.Persistence.Shared.Utils;
using Microsoft.Extensions.Logging;
using PokeGame.Core.Common.Exceptions;
using PokeGame.Core.Domain.Services.Game.Abstract;
using PokeGame.Core.Domain.Services.Models;
using PokeGame.Core.Persistence.Repositories.Abstract;
using PokeGame.Core.Schemas.Game;

namespace PokeGame.Core.Domain.Services.Game.Commands;

internal sealed class GetOwnedPokemonInDeckByConnectionIdCommand: GetOwnedPokemonInDeckCommandBase<(bool DeepVersion, string ConnectionId, Schemas.Game.User CurrentUser)>
{
    public override string CommandName => nameof(GetOwnedPokemonInDeckByConnectionIdCommand);
    private readonly IGameSessionRepository _gameSessionRepository;
    private readonly ILogger<GetOwnedPokemonInDeckByConnectionIdCommand> _logger;

    public GetOwnedPokemonInDeckByConnectionIdCommand(IGameAndPokeApiResourceManagerService gameAndPokeApiResourceManagerService,
        IGameSessionRepository gameSessionRepository,
        IOwnedPokemonRepository ownedPokemonRepository,
        ILogger<GetOwnedPokemonInDeckByConnectionIdCommand> logger): 
            base(gameAndPokeApiResourceManagerService,
                ownedPokemonRepository,
                logger)
    {
        _gameSessionRepository = gameSessionRepository;
        _logger = logger;
    }
    
    public override async Task<DomainCommandResult<IReadOnlyCollection<OwnedPokemon>>> ExecuteAsync((bool DeepVersion, string ConnectionId, Schemas.Game.User CurrentUser) input, CancellationToken ct = default)
    {
        _logger.LogInformation("About to get owned pokemon in deck for user with id: {UserId}", input.CurrentUser.Id);

        var foundGameSession = await EntityFrameworkUtils.TryDbOperation(() =>
            _gameSessionRepository.GetOneWithGameSaveAndDataByConnectionIdAsync(input.ConnectionId), _logger)
                ?? throw new PokeGameApiServerException("Failed to fetch game save data");

        if (foundGameSession.Data is null || !foundGameSession.IsSuccessful)
        {
            throw new PokeGameApiUserException(HttpStatusCode.BadRequest,"Failed to find game save data");
        }

        if (foundGameSession.Data.UserId != input.CurrentUser.Id)
        {
            throw new PokeGameApiUserException(HttpStatusCode.Unauthorized, "User does not have permission to access this deck");
        }
        if (foundGameSession.Data.GameSave?.GameSaveData?.GameData.DeckPokemon.Count is null or < 1)
        {
            throw new PokeGameApiUserException(HttpStatusCode.BadRequest, "Empty pokemon deck for game save");
        }

        var result = await FetchPokemon(foundGameSession.Data, input.DeepVersion);
        
        return new DomainCommandResult<IReadOnlyCollection<OwnedPokemon>>
        {
            CommandResult = result,
        };
    }
}
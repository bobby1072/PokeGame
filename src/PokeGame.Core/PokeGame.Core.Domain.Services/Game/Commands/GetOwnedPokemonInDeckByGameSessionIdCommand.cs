using System.Net;
using BT.Common.Persistence.Shared.Utils;
using Microsoft.Extensions.Logging;
using PokeGame.Core.Common.Exceptions;
using PokeGame.Core.Domain.Services.Game.Abstract;
using PokeGame.Core.Domain.Services.Models;
using PokeGame.Core.Persistence.Repositories.Abstract;
using PokeGame.Core.Schemas.Game;

namespace PokeGame.Core.Domain.Services.Game.Commands;

internal sealed class GetOwnedPokemonInDeckByGameSessionIdCommand: 
    GetOwnedPokemonInDeckCommandBase<(bool DeepVersion, Guid GameSessionId, Schemas.Game.User CurrentUser)>
{
   public override string CommandName => nameof(GetOwnedPokemonInDeckByGameSessionIdCommand);
   private readonly IGameSessionRepository _gameSessionRepository;
   private readonly ILogger<GetOwnedPokemonInDeckByGameSessionIdCommand> _logger;

   public GetOwnedPokemonInDeckByGameSessionIdCommand(IGameAndPokeApiResourceManagerService gameAndPokeApiResourceManagerService,
       IGameSessionRepository gameSessionRepository,
       IOwnedPokemonRepository ownedPokemonRepository,
       ILogger<GetOwnedPokemonInDeckByGameSessionIdCommand> logger): 
       base(gameAndPokeApiResourceManagerService,
           ownedPokemonRepository,
           logger)
   {
       _gameSessionRepository = gameSessionRepository;
       _logger = logger;
   }

   public override async Task<DomainCommandResult<IReadOnlyCollection<OwnedPokemon>>> ExecuteAsync((bool DeepVersion, Guid GameSessionId, Schemas.Game.User CurrentUser) input, CancellationToken cancellationToken)
   {
       _logger.LogInformation("About to get owned pokemon in deck for user with id: {UserId}", input.CurrentUser.Id);

       var foundGameSession = await EntityFrameworkUtils.TryDbOperation(() =>
                                  _gameSessionRepository.GetOneWithGameSaveAndDataByGameSessionIdAsync(input.GameSessionId), _logger)
                              ?? throw new PokeGameApiServerException("Failed to fetch game save data");
       
       var result = await FetchPokemon(foundGameSession.Data, input.DeepVersion, input.CurrentUser, cancellationToken);
        
       return new DomainCommandResult<IReadOnlyCollection<OwnedPokemon>>
       {
           CommandResult = result,
       };
   }
}
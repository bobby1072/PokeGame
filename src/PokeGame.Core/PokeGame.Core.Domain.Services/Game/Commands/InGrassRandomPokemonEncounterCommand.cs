using Microsoft.Extensions.Logging;
using PokeGame.Core.Domain.Services.Abstract;
using PokeGame.Core.Domain.Services.Game.Abstract;
using PokeGame.Core.Domain.Services.Models;
using PokeGame.Core.Persistence.Repositories.Abstract;
using PokeGame.Core.Schemas.Game;

namespace PokeGame.Core.Domain.Services.Game.Commands;

internal sealed class InGrassRandomPokemonEncounterCommand: IDomainCommand<(string SceneName, string ConnectionId, Schemas.Game.User CurrentUser), DomainCommandResult<WildPokemon?>>
{
    public string CommandName => nameof(InGrassRandomPokemonEncounterCommand);
    
    private readonly IGameSessionRepository _gameSessionRepository;
    private readonly IPokeApiClient _pokeApiClient;
    private readonly IPokeGameRuleHelperService _pokeGameRuleHelperService;
    private readonly ILogger<InGrassRandomPokemonEncounterCommand> _logger;

    public InGrassRandomPokemonEncounterCommand(IGameSessionRepository gameSessionRepository,
        IPokeApiClient pokeApiClient,
        IPokeGameRuleHelperService pokeGameRuleHelperService,
        ILogger<InGrassRandomPokemonEncounterCommand> logger)
    {
        _gameSessionRepository = gameSessionRepository;
        _pokeApiClient = pokeApiClient;
        _pokeGameRuleHelperService = pokeGameRuleHelperService;
        _logger = logger;
    }
    
    
    public async Task<DomainCommandResult<WildPokemon?>> ExecuteAsync(
        (string SceneName, string ConnectionId, Schemas.Game.User CurrentUser) input,
        CancellationToken cancelationToken = default)
    {
        throw new NotImplementedException();
    }
}
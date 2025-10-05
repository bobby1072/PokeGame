using PokeGame.Core.Domain.Services.Abstract;
using PokeGame.Core.Domain.Services.Game.Abstract;
using PokeGame.Core.Domain.Services.Game.Commands;
using PokeGame.Core.Schemas.Game;
using PokeGame.Core.Domain.Services.Models;

namespace PokeGame.Core.Domain.Services.Game.Concrete;

internal sealed class GameSaveProcessingManager: IGameSaveProcessingManager
{
    private readonly IDomainServiceCommandExecutor _domainServiceCommandExecutor;

    public GameSaveProcessingManager(IDomainServiceCommandExecutor domainServiceCommandExecutor)
    {
        _domainServiceCommandExecutor = domainServiceCommandExecutor;
    }
    
    public async Task<GameSave> SaveGameAsync(string characterName, Schemas.Game.User currentUser) =>
        (await _domainServiceCommandExecutor
            .RunCommandAsync<CreateNewGameCommand, (string CharacterName, Schemas.Game.User CurrentUser),
                DomainCommandResult<GameSave>>((characterName, currentUser))).CommandResult;
    
    public async Task<IReadOnlyCollection<GameSave>> GetGameSavesForUserAsync(Schemas.Game.User currentUser) =>
        (await _domainServiceCommandExecutor
            .RunCommandAsync<GetGameSavesByUserCommand, Schemas.Game.User,
                DomainCommandResult<IReadOnlyCollection<GameSave>>>(currentUser)).CommandResult;
}
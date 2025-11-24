using BT.Common.Persistence.Shared.Utils;
using Microsoft.Extensions.Logging;
using PokeGame.Core.Common.Exceptions;
using PokeGame.Core.Domain.Services.Abstract;
using PokeGame.Core.Domain.Services.Models;
using PokeGame.Core.Persistence.Entities;
using PokeGame.Core.Persistence.Repositories.Abstract;
using PokeGame.Core.Schemas.Game;

namespace PokeGame.Core.Domain.Services.Game.Commands;

internal sealed class GetGameSavesByUserCommand
    : IDomainCommand<Schemas.Game.User, DomainCommandResult<IReadOnlyCollection<GameSave>>>
{
    public string CommandName => nameof(GetGameSavesByUserCommand);
    private readonly IGameSaveRepository _gameSaveRepository;
    private readonly ILogger<GetGameSavesByUserCommand> _logger;

    public GetGameSavesByUserCommand(
        ILogger<GetGameSavesByUserCommand> logger,
        IGameSaveRepository gameSaveRepository
    )
    {
        _logger = logger;
        _gameSaveRepository = gameSaveRepository;
    }

    public async Task<DomainCommandResult<IReadOnlyCollection<GameSave>>> ExecuteAsync(
        Schemas.Game.User user,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogInformation("About to get game saves for user with id: {UserId}", user.Id);

        var foundGameSaves =
            await EntityFrameworkUtils.TryDbOperation(
                () =>
                    _gameSaveRepository.GetMany<Guid>(
                        (Guid)user.Id!,
                        nameof(GameSaveEntity.UserId),
                        nameof(GameSaveEntity.GameSaveData)
                    ),
                _logger
            ) ?? throw new PokeGameApiServerException("Failed to fetch game saves");

        return new DomainCommandResult<IReadOnlyCollection<GameSave>>
        {
            CommandResult = foundGameSaves.Data,
        };
    }
}

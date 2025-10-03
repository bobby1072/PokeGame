using BT.Common.Api.Helpers.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PokeGame.Core.Api.Attributes;
using PokeGame.Core.Domain.Services.Game.Abstract;
using PokeGame.Core.Schemas.Game;
using PokeGame.Core.Schemas.Input;

namespace PokeGame.Core.Api.Controllers;

[AllowAnonymous]
[RequireValidUserIdHeader]
public sealed class GameSaveController: BaseController
{
    private readonly IGameSaveProcessingManager _gameSaveProcessingManager;

    public GameSaveController(IGameSaveProcessingManager gameSaveProcessingManager)
    {
        _gameSaveProcessingManager = gameSaveProcessingManager;
    }

    [HttpPost("NewGameSave")]
    public async Task<ActionResult<WebOutcome<GameSave>>> InstantiateNewGame([FromBody]NewGameSaveInput input)
    {
        var currentUser = GetCurrentUser();
        
        var newGame = await _gameSaveProcessingManager.SaveGameAsync(input.NewCharacterName, currentUser);

        return new WebOutcome<GameSave>
        {
            Data = newGame,
        };
    }

    [HttpGet("GetAllForSelf")]
    public async Task<ActionResult<WebOutcome<IReadOnlyCollection<GameSave>>>> GetAllGameSavesForUser()
    {
        var currentUser = GetCurrentUser();
        
        var gameSaves = await _gameSaveProcessingManager.GetGameSavesForUserAsync(currentUser);

        return new WebOutcome<IReadOnlyCollection<GameSave>>
        {
            Data = gameSaves,
        };
    }
}
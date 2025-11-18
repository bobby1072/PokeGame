using BT.Common.Api.Helpers.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PokeGame.Core.Api.Attributes;
using PokeGame.Core.Domain.Services.User.Abstract;
using PokeGame.Core.Schemas;
using PokeGame.Core.Schemas.Game;
using PokeGame.Core.Schemas.Input;

namespace PokeGame.Core.Api.Controllers;

[AllowAnonymous]
public sealed class UserController: BaseController
{
    private readonly IUserProcessingManager _userProcessingManager;

    public UserController(IUserProcessingManager userProcessingManager)
    {
        _userProcessingManager = userProcessingManager;
    }

    [HttpGet("Get")]
    public async Task<ActionResult<WebOutcome<User>>> GetUserAsync(string email)
    {
        var result = await _userProcessingManager.GetUserAsync(email);


        return new WebOutcome<User>
        {
            Data = result
        };
    }
    [HttpPost("Save")]
    public async Task<ActionResult<WebOutcome<User>>> SaveUserAsync(SaveUserInput input)
    {
        var result =  await _userProcessingManager.SaveUserAsync(input);

        return new WebOutcome<User>
        {
            Data = result,
            ExtraData = new Dictionary<string, object>
            {
                {"SaveType", input.Id is null ? "Create": "Update"}
            }
        };
    } 
}
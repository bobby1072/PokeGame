using FluentValidation;
using PokeGame.Core.Common.GameInformationData;
using PokeGame.Core.Schemas.Game;

namespace PokeGame.Core.Schemas.Validators;

internal sealed class GameSaveDataValidator: BaseValidator<GameSaveData>
{
    public GameSaveDataValidator(ValidGameSceneList validGameSceneList)
    {
        RuleFor(x => x.GameData.LastPlayedScene)
            .Must(validGameSceneList.Contains)
            .WithMessage("Last played scene is invalid");
    }
}
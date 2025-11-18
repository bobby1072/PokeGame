using FluentValidation;
using PokeGame.Core.Common.GameInformationData;
using PokeGame.Core.Schemas.Game;

namespace PokeGame.Core.Schemas.Validators;

internal sealed class GameSaveDataValidator: BaseValidator<GameSaveData>
{
    public GameSaveDataValidator()
    {
        RuleFor(x => x.GameData.LastPlayedScene)
            .Must(GameConstants.SceneNames.ValidSceneList.Contains)
            .WithMessage("Last played scene is invalid");
        RuleFor(x => x.GameData.DeckPokemon)
            .Must(x => x.Count <= 6)
            .WithMessage("You can only have 6 deck pokemon in your deck");
    }
}
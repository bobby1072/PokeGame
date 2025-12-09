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
        RuleFor(x => x.GameData.LastPlayedLocationX).Must(Between29AndZero).WithMessage("Last played location is invalid");
        RuleFor(x => x.GameData.LastPlayedLocationY).Must(Between29AndZero).WithMessage("Last played location is invalid");


        RuleFor(x => x.GameData.UnlockedGameResources)
            .Must(x =>
            {
                var sceneUnlockedResources = x.Where(y => y.Type == GameDataActualUnlockedGameResourceType.Scene)
                    .ToArray();
                if (sceneUnlockedResources.Length > 0 && !sceneUnlockedResources.All(y => GameConstants.SceneNames.ValidSceneList.Contains(y.ResourceName)))
                {
                    return false;
                }

                return true;
            })
            .WithMessage("Invalid scene name included in unlocked resource collection");;
    }
    private static bool Between29AndZero(int sceneLocation) => sceneLocation <= 29 && sceneLocation >= 0; 
}
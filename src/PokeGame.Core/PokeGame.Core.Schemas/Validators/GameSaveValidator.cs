using FluentValidation;
using PokeGame.Core.Schemas.Game;

namespace PokeGame.Core.Schemas.Validators;

internal sealed class GameSaveValidator: BaseValidator<GameSave>
{
    public GameSaveValidator()
    {
        RuleFor(x => x.CharacterName).NotEmpty().WithMessage("Character name cannot be empty");
        RuleFor(x => x.CharacterName).Must(x => x.Length <= 50).WithMessage("Character name must be less than 50 characters");
    }
}
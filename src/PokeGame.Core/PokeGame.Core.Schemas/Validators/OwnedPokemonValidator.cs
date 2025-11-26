using FluentValidation;
using PokeGame.Core.Schemas.Game;

namespace PokeGame.Core.Schemas.Validators;

internal sealed class OwnedPokemonValidator: BaseValidator<OwnedPokemon>
{
    public OwnedPokemonValidator()
    {
        RuleFor(x => x.PokemonResourceName).NotEmpty().WithMessage("Resource name cannot be empty");
        RuleFor(x => x.PokemonResourceName).Must(IsValidUri).WithMessage("Resource names must be valid Uris");
        RuleFor(x => x.PokemonLevel).Must(x => x is > 0 and < 100).WithMessage("Pokemon level must be between 1 - 99");
        RuleFor(x => x.CurrentExperience).Must(x => x is > 0).WithMessage("Current experience must be greater than 0");
        RuleFor(x => x.CurrentHp).Must(x => x is >= 0).WithMessage("Current hp cannot be negative");
        RuleFor(x => x.MoveOneResourceName).NotEmpty().WithMessage("Move one resource name cannot be empty");
        RuleFor(x => x.MoveOneResourceName).Must(IsValidUri).WithMessage("Resource names must be valid Uris");
        RuleFor(x => x.MoveTwoResourceName).Must(x => x is null || IsValidUri(x)).WithMessage("Resource names must be valid Uris");
        RuleFor(x => x.MoveThreeResourceName).Must(x => x is null || IsValidUri(x)).WithMessage("Resource names must be valid Uris");
        RuleFor(x => x.MoveFourResourceName).Must(x => x is null || IsValidUri(x)).WithMessage("Resource names must be valid Uris");
    }
}
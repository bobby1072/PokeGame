using FluentValidation;

namespace PokeGame.Core.Schemas.Validators;

internal sealed class ItemStackValidator: BaseValidator<ItemStack>
{
    public ItemStackValidator()
    {
        RuleFor(x => x.ResourceName).NotEmpty().WithMessage("Resource name cannot be empty");
        RuleFor(x => x.ResourceName).Must(IsValidUri).WithMessage("Resource names must be valid Uris");
        RuleFor(x => x.Quantity).Must(x => x is > 0 and <= 99).WithMessage("Quantity must be between 1 - 99");
    }
}
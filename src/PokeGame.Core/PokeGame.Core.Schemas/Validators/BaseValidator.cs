using BT.Common.Helpers.Extensions;
using FluentValidation;

namespace PokeGame.Core.Schemas.Validators;

internal abstract class BaseValidator<T> : AbstractValidator<T>
{
    protected static bool IsValidUri(string uri)
    {
        try
        {
            _ = new Uri(uri);

            return true;
        }
        catch
        {
            return false;
        }
    }
    protected static bool IsValidEmail(string email) => email.IsValidEmail();
}
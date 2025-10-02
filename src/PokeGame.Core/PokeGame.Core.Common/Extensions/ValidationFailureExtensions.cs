using System.Text;
using FluentValidation.Results;

namespace PokeGame.Core.Common.Extensions;

public static class ValidationFailureExtensions
{
    public static string ToExceptionMessage(this IEnumerable<ValidationFailure> validationFailures)
    {
        var sb = new StringBuilder();

        foreach (var x in validationFailures)
        {
            sb.AppendLine($"{x.ErrorMessage}. ");
        }
        
        return sb.ToString();
    }
}
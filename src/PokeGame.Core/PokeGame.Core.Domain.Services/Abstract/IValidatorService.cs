namespace PokeGame.Core.Domain.Services.Abstract;

internal interface IValidatorService
{
    Task ValidateAndThrowAsync<TModel>(TModel modelToValidate, CancellationToken cancellationToken = default)
        where TModel : class;
}
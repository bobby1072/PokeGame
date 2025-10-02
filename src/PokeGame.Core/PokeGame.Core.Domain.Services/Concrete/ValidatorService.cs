using System.Net;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PokeGame.Core.Common.Exceptions;
using PokeGame.Core.Common.Extensions;
using PokeGame.Core.Domain.Services.Abstract;

namespace PokeGame.Core.Domain.Services.Concrete;

internal sealed class ValidatorService: IValidatorService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ValidatorService> _logger;
    public ValidatorService(IServiceProvider serviceProvider, ILogger<ValidatorService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task ValidateAndThrowAsync<TModel>(TModel modelToValidate, CancellationToken cancellationToken = default) where TModel : class
    {
        var foundValidator = _serviceProvider.GetService<IValidator<TModel>>();

        if (foundValidator == null)
        {
            _logger.LogWarning("No validator found for type: {ModelTypeName}", typeof(TModel).Name);
            return;
        }
        
        var validationResult = await foundValidator.ValidateAsync(modelToValidate, cancellationToken);

        if (!validationResult.IsValid)
        {
            _logger.LogInformation("Model of type: {ModelTypeName} validation failed", typeof(TModel).Name);
            
            throw new PokeGameApiUserException(HttpStatusCode.BadRequest, validationResult.Errors.ToExceptionMessage());
        }
        
        _logger.LogInformation("Validation succeeded for model of type: {ModelTypeName}", typeof(TModel).Name);
    }
}
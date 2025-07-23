﻿using AutoFixture;
using BT.Common.Persistence.Shared.Models;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using PokeGame.Core.Domain.Services.Pokedex.Commands;
using PokeGame.Core.Persistence.Repositories.Abstract;
using PokeGame.Core.Schemas;

namespace PokeGame.Core.Domain.Services.Tests.PokdexTests;

public sealed class CreatePokedexPokemonCommandTests
{
    private static readonly Fixture _fixture = new();
    private readonly Mock<IPokedexPokemonRepository> _mockPokedexPokemonRepository = new();
    private readonly CreatePokedexPokemonCommand _command;

    public CreatePokedexPokemonCommandTests()
    {
        _command = new CreatePokedexPokemonCommand(
            _mockPokedexPokemonRepository.Object,
            new NullLogger<CreatePokedexPokemonCommand>()
        );
    }
    [Theory]
    [ClassData(typeof(ExecuteAsync_Should_Create_Only_New_Pokemon_ClassData))]
    public async Task ExecuteAsync_Should_Create_Only_New_Pokemon(
        IReadOnlyCollection<PokedexPokemon> existingPokemon, 
        IReadOnlyCollection<PokedexPokemon> input,
        IReadOnlyCollection<PokedexPokemon> expected,
        bool shouldHaveCalledCreateDbMethod)
    {
        //Arrange
        _mockPokedexPokemonRepository
            .Setup(x => x.GetAll())
            .ReturnsAsync(new DbGetManyResult<PokedexPokemon>(existingPokemon));

        if (shouldHaveCalledCreateDbMethod)
        {
            _mockPokedexPokemonRepository
                .Setup(x => x.Create(It.IsAny<IReadOnlyCollection<PokedexPokemon>>()))
                .ReturnsAsync(new DbSaveResult<PokedexPokemon>(expected));
        }
        
        //Act
        var result = await _command.ExecuteAsync(input);
        
        //Assert
        _mockPokedexPokemonRepository.Verify(x => x.GetAll(), Times.Once);
        
        if (!shouldHaveCalledCreateDbMethod)
        {
            Assert.Equal(expected, result);
        }
        else
        {
            _mockPokedexPokemonRepository.Verify(x => x.Create(expected), Times.Once);
        }
    }
    private sealed class ExecuteAsync_Should_Create_Only_New_Pokemon_ClassData : TheoryData<
        IReadOnlyCollection<PokedexPokemon>, IReadOnlyCollection<PokedexPokemon>, IReadOnlyCollection<PokedexPokemon>, bool>
    {
        public ExecuteAsync_Should_Create_Only_New_Pokemon_ClassData()
        {
            var random = new Random();
            var originalPokemon = _fixture
                .Build<PokedexPokemon>()
                .With(x => x.Id, random.Next(0, int.MaxValue))
                .CreateMany()
                .ToArray();
            var newPokemon = _fixture
                .Build<PokedexPokemon>()
                .With(x => x.Id, random.Next(0, int.MaxValue))
                .CreateMany()
                .ToArray();
            Add(originalPokemon, originalPokemon, [], false);
            Add(originalPokemon, [], [], false);
            Add(originalPokemon, originalPokemon.Union(newPokemon).ToArray(), newPokemon, true);
        }
    }
}
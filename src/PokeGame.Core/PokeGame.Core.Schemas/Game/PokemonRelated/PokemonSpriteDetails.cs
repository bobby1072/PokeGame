namespace PokeGame.Core.Schemas.Game.PokemonRelated;

public sealed class PokemonSpriteDetails: DomainModel<PokemonSpriteDetails>
{
    public required string FrontDefault { get; set; }
    public required string FrontShiny { get; set; }
    public required string BackDefault { get; set; }
    public required string BackShiny { get; set; }
    public string? FrontFemale { get; set; }
    public string? FrontShinyFemale { get; set; }
    public string? BackFemale { get; set; }
    public string? BackShinyFemale { get; set; }

    public override bool Equals(PokemonSpriteDetails? other)
    {
        return FrontDefault == other?.FrontDefault && 
               FrontShiny == other.FrontShiny &&
               BackDefault == other.BackDefault &&
               BackShiny == other.BackShiny &&
               FrontFemale == other.FrontFemale &&
               FrontShinyFemale == other.FrontShinyFemale &&
               BackFemale == other.BackFemale &&
               BackShinyFemale == other.BackShinyFemale;
    }
}
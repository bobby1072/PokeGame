namespace PokeGame.Core.Api.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
internal sealed class RequireValidUserIdHeaderAttribute: Attribute
{ }
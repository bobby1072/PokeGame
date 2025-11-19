namespace PokeGame.Core.Common.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public sealed class RequireValidUserIdHeaderAttribute: Attribute
{ }
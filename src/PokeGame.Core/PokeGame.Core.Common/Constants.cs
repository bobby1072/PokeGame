namespace PokeGame.Core.Common;

public static class Constants
{
    public static class ExceptionConstants
    {
        public const string InternalError = "An internal error occurred";
        public const string MissingEnvVars = "Missing environment variables";
    }

    public static class ServiceKeys
    {
        public const string PokedexJsonFile = "PokedexJsonFile";
    }

    public static class ApiConstants
    {
        public const string UserIdHeaderKey =  "UserId";
        public const string GameSaveIdHeaderKey =  "GameSaveId";
    }
}

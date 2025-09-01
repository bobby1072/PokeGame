using System.Text;

namespace PokeGame.Core.ConsoleApp.Helpers;

internal static class ConsoleHelper
{
    public static string GetConsoleNewLine(int numberOf = 1)
    {
        if (numberOf == 1)
            return Environment.NewLine;
        var newLineBuilder = new StringBuilder();
        for (int i = 0; i < numberOf; i++)
        {
            newLineBuilder.Append(Environment.NewLine);
        }

        return newLineBuilder.ToString();
    }
}
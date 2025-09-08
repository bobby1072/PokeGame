using System.Text;

namespace PokeGame.Core.ConsoleApp.Helpers;

internal static class ConsoleHelper
{
    public static string CreateNewLines(int numberOf = 1)
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
    
    private static int GetChoice(IEnumerable<string> options)
    {
        int selectedIndex = 0;
        int optionsCount = options.Count();
        ConsoleKey key;
        while (true)
        {
            do
            {
                Console.WriteLine();

                for (int i = 0; i < optionsCount; i++)
                {
                    if (i == selectedIndex)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("> " + options.ElementAt(i));
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.WriteLine("  " + options.ElementAt(i));
                    }
                }

                ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                key = keyInfo.Key;

                if (key == ConsoleKey.UpArrow && selectedIndex > 0)
                {
                    selectedIndex--;
                }
                else if (key == ConsoleKey.DownArrow && selectedIndex < optionsCount - 1)
                {
                    selectedIndex++;
                }
                Console.Clear();
            } while (key != ConsoleKey.Enter);

            if (selectedIndex > optionsCount || selectedIndex < 0)
            {
                Console.WriteLine($"{CreateNewLines()}Please choose a valid option{CreateNewLines()}");
                continue;
            }
            break;
        }

        return selectedIndex + 1;
    }
}
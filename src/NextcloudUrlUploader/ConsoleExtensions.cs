namespace NextcloudUrlUploader;

using System;

internal static class ConsoleExtensions
{
    public static void WriteLineToConsole(this string text, ConsoleColor foreground, ConsoleColor? background = null) => WriteLineToConsoleInternal(text, isWithNewLine: true, foreground, background);

    public static void WriteToConsole(this string text, ConsoleColor foreground, ConsoleColor? background = null) => WriteLineToConsoleInternal(text, isWithNewLine: false, foreground, background);

    private static void WriteLineToConsoleInternal(string text, bool isWithNewLine, ConsoleColor foreground, ConsoleColor? background = null)
    {
        ConsoleColor prevFore = Console.ForegroundColor;
        ConsoleColor prevBack = Console.BackgroundColor;

        try
        {
            Console.ForegroundColor = foreground;
            if (background.HasValue)
            {
                Console.BackgroundColor = background.Value;
            }

            if (isWithNewLine)
            {
                Console.WriteLine(text);
            }
            else
            {
                Console.Write(text);
            }
        }
        finally
        {
            Console.ForegroundColor = prevFore;
            Console.BackgroundColor = prevBack;
        }
    }
}
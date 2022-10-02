namespace Egrep_Cpy.Log;

public static class Logger
{
    public static void Log(object message, ConsoleColor color = ConsoleColor.White, bool line = true, bool timestamp = false)
    {
        Console.ForegroundColor = color;

        if (timestamp)
        {
            Console.Write($"[{DateTime.Now.ToString("HH:mm:ss.ff")}] - ");
        }

        if (line)
        {
            Console.WriteLine(message);
        }
        else
        {
            Console.Write(message);
        }

        Console.ResetColor();
    }

    public static void LogError(object message, bool line = true, bool timestamp = false)
    {
        Log(message, ConsoleColor.Red, line, timestamp);
    }

    public static void LogWarning(object message, bool line = true, bool timestamp = false)
    {
        Log(message, ConsoleColor.Yellow, line, timestamp);
    }

    public static void LogSuccess(object message, bool line = true, bool timestamp = false)
    {
        Log(message, ConsoleColor.Green, line, timestamp);
    }

    public static void LogInfo(object message, bool line = true, bool timestamp = false)
    {
        Log(message, ConsoleColor.Blue, line, timestamp);
    }

    public static void PrettyLog(params PrettyString[] messages)
    {
        var lastColor = Console.ForegroundColor;
        foreach (var item in messages)
        {
            Log(item.Text, item.Color, false);
        }

        Console.ForegroundColor = lastColor;
        Console.WriteLine();
    }
}
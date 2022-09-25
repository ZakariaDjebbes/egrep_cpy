namespace EGREP_CPY;

public static class Logger
{
    public static void Log(object message, bool line = true, ConsoleColor color = ConsoleColor.White)
    {
        Console.ForegroundColor = color;

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

    public static void LogError(object message, bool line = true)
    {
        Log(message, line, ConsoleColor.Red);
    }
    
    public static void LogWarning(object message, bool line = true)
    {
        Log(message, line, ConsoleColor.Yellow);
    }

    public static void LogSuccess(object message, bool line = true)
    {
        Log(message, line, ConsoleColor.Green);
    }

    public static void LogInfo(object message, bool line = true)
    {
        Log(message, line, ConsoleColor.Blue);
    }
}
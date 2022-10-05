namespace EgrepCpy.Error;

public class InvalidRegExException : Exception
{
    public int Failure { get; set; }

    public InvalidRegExException(string message, int failure) : base(message)
    {
        Failure = failure;
    }
}
namespace ToGlobalUsing;

public static class ArgumentValidation
{
    public static void Validate(this string[] args)
    {
        if (args.Length <= 0)
        {
            Console.WriteLine("Arguments missing");
            Environment.Exit(-1);
        }

        if (!File.Exists(args[0]))
        {
            Console.WriteLine("File not found");
            Environment.Exit(-1);
        }
    }
}
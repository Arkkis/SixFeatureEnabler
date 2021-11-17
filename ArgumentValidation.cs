namespace SixFeatureEnabler;

public static class ArgumentValidation
{
    public static void Validate(this string[] args)
    {
        if (args.Length <= 0)
        {
            Console.WriteLine("Usage: SixFeatureEnabler.exe \"FullPathToSlnOrCsproj\"");
            Environment.Exit(-1);
        }

        if (!File.Exists(args[0]))
        {
            Console.WriteLine("File not found");
            Console.WriteLine("Usage: SixFeatureEnabler.exe \"FullPathToSlnOrCsproj\"");
            Environment.Exit(-1);
        }
    }
}
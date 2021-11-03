namespace ToGlobalUsing;

public static class PathExtensions
{
    public static void ListFilesAndDirectoriesRecursively(this List<string> list, string path)
    {
        foreach (var file in Directory.GetFiles(path))
        {
            if (Path.GetExtension(file) is ".cs")
            {
                if (!IfPathIsIgnored(file))
                {
                    list.Add(file);
                }
            }
        }

        foreach (var directory in Directory.GetDirectories(path))
        {
            if (!IfPathIsIgnored(path))
            {
                list.ListFilesAndDirectoriesRecursively(directory);
            }
        }

        static bool IfPathIsIgnored(string path)
        {
            var ignoredPaths = new List<string>
                {
                    $"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}"
                };

            foreach (var ignoredPath in ignoredPaths)
            {
                if (path.Contains(ignoredPath))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
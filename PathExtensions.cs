namespace SixFeatureEnabler;

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

    public static void CreateProjectPathList(this List<string> projectList, string filePath, bool withFilenames = false)
    {
        projectList.Clear();

        var extension = Path.GetExtension(filePath);

        switch (extension)
        {
            case ".sln":
                foreach (var line in File.ReadAllLines(filePath))
                {
                    var trimLine = line.Trim();
                    if (trimLine.StartsWith("Project"))
                    {
                        var splits = trimLine.Split(',');
                        var projectFile = string.Empty;

                        foreach (var split in splits)
                        {
                            if (split.Contains(".csproj"))
                            {
                                projectFile = split.Trim().Replace("\"", "");

                                if (!Path.IsPathRooted(projectFile))
                                {
                                    projectFile = Path.Combine(Path.GetDirectoryName(filePath), projectFile);
                                }
                            }
                        }

                        if (!projectList.Contains(projectFile) && File.Exists(projectFile))
                        {
                            Console.WriteLine(projectFile);

                            string projectPath;

                            if (!withFilenames)
                            {
                                projectPath = Path.GetDirectoryName(projectFile);
                            }
                            else
                            {
                                projectPath = projectFile;
                            }

                            if (projectPath is not null)
                            {
                                projectList.Add(projectPath);
                            }
                        }
                    }
                }
                break;

            case ".csproj":
                Console.WriteLine(filePath);

                var path = Path.GetDirectoryName(filePath);

                if (path == null)
                {
                    Console.WriteLine("Can't find path");
                    Environment.Exit(-1);
                }

                projectList.Add(path);
                break;

            default:
                Console.WriteLine("Wrong file type. Use sln or csproj.");
                Environment.Exit(-1);
                break;
        }
    }
}
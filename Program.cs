args.Validate();

Console.Clear();
Console.WriteLine("Projects found:");

var projectList = new List<string>();
var projectListWithFilenames = new List<string>();
Console.WriteLine();

Selection:
Console.WriteLine("---------------------------------------------");
Console.WriteLine("-              Choose action                -");
Console.WriteLine("- 1) Convert all usings to global           -");
Console.WriteLine("- 2) Convert to file scoped namespaces      -");
Console.WriteLine("- 3) Enable .NET 6 + C# 10 for all projects -");
Console.WriteLine("- 4) Do everything                          -");
Console.WriteLine("---------------------------------------------");
Console.WriteLine("- 5) Exit                                   -");
Console.WriteLine("---------------------------------------------");

Console.Write("Selection: ");
var key = Console.ReadKey().Key;

var noticeAboutCleanup = false;
var classFileList = new List<string>();
var upgradeVersions = false;

projectList.CreateProjectPathList(args[0]);

foreach (var path in projectList)
{
    classFileList.ListFilesAndDirectoriesRecursively(path);

    switch (key)
    {
        case ConsoleKey.D1:
        case ConsoleKey.NumPad1:
            {
                var usingFile = Path.Combine(path, "Usings.cs");

                if (!File.Exists(usingFile))
                {
                    File.WriteAllText(usingFile, "", Encoding.UTF8);
                }

                UsingService.RemoveUsings(classFileList, usingFile);
                break;
            }

        case ConsoleKey.D2:
        case ConsoleKey.NumPad2:
            {
                NamespaceService.FileScopeNamespaces(classFileList);
                noticeAboutCleanup = true;
                break;
            }

        case ConsoleKey.D3:
        case ConsoleKey.NumPad3:
            {
                upgradeVersions = true;
                break;
            }

        case ConsoleKey.D4:
        case ConsoleKey.NumPad4:
            {
                var usingFile = Path.Combine(path, "Usings.cs");

                if (!File.Exists(usingFile))
                {
                    File.WriteAllText(usingFile, "", Encoding.UTF8);
                }

                UsingService.RemoveUsings(classFileList, usingFile);
                NamespaceService.FileScopeNamespaces(classFileList);

                noticeAboutCleanup = true;
                upgradeVersions = true;
                break;
            }

        case ConsoleKey.D5:
        case ConsoleKey.NumPad5:
            {
                Environment.Exit(0);
                break;
            }

        default:
            Console.Clear();
            goto Selection;
    }

    classFileList.Clear();
}

if (upgradeVersions)
{
    projectListWithFilenames.CreateProjectPathList(args[0], withFilenames: true);
    VersionUpgradeService.UpgradeAllProjects(projectListWithFilenames);
}

if (noticeAboutCleanup)
{
    Console.WriteLine();
    Console.WriteLine();
    Console.WriteLine("To fix indentation in edited files, please run Code Cleanup");
    Console.WriteLine();
}

Console.WriteLine();
Console.WriteLine($"Edited {RuntimeVariables.FilesEditedCount} files");
Console.WriteLine($"Removed {RuntimeVariables.LinesRemovedCount} lines");
Console.WriteLine();
Console.WriteLine("Press any key to exit");
Console.ReadKey();
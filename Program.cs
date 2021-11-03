args.Validate();

var linesRemovedCount = 0;
var filesEditedCount = 0;

Console.WriteLine("Projects found:");

var projectList = new List<string>();
projectList.CreateProjectPathList(args[0]);

Console.WriteLine();
Console.WriteLine("Files edited:");

var classFileList = new List<string>();

foreach (var path in projectList)
{
    var usingFile = Path.Combine(path, "Usings.cs");

    if (!File.Exists(usingFile))
    {
        File.WriteAllText(usingFile, "", Encoding.UTF8);
    }

    classFileList.ListFilesAndDirectoriesRecursively(path);

    var addedUsings = new List<string>();

    foreach (var classFile in classFileList)
    {
        var classFileLines = File.ReadAllLines(classFile);

        if (classFileLines.All(line => !line.Contains("namespace")))
        {
            continue;
        }

        var fileWasEdited = false;

        foreach (var line in classFileLines)
        {
            var trimmedLine = line.Trim();

            if (trimmedLine.StartsWith("using ") && !trimmedLine.Contains("="))
            {
                var newUsingLine = $"global {trimmedLine + Environment.NewLine}";

                if (!addedUsings.Contains(newUsingLine))
                {
                    File.AppendAllText(usingFile, newUsingLine);
                    addedUsings.Add(newUsingLine);
                    fileWasEdited = true;
                }
            }
        }

        if (fileWasEdited)
        {
            Console.WriteLine(classFile);
            filesEditedCount++;
        }

        var usingsToRemove = new List<string>();

        foreach (var line in classFileLines)
        {
            if (line.Contains("namespace"))
            {
                break;
            }

            var trimmedLine = line.Trim();

            if (trimmedLine.StartsWith("using ") && !trimmedLine.Contains("="))
            {
                linesRemovedCount++;
                usingsToRemove.Add(line);
            }
        }

        var newFile = string.Empty;

        foreach (var line in classFileLines)
        {
            if (!usingsToRemove.Contains(line))
            {
                newFile += line;

                if (line != classFileLines.Last())
                {
                    newFile += Environment.NewLine;
                }
            }
        }

        File.WriteAllText(classFile, newFile.TrimStart(), Encoding.UTF8);
    }

    classFileList.Clear();
}

Console.WriteLine();
Console.WriteLine($"Edited {filesEditedCount} files");
Console.WriteLine($"Removed {linesRemovedCount} lines");
Console.WriteLine();
Console.WriteLine("Press any key to exit");
Console.ReadKey();
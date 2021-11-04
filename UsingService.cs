namespace ToGlobalUsing;

public class UsingService
{
    public void RemoveUsings(List<string> classFileList, string usingFile)
    {
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

                if (trimmedLine.StartsWith("using ") && !trimmedLine.Contains('='))
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
                RuntimeVariables.FilesEditedCount++;
            }

            var usingsToRemove = new List<string>();

            foreach (var line in classFileLines)
            {
                if (line.Contains("namespace"))
                {
                    break;
                }

                var trimmedLine = line.Trim();

                if (trimmedLine.StartsWith("using ") && !trimmedLine.Contains('='))
                {
                    RuntimeVariables.LinesRemovedCount++;
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
    }
}
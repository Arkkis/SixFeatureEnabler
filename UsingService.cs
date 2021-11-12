namespace SixFeatureEnabler;

public class UsingService
{
    public void RemoveUsings(List<string> classFileList, string usingFile)
    {
        var addedUsings = new List<string>();

        foreach (var classFile in classFileList)
        {
            string[] fileLines;

            using (StreamReader sr = new StreamReader(classFile))
            {
                fileLines = sr.ReadToEnd().Split('\n');
            }

            if (fileLines.All(line => !line.Contains("namespace")))
            {
                continue;
            }

            var fileWasEdited = false;

            foreach (var line in fileLines)
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
                //Console.WriteLine(classFile);
                RuntimeVariables.FilesEditedCount++;
            }

            var usingsToRemove = new List<string>();

            foreach (var line in fileLines)
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

            for (int i = 0; i < fileLines.Length; i++)
            {
                var line = fileLines[i];

                var hasNewLine = line.LastOrDefault() == '\r';

                if (!usingsToRemove.Contains(line))
                {
                    newFile += line.TrimEnd();

                    if (i != fileLines.Length - 1 && hasNewLine)
                    {
                        newFile += Environment.NewLine;
                    }
                }
            }

            File.WriteAllText(classFile, newFile.TrimStart(), Encoding.UTF8);
        }
    }
}
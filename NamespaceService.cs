namespace SixFeatureEnabler;

public class NamespaceService
{
    public static void FileScopeNamespaces(List<string> classFileList)
    {
        foreach (var classFile in classFileList)
        {
            string[] fileLines;

            using (StreamReader sr = new(classFile))
            {
                fileLines = sr.ReadToEnd().Split('\n');
            }

            var namespaceFound = false;
            var namespaceIndex = 0;

            var firstBracketFound = false;
            var lastBracketFound = false;
            var linesToRemoved = new List<int>();

            for (int i = 0; i < fileLines.Length; i++)
            {
                if (lastBracketFound)
                {
                    break;
                }

                var line = fileLines[i];
                var lineLastCharacterWithoutLinebreak = line.TrimEnd().LastOrDefault();

                if (line.Contains("namespace ") && lineLastCharacterWithoutLinebreak != ';')
                {
                    namespaceIndex = i;
                    linesToRemoved.Add(i);
                    namespaceFound = true;
                    continue;
                }

                if (namespaceFound && !firstBracketFound)
                {
                    if (line.Trim() is "{")
                    {
                        linesToRemoved.Add(i);
                        firstBracketFound = true;
                        continue;
                    }
                }

                if (namespaceFound && firstBracketFound && !lastBracketFound)
                {
                    var lastLineOffset = 0;

                    while (!lastBracketFound)
                    {
                        var lineNumber = fileLines.Length - 1 - lastLineOffset;

                        if (fileLines[lineNumber].StartsWith("}"))
                        {
                            linesToRemoved.Add(lineNumber);
                            lastBracketFound = true;
                            break;
                        }

                        lastLineOffset++;
                    }
                }
            }

            if (!lastBracketFound)
            {
                continue;
            }

            var newFile = "";

            for (int i = 0; i < fileLines.Length; i++)
            {
                var line = fileLines[i];

                var hasNewLine = line.LastOrDefault() == '\r';

                if (namespaceIndex == i)
                {
                    RuntimeVariables.LinesRemovedCount += 3;
                    newFile += line.TrimEnd() + ";";
                }

                if (!linesToRemoved.Contains(i))
                {
                    newFile += line.TrimEnd();
                }

                if (i != fileLines.Length - 1 && i != fileLines.Length - 2 && hasNewLine)
                {
                    newFile += Environment.NewLine;
                }
            }

            RuntimeVariables.FilesEditedCount++;
            File.WriteAllText(classFile, newFile.TrimStart(), Encoding.UTF8);
        }
    }
}
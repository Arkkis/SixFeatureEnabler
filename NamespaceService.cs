namespace ToGlobalUsing;

public class NamespaceService
{
    public void FileScopeNamespaces(List<string> classFileList)
    {
        foreach (var classFile in classFileList)
        {
            var fileLines = File.ReadAllLines(classFile);

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

                if (line.Contains("namespace ") && !line.EndsWith(';'))
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
                if (namespaceIndex == i)
                {
                    RuntimeVariables.LinesRemovedCount += 3;
                    newFile += fileLines[i] + ";" + Environment.NewLine + Environment.NewLine;
                }

                if (!linesToRemoved.Contains(i))
                {
                    newFile += fileLines[i];

                    if (i != fileLines.Length - 1)
                    {
                        newFile += Environment.NewLine;
                    }
                }
            }

            RuntimeVariables.FilesEditedCount++;
            File.WriteAllText(classFile, newFile.TrimStart(), Encoding.UTF8);
        }
    }
}
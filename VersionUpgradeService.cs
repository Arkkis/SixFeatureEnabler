namespace SixFeatureEnabler;

public class VersionUpgradeService
{
    public void UpgradeAllProjects(List<string> projectFileList)
    {
        foreach (var projectFile in projectFileList)
        {
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
        }
    }
}
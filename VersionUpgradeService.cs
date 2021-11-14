using System.IO;

namespace SixFeatureEnabler;

public class VersionUpgradeService
{
    public void UpgradeAllProjects(List<string> projectFileList)
    {
        foreach (var projectFile in projectFileList)
        {
            string[] fileLines;

            using (StreamReader sr = new StreamReader(projectFile))
            {
                fileLines = sr.ReadToEnd().Split('\n');
            }

            var langVersionFound = false;

            if (fileLines.Any(line => line.ToLower().Contains("<LangVersion>".ToLower())))
            {
                langVersionFound = true;
            }

            var newFile = string.Empty;

            for (int i = 0; i < fileLines.Length; i++)
            {
                var line = fileLines[i];
                var hasNewLine = line.LastOrDefault() == '\r';
                var indent = string.Empty;

                if (line.Contains("<"))
                {
                    indent = line.Split('<')[0];
                }

                if (line.ToLower().Contains("<TargetFramework>".ToLower()))
                {
                    if (!line.ToLower().Contains("<TargetFramework>net6.0</TargetFramework>".ToLower()))
                    {
                        newFile += indent + "<TargetFramework>net6.0</TargetFramework>".TrimEnd();
                    }
                    else
                    {
                        newFile += line.TrimEnd();
                    }

                    if (!langVersionFound)
                    {
                        newFile += Environment.NewLine + indent + "<LangVersion>10</LangVersion>".TrimEnd();
                    }
                }
                else if (langVersionFound && line.ToLower().Contains("<LangVersion>".ToLower()))
                {
                    if (!line.ToLower().Contains("<LangVersion>10</LangVersion>".ToLower()))
                    {
                        newFile += indent + "<LangVersion>10</LangVersion>".TrimEnd();
                    }
                    else
                    {
                        newFile += line.TrimEnd();
                    }
                }
                else
                {
                    newFile += line.TrimEnd();
                }

                if (i != fileLines.Length - 1 && hasNewLine)
                {
                    newFile += Environment.NewLine;
                }
            }

            File.WriteAllText(projectFile, newFile.TrimStart(), Encoding.UTF8);
        }
    }
}
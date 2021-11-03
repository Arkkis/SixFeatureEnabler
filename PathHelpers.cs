namespace ToGlobalUsing;

internal class PathHelpers
{
    public List<string> CreateProjectPathList(string filePath)
    {
        var projectList = new List<string>();

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

                if (!projectList.Contains(projectFile))
                {
                    Console.WriteLine(projectFile);

                    var path = Path.GetDirectoryName(projectFile);

                    if (path is not null)
                    {
                        projectList.Add(path);
                    }
                }
            }
        }

        return projectList;
    }
}

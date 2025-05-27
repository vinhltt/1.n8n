using System.Reflection;

namespace CoreFinance.Contracts.Utilities;

public class FileHelper
{
    public static async Task<string> ReadFile(
        string filepath)
    {
        if (!File.Exists(filepath))
            throw new FileNotFoundException(filepath);

        var result = await File.ReadAllTextAsync(filepath);

        return result;
    }

    public static string GetApplicationFolder()
    {
        var path = Assembly.GetExecutingAssembly()
            .Location;

        return Path.GetDirectoryName(path)
               ?? throw new Exception(
                   "FolderNotFoundException");
    }
}
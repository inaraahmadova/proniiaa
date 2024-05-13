using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pronia.Extensions;
public static class FileExtension
{
    public static bool IsValidType(this IFormFile file, string type)
        => file.ContentType.Contains(type);
    public static bool IsValidLength(this IFormFile file, int kb)
        => file.Length <= kb * 1024;
    public static async Task<string> SaveFileAsync(this IFormFile file, string path)
    {
        string ext = Path.GetExtension(file.FileName);
        string newName = Path.GetRandomFileName();
        await using FileStream fs = new FileStream(Path.Combine(path, newName + ext), FileMode.Create);
        await file.CopyToAsync(fs);
        return newName + ext;
    }
}

using System.Text;

namespace Pronia.Extensions
{
    public static class ListExtension
    {
        public static string Bind<T>(this IEnumerable<string>? list, char letter)
        {
            if (list == null) return string.Empty;
            StringBuilder sb = new StringBuilder();
            foreach (string item in list)
            {
                sb.Append(item);
                sb.Append(letter);
            }
            sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }
    }
}

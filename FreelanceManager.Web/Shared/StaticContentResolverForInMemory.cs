using System.IO;

namespace FreelanceManager.Web.Shared
{
    public class StaticContentResolverForInMemory : IStaticContentResolver
    {
        public string GetLocation(string item)
        {
            if (item == null)
                return null;

            if (!item.StartsWith("/"))
                item = "/" + item;

            var fullProjectPath = "";
            var directoryName = Path.GetDirectoryName(typeof(Bootstrapper).Assembly.CodeBase);
            if (directoryName != null)
            {
                var assemblyPath = directoryName.Replace(@"file:\", string.Empty);
                fullProjectPath = Path.Combine(assemblyPath, "..", "..", "..", "FreelanceManager.Web");
            }

            return fullProjectPath + item;
        }
    }
}
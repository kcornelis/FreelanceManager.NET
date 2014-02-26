namespace FreelanceManager.Web.Shared
{
    public class StaticContentResolverForWeb : IStaticContentResolver
    {
        public string GetLocation(string item)
        {
            if (item == null)
                return null;

            if (!item.StartsWith("/"))
                item = "/" + item;

            return "~" + item;
        }
    }
}
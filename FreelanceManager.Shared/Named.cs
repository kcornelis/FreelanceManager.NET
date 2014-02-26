namespace FreelanceManager
{
    public interface INamed
    {
        string Id { get; set; }
        string Name { get; set; }
    }

    public class Named : INamed
    {
        public string Id
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }
    }

    public class NamedReference<T> where T : INamed
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public static implicit operator NamedReference<T>(T doc)
        {
            return new NamedReference<T>
            {
                Id = doc.Id,
                Name = doc.Name
            };
        }
    }
}

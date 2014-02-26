namespace FreelanceManager.Web
{
    public class Initialize
    {
        static bool _initialized;
        static object _lock = new object();
        
        public static void ForWeb()
        {
            if (_initialized)
                return;

            lock (_lock)
            {
                if (_initialized)
                    return;

                Bootstrapper.Context = Shared.ExecutionContext.InMemory;

                _initialized = true;
            }
        }
    }
}

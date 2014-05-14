namespace FreelanceManager.Performance
{
    public static class Config
    {
        //public static string Url = "https://fm-kcornelis.cloudapp.net:8443";
        public static string Url = "http://localhost:3443";
        public static string AdminEmail = "kevin.cornelis@outlook.com";
        public static string AdminPassword = "kevin";

        public static int Accounts = 1; // only 1 works
        public static int Clients = 1;
        public static int Projects = 1;
        public static int DurationInMonths = 12;
        public static int TimeRegistrationsPerDay = 1;
    }
}

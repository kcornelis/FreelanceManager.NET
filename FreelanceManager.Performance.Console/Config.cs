namespace FreelanceManager.Performance.Console
{
    public static class Config
    {
        public static string Url = "https://fm-kcornelis.cloudapp.net:8443";
        public static string AdminEmail = "kevin.cornelis@outlook.com";
        public static string AdminPassword = "kevin";

        public static int Accounts = 1; // only 1 works
        public static int Clients = 5;
        public static int Projects = 20;
        public static int DurationInMonths = 12;
        public static int TimeRegistrationsPerDay = 30;
    }
}

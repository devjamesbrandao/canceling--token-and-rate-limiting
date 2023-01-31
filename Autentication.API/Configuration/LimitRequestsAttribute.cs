namespace Autentication.API.Configuration
{
    [AttributeUsage(AttributeTargets.Method)]
    public class LimitRequests : Attribute
    {
        public int TimeWindowInSeconds { get; set; }
        public int MaxRequests { get; set; }
    }
}
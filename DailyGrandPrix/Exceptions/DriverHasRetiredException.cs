namespace DailyGrandPrix.Exceptions
{
    internal class DriverHasRetiredException : ApplicationException
    {
        public DriverHasRetiredException(string message) : base(message) { }
    }
}

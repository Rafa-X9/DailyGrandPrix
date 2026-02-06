namespace DailyGrandPrix.Exceptions
{
    internal class DriverAlreadyFinishedException : ApplicationException
    {
        public DriverAlreadyFinishedException(string message) : base(message) { }
    }
}

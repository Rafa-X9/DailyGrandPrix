namespace DailyGrandPrix.Exceptions
{
    internal class DriverAlreadyInException : ApplicationException
    {
        public DriverAlreadyInException(string message) : base(message) { }
    }
}

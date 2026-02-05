namespace DailyGrandPrix.Exceptions
{
    internal class NotConfirmedException : ApplicationException
    {
        public NotConfirmedException(string message) : base(message) { }
    }
}

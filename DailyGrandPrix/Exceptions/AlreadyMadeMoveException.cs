namespace DailyGrandPrix.Exceptions
{
    internal class AlreadyMadeMoveException : ApplicationException
    {
        public AlreadyMadeMoveException(string message) : base(message) { }
    }
}

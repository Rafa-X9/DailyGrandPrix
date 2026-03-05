namespace DailyGrandPrix.Exceptions
{
    internal class OutOfFuel : ApplicationException
    {
        public OutOfFuel() { }
        public OutOfFuel(string message) : base(message)
        {
        }
    }
}

namespace DailyGrandPrix.Services
{
    internal static class InputService
    {
        public static int GetIntInput(string message = "Enter a number")
        {
            Console.WriteLine(message);
            Console.Write("> ");
            if (!int.TryParse(Console.ReadLine(), out int input))
            {
                throw new FormatException("Invalid numeric input");
            }
            return input;
        }
    
        public static string GetStringInput(string message = "Enter text", bool allowEmpty = false)
        {
            Console.WriteLine(message);
            Console.Write("> ");
            string? input = Console.ReadLine();
            if (input is null || (string.IsNullOrWhiteSpace(input) && !allowEmpty))
            {
                throw new FormatException("Input was incorrectly null or empty.");
            }
            return input;
        }
    
        public static T GetEnumInput<T>(string message = "Enter enum value") where T : struct, Enum
        {
            string input = GetStringInput(message: message);
            if (!Enum.TryParse(input, out T result))
            {
                throw new FormatException("Invalid enum input.");
            }
            return result;
        }
    }
}

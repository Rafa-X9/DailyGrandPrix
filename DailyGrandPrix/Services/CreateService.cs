using DailyGrandPrix.Entities;
using DailyGrandPrix.Enums;

namespace DailyGrandPrix.Services
{
    internal class CreateService
    {
        public SaveService Saves { get; set; }

        public CreateService(SaveService saves)
        {
            Saves = saves;
        }

        public void CreateDriver()
        {
            try
            {
                Console.Write("Driver's name: ");
                string name = Console.ReadLine();
                Console.Write("Driver's username: ");
                string username = Console.ReadLine();
                Console.Write("Driver's number: ");
                int number = int.Parse(Console.ReadLine());
                Console.Write("Driver's team: ");
                Teams team = Enum.Parse<Teams>(Console.ReadLine());
                int id = Saves.Drivers.Count() + 1;
                Saves.Drivers.Add(new Driver(id, name, username, number, team));
            }
            catch (FormatException ex)
            {
                Console.WriteLine("Format error! " + ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("UNEXPECTED ERROR");
                Console.WriteLine(ex.Message);
            }
        }

        public void CreateTrack()
        {
            try
            {
                int id = Saves.Tracks.Count() + 1;
                Console.Write("Name: ");
                string name = Console.ReadLine();
                Console.Write("Steps per lap: ");
                int stepsPerLap = int.Parse(Console.ReadLine());
                Saves.Tracks.Add(new(id, name, stepsPerLap));
            }
            catch (FormatException ex)
            {
                Console.WriteLine("Format error! " + ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("UNEXPECTED ERROR");
                Console.WriteLine(ex.Message);
            }
        }
    }
}

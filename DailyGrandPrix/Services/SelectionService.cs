using DailyGrandPrix.Entities;
using DailyGrandPrix.Exceptions;

namespace DailyGrandPrix.Services
{
    internal static class SelectionSerivce
    {
        internal static Championship GetChampionship(SaveService saveService)
        {
            Console.Clear();
            foreach (Championship c in saveService.Championships)
            {
                Console.WriteLine(c.Id + " - " + c.Name + " - " + "Has " + c.Races.Count + " races");
            }
            Console.Write("> ");
            int choice = int.Parse(Console.ReadLine());
            return saveService.Championships.Where(ch => ch.Id == choice).First();

        }

        internal static Race GetRace(SaveService saveService)
        {
            Console.Clear();
            Championship champ = GetChampionship(saveService);
            foreach (Race r in champ.Races)
            {
                Console.WriteLine(r.Id + " - " + r.Track.Name + " - " + r.Drivers.Count + " drivers");
            }
            Console.Write("> ");
            int choice = int.Parse(Console.ReadLine());
            return champ.Races.Where(ra => ra.Id == choice).First();

        }

        internal static Track GetTrack(SaveService saveService)
        {
            foreach (Track t in saveService.Tracks)
            {
                Console.Clear();
                Console.WriteLine("Choose a track:");
                Console.WriteLine($"{t.Id} - {t.Name} - {t.StepsPerLap} steps a lap.");
            }
            Console.Write("> ");
            int choice = int.Parse(Console.ReadLine());
            return saveService.Tracks.Where(t => t.Id == choice).First();

        }

        internal static Driver GetDriver(SaveService saveService)
        {
            Console.Clear();
            Console.WriteLine("Type the Id of the driver:");
            foreach (Driver d in saveService.Drivers)
            {
                Console.WriteLine($"{d.Id} - {d.Name} - {d.Username}");
            }
            Console.Write("> ");
            int choice = int.Parse(Console.ReadLine());
            return saveService.Drivers.Where(dr => dr.Id == choice).First();

        }
    }
}
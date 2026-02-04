using DailyGrandPrix.Entities;
using DailyGrandPrix.Services;

namespace DailyGrandPrix
{
    class Program
    {
        public static void Main(string[] args)
        {
            SaveService saveService = new();
            CreateService createSerivce = new(saveService);

            saveService.ImportChampionships();
            saveService.ImportTracks();
            saveService.ImportRaces();
            foreach (Championship champ in saveService.Championships)
            {
                Console.WriteLine(champ.Name.ToUpper());
                foreach (Race race in champ.Races)
                {
                    Console.WriteLine("Race: " + race.Track.Name + ", started " + race.Start);
                }
            }
        }
    }
}
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

            foreach (Championship champ in saveService.Championships)
            {
                Console.WriteLine(champ.Name + " in " + champ.Year);
            }

            /*
                ===TO-DO LIST===
                1. Create methods to create, save, and import races
                2. Create methods to edit a driver's personal information
                3. Make race's processing shenanigans
            */
        }
    }
}
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

            /*
                ===TO-DO LIST===
                1. Create methods to save and import tracks
                2. Create methods to create, save, and import championships
                3. Create methods to create, save, and import races
                4. Create methods to edit a driver's personal information
                5. Make race's processing shenanigans
            */
        }
    }
}
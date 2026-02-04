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
                1. Create methods to create, save, and import championships
                2. Create methods to create, save, and import races
                3. Create methods to edit a driver's personal information
                4. Make race's processing shenanigans
            */
        }
    }
}
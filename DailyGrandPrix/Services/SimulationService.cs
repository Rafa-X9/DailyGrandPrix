using DailyGrandPrix.Entities;
using DailyGrandPrix.Enums;

namespace DailyGrandPrix.Services
{
    internal sealed class SimulationService
    {
        public DriverRace DriverRace { get; set; }
        public int[] PitStops { get; set; }
        public Tyres[] Tyres { get; set; }
        public Tyres Starting { get; set; }
        public int StartingFuel { get; set; }
        public List<Actions> Actions { get; set; } = new List<Actions>();

        public SimulationService(DriverRace dr)
        {
            DriverRace = dr;
            Random random = new Random();
            int qPit = random.Next(1, 3);
            PitStops = new int[qPit];
            for (int i = 0; i < qPit; i++)
            {
                PitStops[i] = random.Next(3, 11);
            }
            if (qPit == 2)
            {
                while (PitStops[0] == PitStops[1])
                {
                    PitStops[1] = random.Next(3, 11);
                }
            }
            Tyres = new Tyres[qPit];
            for (int i = 0; i < qPit; i++)
            {
                int tyres = random.Next(1, 4);
                if (tyres == 1) Tyres[i] = Enums.Tyres.Softs;
                else if (tyres == 2) Tyres[i] = Enums.Tyres.Mediums;
                else if (tyres == 3) Tyres[i] = Enums.Tyres.Hards;
            }
            int fuel = random.Next(60, 101);
            StartingFuel = fuel;
            dr.FuelAmount = fuel;
            int tyre = random.Next(1, 4);
            Tyres t;
            if (tyre == 1) t = Enums.Tyres.Softs;
            else if (tyre == 2) t = Enums.Tyres.Mediums;
            else t = Enums.Tyres.Hards;
            Starting = t;
            dr.TyreCompound = t;
            dr.TyreWear = 100;
            dr.MovesMade = 0;
            int drClass = random.Next(1, 4);
            if (drClass == 1) dr.DriverClass = DriverClass.OscarPiastri;
            else if (drClass == 2) dr.DriverClass = DriverClass.SebastianVettel;
            else dr.DriverClass = DriverClass.GeorgeRussel;
        }
    
        public double NeededToFinish()
        {
            int NeededThis = (DriverRace.Race.Track.StepsPerLap * DriverRace.Race.Track.RaceLaps)
                                - (DriverRace.StepsDriven - DriverRace.StepsHistory.Last());

            return (double)NeededThis / DriverRace.StepsHistory.Last();
        }
    }
}

using DailyGrandPrix.Enums;

namespace DailyGrandPrix.Entities
{
    internal class Race
    {
        public int Id { get; private set; }
        public DateOnly Start { get; private set; }
        public DateOnly? End { get; private set; }
        public List<DriverRace> Drivers { get; private set; } = new();
        public Championship Championship { get; private set; }
        public RaceState RaceState { get; private set; }
        public Track Track { get; private set; } = new();

        public Race(int id, Championship championship, Track track)
        {
            Id = id;
            Start = DateOnly.FromDateTime(DateTime.Now);
            End = null;
            Championship = championship;
            RaceState = RaceState.AddingDrivers;
            Track = track;
        }

        public Race(int id, DateOnly start, DateOnly? end,
            Championship championship, RaceState raceState, Track track)
        {
            Id = id;
            Start = start;
            End = end;
            Championship = championship;
            RaceState = raceState;
            Track = track;
        }
    
        public void AddDriver(Driver driver)
        {
            if (RaceState != RaceState.AddingDrivers)
            {
                throw new ArgumentException("This race already started!");
            }

            DriverRace dr = new();
            Drivers.Add(dr);
            driver.Races.Add(dr);
        }
    }
}

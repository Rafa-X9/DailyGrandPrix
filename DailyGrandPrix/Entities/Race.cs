using DailyGrandPrix.Enums;
using DailyGrandPrix.Exceptions;
using DailyGrandPrix.Services;

namespace DailyGrandPrix.Entities
{
    internal class Race
    {
        public int Id { get; private set; }
        public DateOnly Start { get; private set; }
        public DateOnly? End { get; private set; }
        public List<DriverRace> Drivers { get; private set; } = new();
        public Championship Championship { get; private set; }
        public RaceState RaceState { get; set; }
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

            DriverRace? d = Drivers.Where(dri => dri.Driver == driver).FirstOrDefault();
            if (d != null) throw new DriverAlreadyInException("This driver is already in this race!");

            DriverRace dr = new(driver);
            Drivers.Add(dr);
            driver.Races.Add(dr);
        }

        public override string ToString()
        {
            return $"Race in {Track.Name} for the {Championship.Name} championship." +
                $"\nHas {Drivers.Count} drivers." +
                $"\nState: {RaceState}.\n";
        }
    }
}

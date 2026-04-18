using DailyGrandPrix.Enums;
using DailyGrandPrix.Exceptions;
using DailyGrandPrix.Services;
using System.Text.Json.Serialization;

namespace DailyGrandPrix.Entities
{
    internal class Race
    {
        public int Id { get; set; }
        public DateOnly Start { get; set; }
        public DateOnly? End { get; set; }
        public List<DriverRace> Drivers { get; set; } = new();
        [JsonIgnore] public Championship? Championship { get; set; }
        public int? ChampionshipId
        {
            get
            {
                return (Championship is null) ? null : Championship.Id;
            }
        }
        public RaceState RaceState { get; set; }
        public Track Track { get; set; } = new();
        public int MovesInto { get; set; } = 0;
        public int RainFactor { get; set; } = 0;

        public Race() { }

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
            Championship championship, RaceState raceState,
            Track track, int movesInto)
        {
            Id = id;
            Start = start;
            End = end;
            Championship = championship;
            RaceState = raceState;
            Track = track;
            MovesInto = movesInto;
        }

        public void AddDriver(Driver driver, Race race)
        {
            if (RaceState != RaceState.AddingDrivers)
            {
                throw new ArgumentException("This race already started!");
            }

            DriverRace? d = Drivers.Where(dri => dri.Driver == driver).FirstOrDefault();
            if (d != null) throw new DriverAlreadyInException("This driver is already in this race!");

            DriverRace dr = new(driver, race);
            Drivers.Add(dr);
            driver.Races.Add(dr);
        }

        public override string ToString()
        {
            return $"Race in {Track.Name} for the {Championship?.Name} championship." +
                $"\nHas {Drivers.Count} drivers." +
                $"\nState: {RaceState}." +
                $"\nTrack: {Track.Name}" +
                $"\nId: {Id}";
        }
    }
}

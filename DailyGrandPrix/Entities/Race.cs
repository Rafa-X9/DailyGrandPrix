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
        public List<int> RainHistory { get; set; } = new();
        public int RainFactor
        {
            get
            {
                if (RainHistory.Count == 0 || RainHistory.Count < MovesInto)
                {
                    return 0;
                }
                if (MovesInto == 0)
                {
                    return RainHistory[0];
                }
                return RainHistory[MovesInto - 1];
            }
        }

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

        public List<(string Name, int PositionGain, string LastAction)> GetWhatHappened()
        {
            if (MovesInto == 0)
            {
                throw new InvalidOperationException("All drivers must have made a move.");
            }

            Drivers.Sort();
            List<(string Name, int PositionGain, string LastAction)> list = [];

            if (MovesInto == 1)
            {
                foreach (DriverRace d in Drivers)
                {
                    list.Add((d.Driver.Name, 0, d.LastAction.ToString().ToLower()));
                }
                return list;
            }

            var oneMoveEarlier = Drivers
                .Select(d => (d.Driver.Id, d.Driver.Name, d.StepsDriven - d.StepsHistory.Last()))
                .ToList();

            for (int iEarlier = 0; iEarlier < oneMoveEarlier.Count; iEarlier++)
            {
                for (int iNow = 0; iNow < Drivers.Count; iNow++)
                {
                    if (oneMoveEarlier[iEarlier].Id != Drivers[iNow].Driver.Id)
                    {
                        continue;
                    }
                    list.Add((Drivers[iNow].Driver.Name, iEarlier - iNow, Drivers[iNow].LastAction.ToString()));
                }
            }
            return list;
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

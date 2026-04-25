using DailyGrandPrix.Enums;
using DailyGrandPrix.Exceptions;
using DailyGrandPrix.Services;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DailyGrandPrix.Entities
{
    internal class Race
    {
        public int Id { get; }
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
        public Track Track { get; set; }
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

        public Race(Dictionary<string, object> json)
        {
            //Id
            if (!json.TryGetValue("Id", out object? id))
            {
                throw new ArgumentException("JSON didn't have an Id key for Race");
            }
            if (!int.TryParse(id.ToString(), out int idInt))
            {
                throw new ArgumentException("JSON's Id value for Race wasn't a proper integer");
            }
            Id = idInt;


            //Start
            if (!json.TryGetValue("Start", out object? start) || start is null)
            {
                throw new ArgumentException("JSON didn't have a Start key for Championship or it was null");
            }
            string? startStr = Convert.ToString(start);
            if (string.IsNullOrEmpty(startStr))
            {
                throw new ArgumentException("JSON's Start key for Championship was null, empty, or invalid");
            }
            if (!DateOnly.TryParse(startStr, out DateOnly startDate))
            {
                throw new ArgumentException("JSON's Start key for Championship wasn't a valid DateOnly");
            }
            Start = startDate;


            //End
            if (!json.TryGetValue("End", out object? end) || end is null)
            {
                End = null;
            }
            else
            {
                string? endStr = Convert.ToString(end);
                if (string.IsNullOrEmpty(endStr))
                {
                    End = null;
                }
                else if (!DateOnly.TryParse(endStr, out DateOnly endDate))
                {
                    End = null;
                }
                else
                {
                    End = endDate;
                }
            }


            //Drivers
            if (!json.TryGetValue("Drivers", out object? drivers))
            {
                throw new ArgumentException("Race didn't have a Drivers key");
            }
            if (drivers is null)
            {
                throw new ArgumentException("Race's Drivers key's value was null");
            }
            string? driversStr = drivers.ToString();
            if (string.IsNullOrEmpty(driversStr))
            {
                throw new ArgumentException("Race's Drivers key was null or empty");
            }
            var driversJson = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(driversStr);
            if (driversJson is null)
            {
                throw new ArgumentException("Race's Drivers key wasn't a valid List<Dictionary<string, object>>");
            }
            driversJson.ForEach(d => Drivers.Add(new(d)));


            //RaceState
            if (!json.TryGetValue("RaceState", out object? state) || state is null || !int.TryParse(state.ToString(), out int stateInt) || !Enum.IsDefined(typeof(Teams), stateInt))
            {
                throw new ArgumentException("JSON didn't have a RaceState key for Race or it wasn't a member of RaceState enumeration");
            }
            RaceState = (RaceState)stateInt;


            //Track
            if (!json.TryGetValue("Track", out object? track) || track is null)
            {
                throw new ArgumentException("JSON didn't have a Track key for Race or it was null");
            }
            string? trackStr = track.ToString();
            if (string.IsNullOrEmpty(trackStr))
            {
                throw new ArgumentException("JSON's Track key for Race wasn't a proper JSON");
            }
            var trackJson = JsonSerializer.Deserialize<Dictionary<string, object>>(trackStr);
            if (trackJson is null)
            {
                throw new ArgumentException("JSON's Track key for Race wasn't a proper JSON");
            }
            Track = new(trackJson);


            //MovesInto
            if (!json.TryGetValue("MovesInto", out object? moves))
            {
                throw new ArgumentException("JSON didn't have a MovesInto key for Race");
            }
            if (!int.TryParse(moves.ToString(), out int movesInt))
            {
                throw new ArgumentException("JSON's MovesInto value for Race wasn't a proper integer");
            }
            MovesInto = movesInt;


            //RainHistory
            if (json.TryGetValue("RainHistory", out object? rain) && rain is not null && rain is List<int> rainList)
            {
                RainHistory = rainList;
            }
        }

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

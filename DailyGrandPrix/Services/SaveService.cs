using DailyGrandPrix.Entities;
using DailyGrandPrix.Enums;

namespace DailyGrandPrix.Services
{
    internal sealed class SaveService
    {
        public static string DatabasePath = @"C:\Users\Lenovo\Desktop\Rafael\projetosCsharp\DailyGrandPrix\Database";
        public static string ChampionshipPath = DatabasePath + @"\Championships";
        //public static string RacePath = ChampionshipPath + @"\Races";
        public static string DriversPath = DatabasePath + @"\Drivers";
        public static string TracksPath = DatabasePath + @"\Tracks";
        public List<Championship> Championships { get; set; } = new();
        //public List<Race> Races { get; set; } = new();
        public List<Driver> Drivers { get; set; } = new();
        public List<Track> Tracks { get; set; } = new();

        public SaveService()
        {
            DirectoryInfo database = new DirectoryInfo(DatabasePath);
            DirectoryInfo championship = new DirectoryInfo(ChampionshipPath);
            //DirectoryInfo race = new DirectoryInfo(RacePath);
            DirectoryInfo drivers = new DirectoryInfo(DriversPath);
            DirectoryInfo tracks = new DirectoryInfo(TracksPath);

            if (!database.Exists) database.Create();
            if (!championship.Exists) championship.Create();
            //if (!race.Exists) race.Create();
            if (!drivers.Exists) drivers.Create();
            if (!tracks.Exists) tracks.Create();
        }

        public void ImportAll()
        {
            ImportDrivers();
            ImportTracks();
            ImportChampionships();
            ImportRaces();
        }

        public void SaveAll()
        {
            SaveDrivers();
            SaveTracks();
            SaveChampionships();
            SaveRaces();
        }

        public void ImportDrivers()
        {
            foreach (string file in Directory.GetFiles(DriversPath))
            {
                StreamReader sr = new StreamReader(file);
                string[] line = sr.ReadLine().Split(',');
                int id = int.Parse(line[0]);
                string name = line[1];
                string username = line[2];
                int number = int.Parse(line[3]);
                Teams team = Enum.Parse<Teams>(line[4]);
                Drivers.Add(new Driver(id, name, username, number, team));
                sr.Close();
            }
        }

        public void SaveDrivers()
        {
            foreach (Driver d in Drivers)
            {
                StreamWriter sw = new(DriversPath + @"\" + d.Name + ".txt", false);
                sw.WriteLine($"{d.Id},{d.Name},{d.Username},{d.Number},{d.Team}");
                sw.Close();
            }
        }

        public void DeleteUntrackedDrivers()
        {
            foreach (string file in Directory.GetFiles(DriversPath))
            {
                bool isTracked = false;
                foreach (Driver d in Drivers)
                {
                    if (file == (DriversPath + @"\" + d.Name + ".txt")) isTracked = true;
                }
                if (isTracked) continue;
                File.Delete(file);
            }
        }

        public void ImportTracks()
        {
            foreach (string file in Directory.GetFiles(TracksPath))
            {
                StreamReader sr = new(file);
                string[] line = sr.ReadLine().Split(',');
                int id = int.Parse(line[0]);
                string name = line[1];
                int stepsPerLap = int.Parse(line[2]);
                Tracks.Add(new Track(id, name, stepsPerLap));
                sr.Close();
            }
        }

        public void SaveTracks()
        {
            foreach (Track t in Tracks)
            {
                StreamWriter sw = new(TracksPath + $@"\{t.Name}.txt", false);
                sw.WriteLine($"{t.Id},{t.Name},{t.StepsPerLap}");
                sw.Close();
            }
        }

        public void DeleteUntrackedTracks()
        {
            foreach (string file in Directory.GetFiles(TracksPath))
            {
                bool isTracked = false;
                foreach (Track t in Tracks)
                {
                    if (file == (TracksPath + @"\" + t.Name + ".txt")) isTracked = true;
                }
                if (isTracked) continue;
                File.Delete(file);
            }
        }

        public void DeleteTrack(Track track)
        {
            foreach (Championship champ in Championships)
            {
                Race? toDelete = null;
                foreach (Race race in champ.Races)
                {
                    if (race.Track == track)
                    {
                        toDelete = race;
                    }
                }
                if (toDelete != null) champ.Races.Remove(toDelete);
            }
            Tracks.Remove(track);
        }

        public void ImportChampionships()
        {
            foreach (string folder in Directory.GetDirectories(ChampionshipPath))
            {
                StreamReader sr = new(folder + @"\about.txt");
                string[] line = sr.ReadLine().Split(',');
                int id = int.Parse(line[0]);
                int year = int.Parse(line[1]);
                string name = line[2];
                Championships.Add(new(id, year, name));
                sr.Close();
            }
        }

        public void SaveChampionships()
        {
            foreach (Championship c in Championships)
            {
                DirectoryInfo di = new(ChampionshipPath + $@"\{c.Name}");
                if (!di.Exists) di.Create();
                StreamWriter sw = new(ChampionshipPath + $@"\{c.Name}\about.txt", false);
                sw.WriteLine($"{c.Id},{c.Year},{c.Name}");
                sw.Close();
            }
        }

        public void DeleteUntrackedChampionships()
        {
            foreach (string folder in Directory.GetDirectories(ChampionshipPath))
            {
                bool isTracked = false;
                foreach (Championship champ in Championships)
                {
                    if (folder == (ChampionshipPath + $@"\{champ.Name}"))
                    {
                        isTracked = true;
                    }
                }
                if (isTracked) continue;
                Directory.Delete(folder, true);
            }
        }

        public void ImportRaces()
        {
            foreach (Championship champ in Championships)
            {
                foreach (string race in Directory.GetFiles(ChampionshipPath + $@"\{champ.Name}"))
                {
                    if (race == (ChampionshipPath + $@"\{champ.Name}\about.txt")) continue;
                    string path = race;
                    StreamReader sr = new(path);
                    string[] line = sr.ReadLine().Split(',');
                    int id = int.Parse(line[0]);
                    DateOnly start = DateOnly.FromDateTime(DateTime.ParseExact(line[1], "dd/MM/yyyy", null));
                    DateOnly? end;
                    if (line[2] == "null") end = null;
                    else end = DateOnly.FromDateTime(DateTime.ParseExact(line[2], "dd/MM/yyyy", null));
                    int champId = int.Parse(line[3]);
                    RaceState state = Enum.Parse<RaceState>(line[4]);
                    int trackId = int.Parse(line[5]);
                    Track track = Tracks.Where(t => t.Id == trackId).First();
                    int movesInto = int.Parse(line[6]);
                    Race r = new(id, start, end, champ, state, track, movesInto);
                    champ.Races.Add(r);

                    while (!sr.EndOfStream)
                    {
                        line = sr.ReadLine().Split(',');
                        int driverId = int.Parse(line[0]);
                        DriverRace dr = new(Drivers.Where(d => d.Id == driverId).First());
                        string finalPos = line[1];
                        if (finalPos != "null") dr.FinalPosition = int.Parse(finalPos);
                        else dr.FinalPosition = null;
                        dr.HasRetired = bool.Parse(line[2]);
                        dr.TyreCompound = Enum.Parse<Tyres>(line[3]);
                        dr.TyreWear = int.Parse(line[4]);
                        dr.TyreChanges = int.Parse(line[5]);
                        dr.FuelAmount = int.Parse(line[6]);
                        dr.MovesMade = int.Parse(line[7]);
                        dr.LastAction = Enum.Parse<Actions>(line[8]);
                        for (int i = 9; i < line.Length; i++)
                        {
                            dr.StepsHistory.Add(i);
                        }
                        r.Drivers.Add(dr);
                    }

                    sr.Close();
                }
            }
        }

        public void SaveRaces()
        {
            foreach (Championship champ in Championships)
            {
                foreach (Race race in champ.Races)
                {
                    string path = ChampionshipPath + $@"\{champ.Name}\{race.Track.Name}-Race.txt";
                    StreamWriter sw = new(path, false);
                    string end;
                    if (race.End is not null) end = race.End.ToString();
                    else end = "null";
                    sw.WriteLine($"{race.Id},{race.Start},{end}," +
                        $"{race.Championship.Id},{race.RaceState}," +
                        $"{race.Track.Id},{race.MovesInto}");

                    foreach (DriverRace d in race.Drivers)
                    {
                        string finalPos;
                        if (d.FinalPosition is not null) finalPos = d.FinalPosition.ToString();
                        else finalPos = "null";

                        sw.Write($"{d.Driver.Id},{finalPos},{d.HasRetired}," +
                            $"{d.TyreCompound},{d.TyreWear},{d.TyreChanges}," +
                            $"{d.FuelAmount},{d.MovesMade},{d.LastAction}");
                        foreach (int step in d.StepsHistory) sw.Write($",{step}");
                        sw.WriteLine();
                    }

                    sw.Close();
                }
            }
        }
    }
}

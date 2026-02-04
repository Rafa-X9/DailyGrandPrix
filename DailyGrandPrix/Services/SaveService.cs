using DailyGrandPrix.Entities;
using DailyGrandPrix.Enums;

namespace DailyGrandPrix.Services
{
    internal sealed class SaveService
    {
        public static string DatabasePath = @"C:\Users\Lenovo\Desktop\Rafael\projetosCsharp\DailyGrandPrix\Database";
        public static string ChampionshipPath = DatabasePath + @"\Championships";
        public static string RacePath = ChampionshipPath + @"\Races";
        public static string DriversPath = DatabasePath + @"\Drivers";
        public static string TracksPath = DatabasePath + @"\Tracks";
        public List<Championship> Championships { get; set; } = new();
        public List<Race> Races { get; set; } = new();
        public List<Driver> Drivers { get; set; } = new();
        public List<Track> Tracks { get; set; } = new();

        public SaveService()
        {
            DirectoryInfo database = new DirectoryInfo(DatabasePath);
            DirectoryInfo championship = new DirectoryInfo(ChampionshipPath);
            DirectoryInfo race = new DirectoryInfo(RacePath);
            DirectoryInfo drivers = new DirectoryInfo(DriversPath);
            DirectoryInfo tracks = new DirectoryInfo(TracksPath);

            if (!database.Exists) database.Create();
            if (!championship.Exists) championship.Create();
            if (!race.Exists) race.Create();
            if (!drivers.Exists) drivers.Create();
            if (!tracks.Exists) tracks.Create();
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
    }
}

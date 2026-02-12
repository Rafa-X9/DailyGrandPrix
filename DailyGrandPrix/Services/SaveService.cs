using DailyGrandPrix.Entities;
using DailyGrandPrix.Enums;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;

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
                    if (race.EndsWith(".xlsx")) continue;
                    if (race.EndsWith("log.txt")) continue;
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
                        DriverRace dr = new(Drivers.Where(d => d.Id == driverId).First(), r);
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
                        dr.DriverClass = Enum.Parse<DriverClass>(line[9]);
                        for (int i = 10; i < line.Length; i++)
                        {
                            dr.StepsHistory.Add(int.Parse(line[i]));
                        }
                        r.Drivers.Add(dr);
                    }

                    sr.Close();
                }
            }
        }

        public void SaveRaces(bool makeExcelLog)
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
                            $"{d.FuelAmount},{d.MovesMade},{d.LastAction}," +
                            $"{d.DriverClass}");
                        foreach (int step in d.StepsHistory) sw.Write($",{step}");
                        sw.WriteLine();
                    }

                    sw.Close();
                    GenerateTxtRaceLog(race);
                    if (makeExcelLog) GenerateExcelRaceLog(race);
                }
            }
        }

        private void GenerateExcelRaceLog(Race race)
        {
            try
            {
                race.Drivers.Sort();
                FileInfo path = new($@"{ChampionshipPath}\{race.Championship.Name}\{race.Track.Name}-Race-log.xlsx");
                if (path.Exists) path.Delete();

                using var package = new ExcelPackage(path);
                var worksheet = package.Workbook.Worksheets.Add("Race log");

                // Headers
                worksheet.Cells[1, 1].Value = "Position";
                worksheet.Cells[1, 2].Value = "Driver";
                worksheet.Cells[1, 3].Value = "Action";
                worksheet.Cells[1, 4].Value = "Laps";
                worksheet.Cells[1, 5].Value = "Steps into this lap";
                worksheet.Cells[1, 6].Value = "Fuel amount";
                worksheet.Cells[1, 7].Value = "Tyres";
                worksheet.Cells[1, 8].Value = "Tyre wear";
                worksheet.Cells[1, 9].Value = "Steps driven";

                // Header styling: blue background, white bold text
                var headerRange = worksheet.Cells[1, 1, 1, 9];
                headerRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
                headerRange.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(0, 112, 192)); // Excel-like blue
                headerRange.Style.Font.Color.SetColor(Color.White);
                headerRange.Style.Font.Bold = true;
                headerRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                // Populate rows: one DriverRace per row
                int row = 2;
                for (int i = 0; i < race.Drivers.Count; i++)
                {
                    DriverRace dr = race.Drivers[i];
                    if (dr.StepsDriven < dr.Race.Track.RaceLaps * dr.Race.Track.StepsPerLap)
                    {
                        if (!dr.HasRetired) worksheet.Cells[row, 1].Value = "P" + (i + 1);
                        else worksheet.Cells[row, 1].Value = "DNF";
                    }
                    else worksheet.Cells[row, 1].Value = "Finished P" + (i + 1);
                    worksheet.Cells[row, 2].Value = dr.Driver.Name;
                    worksheet.Cells[row, 3].Value = dr.LastAction;
                    int lapsIn = (int)Math.Floor((double)dr.StepsDriven / dr.Race.Track.StepsPerLap);
                    worksheet.Cells[row, 4].Value = lapsIn;
                    worksheet.Cells[row, 5].Value = dr.StepsDriven - (lapsIn * dr.Race.Track.StepsPerLap) + 1;
                    worksheet.Cells[row, 6].Value = dr.FuelAmount + "/100";
                    worksheet.Cells[row, 7].Value = dr.TyreCompound;
                    worksheet.Cells[row, 8].Value = dr.TyreWear + "/100";

                    string steps = "";
                    foreach (int num in dr.StepsHistory)
                    {
                        steps += num.ToString() + " - ";
                    }
                    if (dr.StepsHistory.Count > 0)
                    {
                        worksheet.Cells[row, 9].Value = steps.Substring(0, steps.Length - 3);
                    }
                    // make steps history wrap if long
                    //worksheet.Cells[row, 7].Style.WrapText = true;

                    worksheet.Cells[row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[row, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    if (dr.DriverClass == DriverClass.OscarPiastri)
                    {
                        worksheet.Cells[row, 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[row, 2].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(242, 141, 17));
                    }
                    else if (dr.DriverClass == DriverClass.SebastianVettel)
                    {
                        worksheet.Cells[row, 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[row, 2].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(49, 49, 245));
                    }
                    else if (dr.DriverClass == DriverClass.GeorgeRussel)
                    {
                        worksheet.Cells[row, 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[row, 2].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(100, 100, 100));
                    }
                    worksheet.Cells[row, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[row, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[row, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[row, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[row, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[row, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    row++;
                }

                int lastRow = Math.Max(1, row - 1);
                var filledRange = worksheet.Cells[1, 1, lastRow, 9];
                worksheet.Cells[row, 1].Value = "This log was generated automatically. " +
                    "Reply to this message if you have questions or concerns.";
                worksheet.Cells[row + 1, 1].Value = "You can find my source code in " +
                    "https://github.com/Rafa-X9/DailyGrandPrix";
                worksheet.Cells[row, 1, row, 8].Merge = true;
                worksheet.Cells[row + 1, 1, row + 1, 8].Merge = true;

                worksheet.Cells[row + 2, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[row + 2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[row + 2, 1].Value = "Oscar Piastri";
                worksheet.Cells[row + 2, 1].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(242, 141, 17));
                
                worksheet.Cells[row + 2, 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[row + 2, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[row + 2, 2].Value = "Sebastian Vettel";
                worksheet.Cells[row + 2, 2].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(49, 49, 245));
                
                worksheet.Cells[row + 2, 3].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[row + 2, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[row + 2, 3].Value = "George Russel";
                worksheet.Cells[row + 2, 3].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(100, 100, 100));

                // Apply strong (thick) border to all filled cells
                filledRange.Style.Border.Top.Style = ExcelBorderStyle.Thick;
                filledRange.Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
                filledRange.Style.Border.Left.Style = ExcelBorderStyle.Thick;
                filledRange.Style.Border.Right.Style = ExcelBorderStyle.Thick;

                // Autofit columns
                worksheet.Cells[1, 1, lastRow + 3, 9].AutoFitColumns();

                package.Save();
            }
            catch (IOException)
            {
                Console.WriteLine("This file is open! Close it.");
                Console.WriteLine("Press enter to continue.");
                Console.ReadLine();
            }
        }

        private void GenerateTxtRaceLog(Race race)
        {
            race.Drivers.Sort();
            StreamWriter sw = new($@"{ChampionshipPath}\{race.Championship.Name}\{race.Track.Name}-Race-log.txt");

            for (int i = 0; i < race.Drivers.Count; i++)
            {
                DriverRace d = race.Drivers[i];
                if (!d.HasRetired && d.StepsDriven < d.Race.Track.StepsPerLap * d.Race.Track.RaceLaps)
                {
                    sw.WriteLine($"**P{i + 1} - {d.Driver.Name} ({d.Driver.Username})**");
                    sw.WriteLine();
                    if (d.LastAction != Actions.None)
                    {
                        sw.Write($"{d.Driver.Name} ");
                        switch (d.LastAction)
                        {
                            case Actions.Conserve:
                                sw.WriteLine("conserved");
                                break;
                            case Actions.Push:
                                sw.WriteLine("pushed");
                                break;
                            case Actions.Pit:
                                sw.WriteLine("made a pitstop for new " + d.TyreCompound);
                                break;
                        }
                        sw.WriteLine();
                    }
                    int laps = (int)Math.Floor((double)d.StepsDriven / d.Race.Track.StepsPerLap);
                    sw.WriteLine("Laps driven: " + laps);
                    sw.WriteLine();
                    sw.WriteLine("Steps into this lap: " + (d.StepsDriven - (laps * d.Race.Track.StepsPerLap) + 1));
                    sw.WriteLine();
                    sw.WriteLine($"Fuel: {d.FuelAmount}/100");
                    sw.WriteLine();
                    sw.WriteLine($"Tyres: {d.TyreCompound}, {d.TyreWear}/100");
                    sw.WriteLine();
                    if (d.StepsHistory.Count > 0)
                    {
                        sw.Write("Steps history: ");
                        d.StepsHistory.ForEach(s => sw.Write(s + " "));
                        sw.WriteLine();
                    }
                    sw.WriteLine();
                }
                else if (d.HasRetired)
                {
                    sw.WriteLine($"**DNF - {d.Driver.Name} ({d.Driver.Username})**");
                    sw.WriteLine();
                    sw.WriteLine(d.Driver.Name + " has retired from the race");
                    sw.WriteLine();
                }
                else if (d.StepsDriven >= d.Race.Track.StepsPerLap * d.Race.Track.RaceLaps)
                {
                    sw.WriteLine($"**P{i + 1} - {d.Driver.Name} ({d.Driver.Username})**");
                    sw.WriteLine();
                    switch (i)
                    {
                        case 0:
                            sw.WriteLine(d.Driver.Name + " finishes first and wins the DailyGrandPrix!");
                            break;
                        case 1:
                            sw.WriteLine(d.Driver.Name + " finishes second in the DailyGrandPrix!");
                            break;
                        case 2:
                            sw.WriteLine(d.Driver.Name + " finishes third and completes the podium of the DailyGrandPrix!");
                            break;
                        default:
                            sw.WriteLine($"{d.Driver.Name} finishes int P{i + 1} in the DailyGrandPrix!");
                            break;
                    }
                    sw.WriteLine();
                }

                sw.WriteLine("---");
                sw.WriteLine();
            }

            sw.WriteLine("^(This message and all calculations of this series" +
                " are made automatically. If you have questions or concerns," +
                " reply to this message. This will summon my creator. You can" +
                " find my source code on [GitHub](https://github.com/Rafa-X9/DailyGrandPrix).)");

            sw.Close();
        }
    }
}
using DailyGrandPrix.Entities;
using DailyGrandPrix.Enums;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using System.Text.Json;

namespace DailyGrandPrix.Services
{
    internal sealed class SaveService
    {
        public string DatabasePath { get; private set; }
        public string ChampionshipPath { get; private set; }
        public string DriversPath { get; private set; }
        public string TracksPath { get; private set; }
        public List<Championship> Championships { get; set; } = new();
        public List<Driver> Drivers { get; set; } = new();
        public List<Track> Tracks { get; set; } = new();

        public SaveService()
        {
            while (!File.Exists("path.txt"))
            {
                try
                {
                    Console.Clear();
                    Console.WriteLine("No path.txt folder found for the database.");
                    string path = InputService.GetStringInput("Type a path to a folder to store the database in:");

                    if (!Directory.Exists(path))
                    {
                        throw new DirectoryNotFoundException();
                    }

                    StreamWriter sw = new("path.txt", false);
                    sw.WriteLine(path);
                    sw.Close();
                }
                catch (FormatException ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine("Press enter to continue.");
                    Console.ReadLine();
                }
                catch (DirectoryNotFoundException)
                {
                    Console.WriteLine("This directory wasn't found!");
                    Console.WriteLine("Press enter to continue.");
                    Console.ReadLine();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("ERROR");
                    Console.WriteLine(ex.Message);
                    Console.WriteLine("Press enter to continue.");
                    Console.ReadLine();
                }
            }

            using (StreamReader sr = new("path.txt"))
            {
                string? line = sr.ReadLine();
                if (line is null)
                {
                    throw new FormatException("path.txt doesn't contain a path.");
                }
                DatabasePath = line;
            }
            ChampionshipPath = DatabasePath + @"\Championships";
            DriversPath = DatabasePath + @"\Drivers";
            TracksPath = DatabasePath + @"\Tracks";

            DirectoryInfo database = new DirectoryInfo(DatabasePath);
            DirectoryInfo championship = new DirectoryInfo(ChampionshipPath);
            DirectoryInfo drivers = new DirectoryInfo(DriversPath);
            DirectoryInfo tracks = new DirectoryInfo(TracksPath);

            if (!database.Exists) database.Create();
            if (!championship.Exists) championship.Create();
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
                using StreamReader sr = new StreamReader(file);
                string? line = sr.ReadLine();
                if (line is null)
                {
                    continue;
                }
                var json = JsonSerializer.Deserialize<Dictionary<string, object>>(line);
                if (json is null)
                {
                    throw new InvalidOperationException("Json for Driver is null");
                }
                Drivers.Add(new(json));
            }
        }

        public void SaveDrivers()
        {
            foreach (Driver d in Drivers)
            {
                using StreamWriter sw = new(DriversPath + @"\" + d.Name + ".txt", false);
                sw.WriteLine(JsonSerializer.Serialize(d));
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
                using StreamReader sr = new(file);
                string? line = sr.ReadLine();
                if (line is null)
                {
                    continue;
                }
                var json = JsonSerializer.Deserialize<Dictionary<string, object>>(line);
                if (json is null)
                {
                    throw new InvalidOperationException("JSON for Track is null");
                }
                Tracks.Add(new(json));
            }
        }

        public void SaveTracks()
        {
            foreach (Track t in Tracks)
            {
                using StreamWriter sw = new(TracksPath + $@"\{t.Name}.txt", false);
                sw.WriteLine(JsonSerializer.Serialize(t));
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
                using StreamReader sr = new(folder + @"\about.txt");
                string? line = sr.ReadLine();
                if (line is null)
                {
                    continue;
                }
                Championship? champ = JsonSerializer.Deserialize<Championship>(line);
                if (champ is not null)
                {
                    Championships.Add(champ);
                }
            }
        }

        public void SaveChampionships()
        {
            foreach (Championship c in Championships)
            {
                DirectoryInfo di = new(ChampionshipPath + $@"\{c.Name}");
                if (!di.Exists) di.Create();
                using StreamWriter sw = new(ChampionshipPath + $@"\{c.Name}\about.txt", false);
                sw.WriteLine(JsonSerializer.Serialize(c));
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
                foreach (string path in Directory.GetFiles(ChampionshipPath + $@"\{champ.Name}"))
                {
                    if (path == (ChampionshipPath + $@"\{champ.Name}\about.txt")) continue;
                    if (path.EndsWith("log.txt")) continue;
                    if (!path.EndsWith(".txt")) continue;
                    using StreamReader sr = new(path);
                    string? line = sr.ReadLine();
                    if (line is null)
                    {
                        continue;
                    }
                    Race? race = JsonSerializer.Deserialize<Race>(line);
                    if (race is not null)
                    {
                        race.Championship = champ;
                        champ.Races.Add(race);
                        foreach (DriverRace d in race.Drivers)
                        {
                            d.Driver = Drivers.Where(dr => dr.Id == d.DriverId).First();
                            d.Race = race;
                        }
                    }
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
                    using StreamWriter sw = new(path, false);
                    sw.WriteLine(JsonSerializer.Serialize(race));

                    GenerateTxtRaceLog(race);
                    if (makeExcelLog) GenerateExcelRaceLog(race);
                    GenerateWhatHappened(race);
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
                    if (dr.LastAction == Actions.Pit)
                    {
                        worksheet.Cells[row, 3].Value = "Pitstop";
                    }
                    else
                    {
                        worksheet.Cells[row, 3].Value = dr.LastAction;
                    }
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
                    else if (dr.DriverClass == DriverClass.LanceStroll)
                    {
                        worksheet.Cells[row, 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[row, 2].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(0, 113, 39));
                    }
                    worksheet.Cells[row, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[row, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[row, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[row, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[row, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[row, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    row++;
                }

                worksheet.Cells[row, 1].Value = "RAIN FACTOR:";
                worksheet.Cells[row, 1].Style.Font.Bold = true;
                string rainChange = race.RainFactor.ToString();
                if (race.RainHistory.Count > 1 && race.MovesInto > 0)
                {
                    int previous = race.RainHistory[race.MovesInto - 1];
                    if (previous > race.RainFactor)
                    {
                        rainChange = $"-{previous - race.RainFactor}" + rainChange;
                        worksheet.Cells[row, 2].Style.Font.Color.SetColor(Color.Red);
                        worksheet.Cells[row, 2].Value = rainChange;
                    }
                    else if (previous < race.RainFactor)
                    {
                        rainChange = $"+{previous - race.RainFactor}" + rainChange;
                        worksheet.Cells[row, 2].Style.Font.Color.SetColor(Color.Green);
                        worksheet.Cells[row, 2].Value = rainChange;
                    }
                    else
                    {
                        rainChange = $"=" + rainChange;
                        worksheet.Cells[row, 2].Value = rainChange;
                    }
                }
                else
                {
                    worksheet.Cells[row, 2].Value = rainChange;
                }

                int lastRow = Math.Max(1, row - 1);
                var filledRange = worksheet.Cells[1, 1, lastRow, 9];
                worksheet.Cells[row + 1, 1].Value = "This log was generated automatically. " +
                    "Reply to this message if you have questions or concerns.";
                worksheet.Cells[row + 2, 1].Value = "You can find my source code in " +
                    "https://github.com/Rafa-X9/DailyGrandPrix";
                worksheet.Cells[row + 1, 1, row + 1, 8].Merge = true;
                worksheet.Cells[row + 2, 1, row + 2, 8].Merge = true;

                worksheet.Cells[row + 3, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[row + 3, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[row + 3, 1].Value = "Oscar Piastri";
                worksheet.Cells[row + 3, 1].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(242, 141, 17));

                worksheet.Cells[row + 3, 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[row + 3, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[row + 3, 2].Value = "Sebastian Vettel";
                worksheet.Cells[row + 3, 2].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(49, 49, 245));

                worksheet.Cells[row + 3, 3].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[row + 3, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[row + 3, 3].Value = "George Russel";
                worksheet.Cells[row + 3, 3].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(100, 100, 100));

                worksheet.Cells[row + 3, 4].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[row + 3, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[row + 3, 4].Value = "Lance Stroll";
                worksheet.Cells[row + 3, 4].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(0, 113, 39));

                // Apply strong (thick) border to all filled cells
                filledRange.Style.Border.Top.Style = ExcelBorderStyle.Thick;
                filledRange.Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
                filledRange.Style.Border.Left.Style = ExcelBorderStyle.Thick;
                filledRange.Style.Border.Right.Style = ExcelBorderStyle.Thick;

                // Autofit columns
                worksheet.Cells[1, 1, lastRow + 4, 9].AutoFitColumns();

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

            sw.WriteLine("Log for " + race.Track.Name + " DailyGrandPrix");
            sw.WriteLine();
            sw.WriteLine("**RAIN FACTOR: " + race.RainFactor + "**");
            sw.WriteLine();

            for (int i = 0; i < race.Drivers.Count; i++)
            {
                DriverRace d = race.Drivers[i];
                if (!d.HasRetired && d.StepsDriven < d.Race.Track.StepsPerLap * d.Race.Track.RaceLaps)
                {
                    sw.WriteLine($"**P{i + 1} - {d.Driver.Name} ({d.Driver.Username}) - {d.Driver.Team}**");
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
                    sw.WriteLine($"**DNF - {d.Driver.Name} ({d.Driver.Username}) - {d.Driver.Team}**");
                    sw.WriteLine();
                    sw.WriteLine(d.Driver.Name + " has retired from the race");
                    sw.WriteLine();
                }
                else if (d.StepsDriven >= d.Race.Track.StepsPerLap * d.Race.Track.RaceLaps)
                {
                    sw.WriteLine($"**P{i + 1} - {d.Driver.Name} ({d.Driver.Username}) - {d.Driver.Team}**");
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

        private void GenerateWhatHappened(Race race)
        {
            if (race.Drivers.Count < 2)
            {
                return;
            }
            if (race.Drivers.Where(d => d.MovesMade == 0 || d.MovesMade == 1).Any())
            {
                return;
            }

            using StreamWriter sw = new($@"{ChampionshipPath}\{race.Championship.Name}\{race.Track.Name}-WhatHappened-log.txt");
            List<(string Name, int PositionGain, string LastAction)> list = race.GetWhatHappened();
            foreach (var info in list)
            {
                if (info.PositionGain > 0)
                {
                    sw.WriteLine($"{info.Name} gained {info.PositionGain} places after doing {info.LastAction}");
                }
                else if (info.PositionGain < 0)
                {
                    sw.WriteLine($"{info.Name} gained {info.PositionGain} places after doing {info.LastAction}");
                }
                else
                {
                    sw.WriteLine($"{info.Name} remained in their position after doing {info.LastAction}");
                }
            }
        }
    }
}
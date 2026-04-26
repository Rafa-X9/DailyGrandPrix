using DailyGrandPrix.Enums;
using DailyGrandPrix.Exceptions;
using Microsoft.Extensions.FileProviders;
using System.Text.Json.Serialization;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DailyGrandPrix.Entities
{
    internal class DriverRace : IComparable
    {
        [JsonIgnore] public Driver Driver { get; set; }
        public int DriverId { get; }
        [JsonIgnore] public Race Race { get; set; }
        public int RaceId
        {
            get
            {
                return Race.Id;
            }
        }
        public Tyres TyreCompound { get; set; } = Tyres.None;
        public int TyreWear { get; set; }
        public int TyreChanges { get; set; }
        public int FuelAmount { get; set; }
        public int MovesMade { get; set; }
        public Actions LastAction { get; set; } = Actions.None;
        public List<int> StepsHistory { get; set; } = new();
        public int StepsDriven
        {
            get
            {
                int sum = 0;
                StepsHistory.ForEach(x => sum += x);
                return sum;
            }
        }
        public int? FinalPosition { get; set; } = null;
        public bool HasRetired { get; set; } = false;
        public DriverClass DriverClass { get; set; }

        public DriverRace(Dictionary<string, object> json)
        {
            //DriverId
            if (!json.TryGetValue("DriverId", out object? driverId))
            {
                throw new ArgumentException("JSON didn't have a DriverId key for DriverRace");
            }
            if (!int.TryParse(driverId.ToString(), out int driverIdInt))
            {
                throw new ArgumentException("JSON's DriverId value for DriverRace wasn't a proper integer");
            }
            DriverId = driverIdInt;


            //TyreCompound
            if (!json.TryGetValue("TyreCompound", out object? comp) || comp is null || !int.TryParse(comp.ToString(), out int compInt) || !Enum.IsDefined(typeof(Tyres), compInt))
            {
                throw new ArgumentException("JSON didn't have a TyreCompound key for DriverRace or it wasn't a member of Tyres enumeration");
            }
            TyreCompound = (Tyres)compInt;


            //TyreWear
            if (!json.TryGetValue("TyreWear", out object? wear))
            {
                throw new ArgumentException("JSON didn't have a TyreWear key for DriverRace");
            }
            if (!int.TryParse(wear.ToString(), out int wearInt))
            {
                throw new ArgumentException("JSON's TyreWear key for DriverRace wasn't a valid integer");
            }
            TyreWear = wearInt;


            //TyreChanges
            if (!json.TryGetValue("TyreChanges", out object? changes))
            {
                throw new ArgumentException("JSON didn't have a TyreChanges key for DriverRace");
            }
            if (!int.TryParse(changes.ToString(), out int changesInt))
            {
                throw new ArgumentException("JSON's TyreChanges key for DriverRace wasn't a valid integer");
            }
            TyreChanges = changesInt;


            //FuelAmount
            if (!json.TryGetValue("FuelAmount", out object? amount))
            {
                throw new ArgumentException("JSON didn't have a FuelAmount key for DriverRace");
            }
            if (!int.TryParse(amount.ToString(), out int amountInt))
            {
                throw new ArgumentException("JSON's FuelAmount key for DriverRace wasn't a valid integer");
            }
            FuelAmount = amountInt;


            //MovesMade
            if (!json.TryGetValue("MovesMade", out object? moves))
            {
                throw new ArgumentException("JSON didn't have a MovesMade key for DriverRace");
            }
            if (!int.TryParse(moves.ToString(), out int movesInt))
            {
                throw new ArgumentException("JSON's MovesMade key for DriverRace wasn't a valid integer");
            }
            MovesMade = movesInt;


            //LastAction
            if (!json.TryGetValue("LastAction", out object? action) || action is null || !int.TryParse(action.ToString(), out int actionInt) || !Enum.IsDefined(typeof(Actions), actionInt))
            {
                throw new ArgumentException("JSON didn't have a LastAction key for DriverRace or it wasn't a member of Tyres enumeration");
            }
            LastAction = (Actions)actionInt;


            //StepsHistory
            if (json.TryGetValue("StepsHistory", out object? steps) && steps is not null && steps is List<int> stepsList)
            {
                StepsHistory = stepsList;
            }

            //FinalPosition
            if (json.TryGetValue("FinalPosition", out object? pos) && pos is not null && int.TryParse(pos.ToString(), out int posInt))
            {
                FinalPosition = posInt;
            }
            else
            {
                FinalPosition = null;
            }


            //HasRetired
            if (!json.TryGetValue("HasRetired", out object? hasRetired) || hasRetired is null || !bool.TryParse(hasRetired.ToString(), out bool hasRetiredBool))
            {
                throw new ArgumentException("JSON didn't have a HasRetired key for DriverRace or it wasn't a proper boolean");
            }
            HasRetired = hasRetiredBool;


            //DriverClass
            if (!json.TryGetValue("DriverClass", out object? driverClass) || driverClass is null || !int.TryParse(driverClass.ToString(), out int classInt) || !Enum.IsDefined(typeof(DriverClass), classInt))
            {
                throw new ArgumentException("JSON didn't have a DriverClass key for DriverRace or it wasn't a member of DriverClass enumeration");
            }
            DriverClass = (DriverClass)classInt;
        }

        public DriverRace(Driver driver, Race race)
        {
            Driver = driver;
            DriverId = driver.Id;
            Race = race;
        }

        public DriverRace(Tyres tyreCompound, int fuelAmount)
        {
            TyreCompound = tyreCompound;
            TyreWear = 100;
            TyreChanges = 0;
            FuelAmount = fuelAmount;
            MovesMade = 0;
        }

        public DriverRace(Tyres tyreCompound, int tyreWear,
            int tyreChanges, int fuelAmount, int movesMade,
            Actions lastAction, List<int> stepsHistory, DriverClass driverClass)
            : this(tyreCompound, tyreWear)
        {
            TyreChanges = tyreChanges;
            FuelAmount = fuelAmount;
            MovesMade = movesMade;
            LastAction = lastAction;
            StepsHistory = stepsHistory;
            DriverClass = driverClass;
        }

        public void MakeMove(Actions action)
        {
            if (HasRetired)
            {
                throw new DriverHasRetiredException("This driver has retired!");
            }

            int wear = 0;
            switch (TyreCompound)
            {
                case Tyres.Softs:
                    wear = 20;
                    break;
                case Tyres.Mediums:
                    wear = 12;
                    break;
                case Tyres.Hards:
                    wear = 7;
                    break;
                case Tyres.Intermediates:
                    wear = (Race.RainFactor > 10) ? 15 : 30;
                    break;
            }

            if (action == Actions.Push && (TyreWear - (wear * 2) < 0 || (TyreWear == 100 && MovesMade > 0)))
            {
                action = Actions.Conserve;
            }
            if (action == Actions.Push && Race.RainFactor <= 10 && TyreCompound == Tyres.Intermediates)
            {
                action = Actions.Conserve;
            }

            List<int> gaps = new();
            foreach (DriverRace dr in Race.Drivers)
            {
                if (dr.Driver.Id == Driver.Id) continue;

                if (dr.MovesMade > MovesMade)
                {
                    if ((dr.StepsDriven - dr.StepsHistory.Last()) - StepsDriven > 0)
                    {
                        gaps.Add((dr.StepsDriven - dr.StepsHistory.Last()) - StepsDriven);
                    }
                }
                else if (dr.MovesMade == MovesMade)
                {
                    if (dr.StepsDriven - StepsDriven > 0)
                    {
                        gaps.Add(dr.StepsDriven - StepsDriven);
                    }
                }
            }

            int slipstream;
            if (gaps.Count > 0)
            {
                gaps.Sort();
                slipstream = gaps[0];
            }
            else
            {
                slipstream = 0;
            }

            int steps = CalculateStep(action == Actions.Push, slipstream);
            StepsHistory.Add(steps);

            if (action == Actions.Push)
            {
                TyreWear -= wear * 2;
                FuelAmount -= 10;
                LastAction = Actions.Push;
            }
            else
            {
                if (DriverClass == DriverClass.OscarPiastri)
                {
                    switch (TyreCompound)
                    {
                        case Tyres.Softs:
                            wear = 14;
                            break;
                        case Tyres.Mediums:
                            wear = 10;
                            break;
                        case Tyres.Hards:
                            wear = 5;
                            break;
                    }
                }
                TyreWear -= wear;
                if (DriverClass == DriverClass.SebastianVettel) FuelAmount -= 3;
                else FuelAmount -= 5;
                LastAction = Actions.Conserve;
            }

            if (TyreWear < 0) TyreWear = 0;
            if (FuelAmount < 0) HasRetired = true;

            MovesMade++;

            Console.WriteLine("Move made.");
            Console.WriteLine("Steps moved: " + steps);
            Console.WriteLine("Tyres: " + TyreCompound + ", " + TyreWear + "/100");
            Console.WriteLine("Fuel: " + FuelAmount + "/100");
            //Console.WriteLine("Press enter to continue");
            //Console.ReadLine();
        }

        public void ChangeTyres(Tyres newTyres)
        {
            if (HasRetired)
            {
                throw new DriverHasRetiredException("This driver has retired!");
            }

            MovesMade++;
            TyreChanges++;
            TyreCompound = newTyres;
            TyreWear = 100;
            StepsHistory.Add(0);
            LastAction = Actions.Pit;
            Console.WriteLine("Move made.");
            Console.WriteLine("Changed to " + newTyres);
            Console.WriteLine("Tyres: " + TyreCompound + ", " + TyreWear + "/100");
            Console.WriteLine("Fuel: " + FuelAmount + "/100");
            //Console.WriteLine("Press enter to continue");
            //Console.ReadLine();
        }

        public int CalculateStep(bool IsPushing, int GapAhead)
        {
            double CompFactor = 1 - (0.1 * ((int)TyreCompound - 1));
            double LifeFactor = (double)TyreWear / 100;
            double FuelFactor = 1 - ((double)FuelAmount / 100);
            double Slipstream;
            if (!(GapAhead <= 0 || GapAhead > 20))
            {
                List<int> invert = new()
                {
                    20, 19, 18, 17, 16,
                    15, 14, 13, 12, 11,
                    10, 9, 8, 7 , 6,
                    5, 4, 3, 2, 1
                };
                GapAhead = invert[GapAhead - 1] + 1;
            }
            Slipstream = Math.Max(0, (double)GapAhead / 20);

            int baseSteps;

            if (!IsPushing) baseSteps = (int)Math.Ceiling(((2.5 + (12.5 * (CompFactor * LifeFactor * (0.6 + (0.4 * FuelFactor))))) * 2.5) * (1 + (0.15 * Slipstream)));
            else if (DriverClass != DriverClass.GeorgeRussel) baseSteps = (int)Math.Ceiling(((2.5 + (12.5 * (CompFactor * LifeFactor * (0.6 + (0.4 * FuelFactor))))) * 3.25) * (1 + (0.15 * Slipstream)));
            else baseSteps = (int)Math.Ceiling(((2.5 + (12.5 * (CompFactor * LifeFactor * (0.6 + (0.4 * FuelFactor))))) * 3.75) * (1 + (0.15 * Slipstream)));

            switch (TyreCompound)
            {
                case Tyres.Softs:
                    baseSteps = (int)Math.Ceiling(baseSteps / (Race.RainFactor / 5.0 + 1.0));
                    break;
                case Tyres.Mediums:
                    baseSteps = (int)Math.Ceiling(baseSteps / (Race.RainFactor / 12.0 + 1.0));
                    break;
                case Tyres.Hards:
                    baseSteps = (int)Math.Ceiling(baseSteps / (Race.RainFactor / 20.0 + 1.0));
                    break;
                case Tyres.Intermediates:
                    double strollFactor = (DriverClass == DriverClass.LanceStroll) ? 1.0 : 1.3;
                    double divisor = 0.0104 * ((Race.RainFactor - 25) * (Race.RainFactor - 25)) + strollFactor;
                    baseSteps = (int)Math.Ceiling(baseSteps / divisor);
                    break;
                default:
                    throw new InvalidOperationException("Driver doesn't have a tyre");
            }  

            return baseSteps;
        }

        public void CheckFinish(Race race, int position)
        {
            if (StepsDriven >= race.Track.StepsPerLap * race.Track.RaceLaps)
            {
                FinalPosition = position;
            }
        }

        public int CompareTo(object? obj)
        {
            if (obj is not DriverRace)
            {
                throw new ArgumentException("Tried comparing DriverRace to other type");
            }
            if (obj is null)
            {
                throw new ArgumentException("Tried comparing DriverRace to null type");
            }

            DriverRace other = (DriverRace)obj;

            if (StepsHistory.Count == 0 && other.StepsHistory.Count == 0)
            {
                return 0;
            }

            if (HasRetired || other.HasRetired)
            {
                if (HasRetired) return 1;
                else return -1;
            }
            else
            {
                if (FinalPosition == null && other.FinalPosition == null)
                {
                    return other.StepsDriven.CompareTo(StepsDriven);
                }
                else
                {
                    if (FinalPosition != null && other.FinalPosition == null)
                    {
                        return -1;
                    }
                    else if (other.FinalPosition != null && FinalPosition == null)
                    {
                        return 1;
                    }
                    else
                    {
                        if (MovesMade != other.MovesMade)
                        {
                            if (FinalPosition < other.FinalPosition) return -1;
                            else return 1;
                        }
                        else
                        {
                            int NeededThis = (Race.Track.StepsPerLap * Race.Track.RaceLaps)
                                - (StepsDriven - StepsHistory.Last());

                            int NeededOther = (Race.Track.StepsPerLap * Race.Track.RaceLaps)
                                - (other.StepsDriven - other.StepsHistory.Last());

                            double factorThis = (double)NeededThis / StepsHistory.Last();
                            double factorOther = (double)NeededOther / other.StepsHistory.Last();

                            if (factorThis < factorOther)
                            {
                                return -1;
                            }
                            else
                            {
                                return 1;
                            }
                        }
                    }
                }
            }
        }
    }
}
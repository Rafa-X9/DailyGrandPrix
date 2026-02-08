using DailyGrandPrix.Enums;
using DailyGrandPrix.Exceptions;

namespace DailyGrandPrix.Entities
{
    internal class DriverRace : IComparable
    {
        public Driver Driver { get; set; }
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
        public Race? Race { get; set; } = null;

        public DriverRace(Driver driver)
        {
            Driver = driver;
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
            }

            if (TyreWear - (wear * 2) < 0 && action == Actions.Push)
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
            Console.WriteLine("Press enter to continue");
            Console.ReadLine();
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
            Console.WriteLine("Press enter to continue");
            Console.ReadLine();
        }

        public int CalculateStep(bool IsPushing, int GapAhead)
        {
            double CompFactor = (double)1 - (0.1 * ((int)TyreCompound - 1));
            double LifeFactor = (double)TyreWear / 100;
            double FuelFactor = (double)1 - (FuelAmount / 100);
            double Slipstream = Math.Max(0, ((double)GapAhead / 20));

            if (!IsPushing) return (int)Math.Ceiling(((2.5 + (12.5 * (CompFactor * LifeFactor * (0.6 + (0.4 * FuelFactor))))) * 2.5) * (1 + (0.15 * Slipstream)));
            else if (DriverClass != DriverClass.GeorgeRussel) return (int)Math.Ceiling(((2.5 + (12.5 * (CompFactor * LifeFactor * (0.6 + (0.4 * FuelFactor))))) * 3.25) * (1 + (0.15 * Slipstream)));
            else return (int)Math.Ceiling(((2.5 + (12.5 * (CompFactor * LifeFactor * (0.6 + (0.4 * FuelFactor))))) * 3.25) * (1 + (0.15 * Slipstream)));
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

            DriverRace other = obj as DriverRace;

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
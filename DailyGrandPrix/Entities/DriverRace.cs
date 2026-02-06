using DailyGrandPrix.Enums;

namespace DailyGrandPrix.Entities
{
    internal class DriverRace
    {
        public Driver Driver { get; set; }
        public Tyres TyreCompound { get; set; } = Tyres.None;
        public int TyreWear { get; set; }
        public int TyreChanges { get; set; }
        public int FuelAmount { get; set; }
        public int MovesMade { get; set; }
        public Actions LastAction { get; set; } = Actions.None;
        public List<int> StepsHistory { get; set; } = new();

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
            Actions lastAction, List<int> stepsHistory) : this(tyreCompound, tyreWear)
        {
            TyreChanges = tyreChanges;
            FuelAmount = fuelAmount;
            MovesMade = movesMade;
            LastAction = lastAction;
            StepsHistory = stepsHistory;
        }

        public void MakeMove()
        {
            throw new NotImplementedException();
        }

        public void ChangeTyres()
        {
            throw new NotImplementedException();
        }
    }
}

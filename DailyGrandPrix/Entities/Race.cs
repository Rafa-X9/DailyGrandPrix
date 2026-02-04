using DailyGrandPrix.Enums;

namespace DailyGrandPrix.Entities
{
    internal class Race
    {
        public int Id { get; private set; }
        public DateOnly Start { get; private set; }
        public DateOnly? End { get; private set; }
        public List<DriverRace> Drivers { get; private set; } = new();
        public Championship Championship { get; private set; }
        public RaceState RaceState { get; private set; }
        public Track Track { get; private set; } = new();
    }
}

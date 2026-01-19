using DailyGrandPrix.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DailyGrandPrix.Exceptions;

namespace DailyGrandPrix.Entities
{
    internal class Driver
    {
        //personal info
        public string Name { get; set; }
        public string Username { get; set; }
        public int Number { get; set; }
        public Teams Team { get; set; }

        //car info
        public Tyres Tyres { get; set; }
        public int TyreWear { get; set; }
        public int TyreChanges { get; set; }
        public int Fuel { get; set; }

        //race info
        public int StepsDriven { get; set; }
        public int MovesMade { get; set; }
        public int LapsDriven { get
            {
                return (int) Math.Floor((double)StepsDriven / Services.StepsPerLap);
            }
        }
        public int StepsInLap { get
            {
                return StepsDriven - (LapsDriven * Services.StepsPerLap);
            }
        }

        //championship info
        public int Points { get; set; }
        public int Wins { get; set; }
        public int Podiums { get; set; }


        public Driver(string name, string username, int number, Teams team)
        {
            Name = name;
            Username = username;
            Number = number;
            Team = team;
            Points = 0;
            Wins = 0;
            Podiums = 0;
        }

        public Driver(StreamReader sr, bool OnlyPersonalInfo)
        {
            //personal info
            string[] line = sr.ReadLine().Split(',');
            Name = line[0];
            Username = line[1];
            Number = int.Parse(line[2]);
            Team = Enum.Parse<Teams>(line[3]);

            if (!OnlyPersonalInfo)
            {
                line = sr.ReadLine().Split(',');
                Tyres = Enum.Parse<Tyres>(line[0]);
                TyreWear = int.Parse(line[1]);
                TyreChanges = int.Parse(line[2]);
                Fuel = int.Parse(line[3]);

                line = sr.ReadLine().Split(',');
                StepsDriven = int.Parse(line[0]);
                MovesMade = int.Parse(line[1]);

                line = sr.ReadLine().Split(',');
                Points = int.Parse(line[0]);
                Wins = int.Parse(line[1]);
                Podiums = int.Parse(line[2]);
            }
        }


        public void MakeStep(bool IsPushing)
        {
            int steps = Services.CalculateSteps((int)Tyres, TyreWear, Fuel, IsPushing);

            if (!IsPushing)
            {
                switch (Tyres)
                {
                    case Tyres.Softs:
                        TyreWear -= 20;
                        break;
                    case Tyres.Mediums:
                        TyreWear -= 12;
                        break;
                    case Tyres.Hards:
                        TyreWear -= 7;
                        break;
                }
                Fuel -= 5;
            }
            else
            {
                switch (Tyres)
                {
                    case Tyres.Softs:
                        TyreWear -= 40;
                        break;
                    case Tyres.Mediums:
                        TyreWear -= 24;
                        break;
                    case Tyres.Hards:
                        TyreWear -= 14;
                        break;
                }
                Fuel -= 10;
            }

            if (TyreWear < 0 && LapsDriven < Services.RaceLaps)
            {
                StepsDriven = -1;
                throw new Puncture("Tyres have worn out");
            }

            if (Fuel < 0 && LapsDriven < Services.RaceLaps)
            {
                StepsDriven = -1;
                throw new OutOfFuel("Car has run out of fuel");
            }

            StepsDriven += steps;
            MovesMade++;
        }

        public void ChangeTyres(Tyres NewTyres)
        {
            Tyres = NewTyres;
            TyreChanges++;
            TyreWear = 100;
            MovesMade++;
        }

        public override string ToString()
        {
            return "Name = " + Name
                + "\nUsername = " + Username
                + "\nNumber = " + Number
                + "\nTeam = " + Team;
        }
    }
}

using System.Collections.Generic;

namespace SimpleFlightComputer
{
    public static class Test
    {
        public const double G = 9.80665; // [m/s²]

        public static SpaceShip CreateAvenger()
        {
            SpaceShip ship = new SpaceShip();
            ship.Mass = 50040;
            ship.MaxThrust = new DVector3(3.4, 4.3, 8.4) * G * ship.Mass;
            ship.MinThrust = new DVector3(-3.4, -4.3, -4.7) * G * ship.Mass;
            return ship;
        }

        /// <summary>
        /// The maximum step count. This is just a safety to prevent an infinite loop in case there is a bug.
        /// </summary>
        public static int MaxStepCount = 20000;

        /// <summary>
        /// Performs the maneuver.
        /// </summary>
        /// <param name="ship">The ship.</param>
        /// <param name="flightComputer">The flight computer.</param>
        /// <param name="states">The states.</param>
        /// <returns></returns>
        public static bool PerformManeuver(
            SpaceShip ship,
            FlightComputer flightComputer,
            out List<(double timestamp, DVector3 velocity, DVector3 position)> states)
        {
            states = new List<(double timestamp, DVector3 velocity, DVector3 position)>();

            double timestamp = 0;

            // add current velocity and position
            states.Add((timestamp, ship.Speed * ship.FlightDirection, ship.Position));

            bool finishedSuccessfully = true;

            // simple check whether maneuver is finished
            while ((ship.DesiredSpeed * ship.DesiredFlightDirection - ship.Speed * ship.FlightDirection).Length() > 0.1)
            {
                // determine thrust and time remaining to complete maneuver
                DVector3 thrust = flightComputer.ComputeThrust(ship);

                // apply thrust to ship
                ship.ApplyThrust(thrust, flightComputer.UpdateThrustInterval);

                // update timestamp
                timestamp += flightComputer.UpdateThrustInterval;

                // add updated velocity and position to lists
                states.Add((timestamp, ship.Speed * ship.FlightDirection, ship.Position));

                // just to prevent infinite loop in case of a bug
                if (states.Count >= MaxStepCount)
                {
                    finishedSuccessfully = false;
                    break;
                }
            }

            return finishedSuccessfully;
        }
    }
}
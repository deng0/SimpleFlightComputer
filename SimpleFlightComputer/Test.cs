using System;
using System.Collections.Generic;

namespace SimpleFlightComputer
{
    public static class Test
    {
        public static SpaceShip CreateAvenger()
        {
            SpaceShip ship = new SpaceShip();
            ship.Mass = 50040;
            ship.MaxThrust = new DVector3(3.4, 4.3, 8.4) * Constants.G * ship.Mass;
            ship.MinThrust = new DVector3(-3.4, -4.3, -4.7) * Constants.G * ship.Mass;
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
        /// <param name="timeStep">The time step in seconds.</param>
        /// <param name="computeThrustFunc">The compute thrust function.</param>
        /// <param name="states">The states.</param>
        public static bool PerformManeuver(
            SpaceShip ship,
            double timeStep,
            Func<SpaceShip, (DVector3 thrust, double nextThrustChange, double remaintingTime)> computeThrustFunc,
            out List<(double timestamp, DVector3 velocity, DVector3 position)> states)
        {
            states = new List<(double timestamp, DVector3 velocity, DVector3 position)>();

            double timestamp = 0;

            // add current velocity and position
            states.Add((timestamp, ship.Velocity, ship.Position));

            bool finishedSuccessfully = true;

            while ((ship.DesiredVelocity - ship.Velocity).Length() > 0.1)
            {
                // determine thrust and time remaining to complete maneuver
                (DVector3 thrust, double nextThrustChange, double remainingTime) = computeThrustFunc(ship);

                double thrustTime = timeStep;

                // prevent overshooting
                if (nextThrustChange > 0)
                {
                    thrustTime = Math.Min(nextThrustChange, thrustTime);
                }

                ship.ApplyThrust(thrust, thrustTime);

                timestamp += thrustTime;

                // add updated velocity and position to lists
                states.Add((timestamp, ship.Velocity, ship.Position));

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
using System;

namespace SimpleFlightComputer
{
    public static class FlightComputer
    {
        /// <summary>
        /// Determines the maximum thrust.
        /// </summary>
        /// <param name="ship">The ship.</param>
        /// <returns>The thrust and the time needed to complete maneuver.</returns>
        public static (DVector3 thrust, double nextAccelerationChange, double totalTime) DetermineMaxThrust(SpaceShip ship)
        {
            DVector3 maxThrust = ship.DetermineMaxAvailableThrust(ship.DesiredVelocity - ship.Velocity);
            DVector3 maxAcceleration = maxThrust / ship.Mass;
            DVector3 burnTimes = DetermineAccelerationTimes(maxAcceleration, ship.DesiredVelocity - ship.Velocity);
            double maxBurnTime = Math.Max(burnTimes.X, Math.Max(burnTimes.Y, burnTimes.Z));
            double nextAccelerationChange = maxBurnTime;
            DetermineNextAccelerationChange(burnTimes, ref nextAccelerationChange);
            return (maxThrust, nextAccelerationChange, maxBurnTime);
        }

        /// <summary>
        /// Determines the stable thrust.
        /// </summary>
        /// <param name="ship">The ship.</param>
        /// <returns>The thrust and the time needed to complete maneuver.</returns>
        public static (DVector3 thrust, double nextAccelerationChange, double totalTime) DetermineStableThrust(SpaceShip ship)
        {
            DVector3 maxThrust = ship.DetermineMaxAvailableThrust(ship.DesiredVelocity - ship.Velocity);
            DVector3 maxAcceleration = maxThrust / ship.Mass;
            DVector3 burnTimes = DetermineAccelerationTimes(maxAcceleration, ship.DesiredVelocity - ship.Velocity);
            double maxBurnTime = Math.Max(burnTimes.X, Math.Max(burnTimes.Y, burnTimes.Z));

            //v = a * t
            //a = F / m
            //F = v / t * m
            DVector3 stableThrust = (ship.DesiredVelocity - ship.Velocity) / maxBurnTime * ship.Mass;
            return (stableThrust, maxBurnTime, maxBurnTime);
        }

        /// <summary>
        /// Determines the stable anti drift thrust.
        /// What we want to achieve is to eliminate drift as fast as possible without increasing the total time and also when already traveling in the desired direction to keep a stable acceleration direction.
        /// </summary>
        /// <param name="ship">The ship.</param>
        /// <returns>The thrust and the time needed to complete maneuver.</returns>
        public static (DVector3 thrust, double nextAccelerationChange, double totalTime) DetermineStableAntiDriftThrust(SpaceShip ship)
        {
            DVector3 vc = ship.Velocity;
            DVector3 vd = ship.DesiredVelocity;

            // determine the max thrust available for each of the three axis that will push the ship towards the desired velocity
            DVector3 maxThrust = ship.DetermineMaxAvailableThrust(vd - vc);

            // the resulting max acceleration (am) will be
            DVector3 am = maxThrust / ship.Mass;

            // determine the burn times (tm) needed for each thrust axis to achieve the correct velocity along that axis when it would use max acceleration available (am)
            DVector3 tm = DetermineAccelerationTimes(am, vd - vc);

            // determine the max/total time (tt) it will take to fully achieve the desired velocity vector which will obviously be
            double tt = Math.Max(tm.X, Math.Max(tm.Y, tm.Z));

            // if no burnTime is needed we obviously don't have to compute any thrust
            if (Helpers.IsZero(tt))
            {
                return (DVector3.Zero, tt, tt);
            }

            // just a small helper to prevent oszillating
            double nextAccelerationChange = tt;
            DetermineNextAccelerationChange(tm, ref nextAccelerationChange);

            if (ship.SpacebrakeEngaged)
            {
                // always use max thrust when spacebrake is enaged.
                return (maxThrust, nextAccelerationChange, tt);
            }

            // determine the acceleration (aS) that would achieve the new velocity vector in a stable manner
            DVector3 aS = (vd - vc) / tt;
            DVector3 stableThrust = aS * ship.Mass;

            // if pilot wants to stop without spacebreak, use stableThrust for a more predictable flight path
            if (Helpers.IsZero(vd.Length()))
            {
                return (stableThrust, tt, tt);
            }

            // determine the acceleration (ad) the ship would do when starting at standstill and accelerating along the desired velocity vector
            // first determine max available thrust that would be available for that
            DVector3 amd = ship.DetermineMaxAvailableThrust(vd) / ship.Mass;

            // determine burn times needed
            DVector3 td = DetermineAccelerationTimes(amd, vd);

            // determine theoretical max/total burn time needed
            double ttd = Math.Max(td.X, Math.Max(td.Y, td.Z));

            // now device desired velocity vector by needed time to get the acceleration
            DVector3 ad = vd / ttd;

            // for each thrust axis we use
            // function that describes the velocity at time t when am is used starting at vc
            // fvm(t) = vc + am * t

            // function that describes the velocity at time t when ad is used ending at vd
            // fvd(t) = vd + ad * (t - tt)

            // determine intersection (ti) of these lines
            // vc + am * ti = vd + ad * (ti - tt)
            // am * ti - ad * ti = vd - vc - ad * tt
            DVector3 ti = (vd - vc - ad * tt) / (am - ad);

            // determine the thrust we want to perform, we either use the stable thrust or the max thrust depending on where the intersection was
            DVector3 thrust;
            thrust.X = ti.X < 0 || ti.X > tt ? stableThrust.X : maxThrust.X;
            thrust.Y = ti.Y < 0 || ti.Y > tt ? stableThrust.Y : maxThrust.Y;
            thrust.Z = ti.Z < 0 || ti.Z > tt ? stableThrust.Z : maxThrust.Z;

            // update nextAccelerationChange if needed
            DetermineNextAccelerationChange(ti, ref nextAccelerationChange);

            return (thrust, nextAccelerationChange, tt);
        }

        private static DVector3 DetermineAccelerationTimes(in DVector3 acceleration, in DVector3 velocityChange)
        {
            DVector3 times = DVector3.Zero;

            if (!Helpers.IsZero(velocityChange.X))
            {
                times.X = velocityChange.X / acceleration.X;
            }
            if (!Helpers.IsZero(velocityChange.Y))
            {
                times.Y = velocityChange.Y / acceleration.Y;
            }
            if (!Helpers.IsZero(velocityChange.Z))
            {
                times.Z = velocityChange.Z / acceleration.Z;
            }

            return times;
        }

        private static void DetermineNextAccelerationChange(in DVector3 burnTimes, ref double nextChange)
        {
            if (burnTimes.X > 0 && burnTimes.X < nextChange)
            {
                nextChange = burnTimes.X;
            }

            if (burnTimes.Y > 0 && burnTimes.Y < nextChange)
            {
                nextChange = burnTimes.X;
            }

            if (burnTimes.Z > 0 && burnTimes.Z < nextChange)
            {
                nextChange = burnTimes.Z;
            }
        }
    }
}
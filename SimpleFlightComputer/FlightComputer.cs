using System;

namespace SimpleFlightComputer
{
    /// <summary>
    /// This class implements different modes how to determine thrust.
    ///
    /// basic physical formulas used:
    ///   velocity = acceleration * time
    ///   acceleration = thrust / mass
    ///   thrust = velocity / time * mass
    /// </summary>
    public class FlightComputer
    {
        /// <summary>
        /// The UpdateThrustInterval in s.
        /// </summary>
        public double UpdateThrustInterval = 0.01;

        /// <summary>
        /// When this is disabled, simple "max thrust" mode is used.
        /// </summary>
        public bool PrioritizeStability = true;

        /// <summary>
        /// Use maximum thrust available when spacebrake engaged, which may result in unpredictable flight path.
        /// </summary>
        public bool UseMaxThrustWhenSpacebrakeEngaged = false;

        /// <summary>
        /// Whether to use anti-drift. This has no effect when PrioritizeStability is disabled.
        /// </summary>
        public bool UseAntiDrift = true;

        /// <summary>
        /// Whether to use anti-drift while braking. This has no effect when PrioritizeStability or UseAntiDrift is disabled.
        /// </summary>
        public bool UseAntiDriftBraking = true;

        /// <summary>
        /// An option to clamp the anti drift ratio.
        /// The anti drift ratio is acceleration along desired flight direction vs acceleration to counter drift
        /// and is determined by the ship's capabilities to acceleration in these directions.
        /// Ssetting this to 1 would mean that the change of thrust direction, when drift has been eliminated, will not be larger than 45 degrees.
        /// </summary>
        public double ClampAntiDriftRatio = 0;

        /// <summary>
        /// Computes the thrust to perform at this moment.
        /// </summary>
        /// <param name="ship">The ship.</param>
        /// <returns>
        /// The thrust.
        /// </returns>
        public DVector3 ComputeThrust(SpaceShip ship)
        {
            // compute desired velocity change
            DVector3 desiredVelocityChange = ship.DesiredFlightDirection * ship.DesiredSpeed - ship.FlightDirection * ship.Speed;

            // determine thrust needed to get to the desired velocity within one UpdateThrustInterval
            DVector3 thrustToDesired = desiredVelocityChange * ship.Mass / this.UpdateThrustInterval;

            // clamp thrustToDesired with ship's capabilities
            DVector3 clampedThrustToDesired = ship.ClampThrust(thrustToDesired, out int maxedAxis);

            // unless overriden use this thrust as the resulting thrust
            DVector3 thrust = clampedThrustToDesired;

            // if no thrust axis is maxed out, just use that thrust
            if (maxedAxis < 0)
            {
                return clampedThrustToDesired;
            }

            // check if "max thrust" mode should be used
            if (!this.PrioritizeStability || this.UseMaxThrustWhenSpacebrakeEngaged && ship.SpacebrakeEngaged)
            {
                // determine the max available thrust along the needed thrust axis
                DVector3 maxThrust = ship.GetMaxAvailableThrust(
                    Math.Sign(thrustToDesired.X),
                    Math.Sign(thrustToDesired.Y),
                    Math.Sign(thrustToDesired.Z));

                // only use maxThrust along the correspondig axis if that does not exceed thrustToDesired
                if (maxThrust.X / thrustToDesired.X < 1)
                {
                    thrust.X = maxThrust.X;
                }
                if (maxThrust.Y / thrustToDesired.Y < 1)
                {
                    thrust.Y = maxThrust.Y;
                }
                if (maxThrust.Z / thrustToDesired.Z < 1)
                {
                    thrust.Z = maxThrust.Z;
                }
            }
            // if desired speed is 0 and AntiDriftBraking is not enabled use stable thrust
            else if (this.UseAntiDrift && (ship.DesiredSpeed > 0 || this.UseAntiDriftBraking))
            {
                // use normalized thrust along flight direction (same as the desired flight direction)
                DVector3 thrustAlongDesiredNormalized = ship.DesiredFlightDirection;

                // project vector thrustToDesired to thrustAlongDesiredNormalized 
                double thrustDesiredToNoDriftLength = DVector3.Dot(thrustToDesired, thrustAlongDesiredNormalized);
                // with that we can determine the thrust to no drift
                DVector3 thrustToNoDrift = thrustToDesired - thrustDesiredToNoDriftLength * thrustAlongDesiredNormalized;

                // and again the thrust needed to get from no drift to desired speed
                DVector3 thrustNoDriftToDesired = thrustToDesired - thrustToNoDrift;

                // compute lengths of thrustToNoDrift and thrustNoDriftToDesired
                double thrustToNoDriftLength = thrustToNoDrift.Length();
                double thrustNoDriftToDesiredLength = Math.Abs(thrustDesiredToNoDriftLength); // same as thrustNoDriftToDesired.Length

                // check if there actually is drift to eliminate
                if (thrustToNoDriftLength > 0)
                {
                    // compute lengths of clamped versions of thrustToNoDrift and thrustNoDriftToDesired to determine thrust power ratio
                    double clampedThrustToNoDriftLength = ship.ClampThrust(thrustToNoDrift, out _).Length();
                    double clampedThrustNoDriftToDesiredLength = ship.ClampThrust(thrustNoDriftToDesired, out _).Length();

                    // compute thrust ratio that counters drift more effectively while applying the clamp
                    double optRatio = Math.Max(this.ClampAntiDriftRatio, clampedThrustNoDriftToDesiredLength / clampedThrustToNoDriftLength);

                    // if using that optimized thrust ratio would not overshoot desired speed in desired flight direction
                    if (thrustNoDriftToDesiredLength / thrustToNoDriftLength > optRatio)
                    {
                        // use the thrust that results by using that ratio
                        thrust = ship.ClampThrust(
                            thrustToNoDrift + thrustNoDriftToDesired * (thrustToNoDriftLength * optRatio / thrustNoDriftToDesiredLength),
                            out _);
                    }
                }
            }

            return thrust;
        }
    }
}
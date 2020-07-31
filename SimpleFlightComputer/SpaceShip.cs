namespace SimpleFlightComputer
{
    public class SpaceShip
    {
        /// <summary>
        /// The total mass [kg].
        /// </summary>
        public double Mass;

        /// <summary>
        /// The maximum thrust to the right, upward, forward [N].
        /// </summary>
        public DVector3 MaxThrust;
        /// <summary>
        /// The minimum (negative) thrust to the left, downward, backward [N].
        /// </summary>
        public DVector3 MinThrust;

        public double AfterburnerFactor = 2;

        /// <summary>
        /// The current position [m].
        /// </summary>
        public DVector3 Position;

        ///// <summary>
        ///// The current orientation.
        ///// </summary>
        //public Quaternion Orientation;

        /// <summary>
        /// The current velocity vector [m/s] in ship Orientation.
        /// </summary>
        public DVector3 Velocity;

        /// <summary>
        /// The desired velocity vector [m/s] in ship Orientation as specified by the pilot's input.
        /// </summary>
        public DVector3 DesiredVelocity;

        /// <summary>
        /// Whether the pilot has currently engaged afterburner.
        /// </summary>
        public bool AfterburnerEngaged;

        /// <summary>
        /// Whether the pilot has currently engaged spacebrake.
        /// </summary>
        public bool SpacebrakeEngaged;

        /// <summary>
        /// Determines the maximum available thrust to achieve the specified velocity change.
        /// If no velocity change is needed for the corresponding thrust axis, the returned thrust for that axis will be 0.
        /// The needed burn times for each thrust axis will be different.
        /// </summary>
        /// <param name="velocityChange">The velocity change.</param>
        /// <returns>The max available thrust.</returns>
        public DVector3 DetermineMaxAvailableThrust(DVector3 velocityChange)
        {
            DVector3 thrust = DVector3.Zero;
            if (!Helpers.IsZero(velocityChange.X))
            {
                thrust.X = velocityChange.X > 0 ? this.MaxThrust.X : this.MinThrust.X;
            }
            if (!Helpers.IsZero(velocityChange.Y))
            {
                thrust.Y = velocityChange.Y > 0 ? this.MaxThrust.Y : this.MinThrust.Y;
            }
            if (!Helpers.IsZero(velocityChange.Z))
            {
                thrust.Z = velocityChange.Z > 0 ? this.MaxThrust.Z : this.MinThrust.Z;
            }

            return (this.AfterburnerEngaged ? this.AfterburnerFactor : 1) * thrust;
        }

        /// <summary>
        /// Applies the thrust for the specified amount of time
        /// </summary>
        /// <param name="thrust">The thrust [N].</param>
        /// <param name="time">The time [s].</param>
        public void ApplyThrust(DVector3 thrust, double time)
        {
            // determine acceleration
            DVector3 acc = thrust / this.Mass;

            // update current position.
            this.Position += 0.5 * acc * time * time + this.Velocity * time;

            // update current velocity vector.
            this.Velocity += acc * time;
        }
    }
}
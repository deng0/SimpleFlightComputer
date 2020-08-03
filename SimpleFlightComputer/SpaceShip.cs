﻿namespace SimpleFlightComputer
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
        /// The current speed [m/s].
        /// </summary>
        public double Speed;

        /// <summary>
        /// The flight direction (normalized vector) in ship orientation.
        /// </summary>
        public DVector3 FlightDirection = new DVector3(0, 0, 1);

        /// <summary>
        /// The desired velocity vector [m/s] in ship orientation as specified by the pilot's input.
        /// </summary>
        public double DesiredSpeed;

        /// <summary>
        /// The desired flight direction (normalized vector) in ship orientation as specified by the pilot's input.
        /// </summary>
        public DVector3 DesiredFlightDirection = new DVector3(0, 0, 1);

        /// <summary>
        /// Whether the pilot has currently engaged afterburner.
        /// </summary>
        public bool AfterburnerEngaged;

        /// <summary>
        /// Whether the pilot has currently engaged spacebrake.
        /// </summary>
        public bool SpacebrakeEngaged;

        /// <summary>
        /// Gets the maximum thrust available along the required thrust axis.
        /// If the power along thrust axis is not indepentent this does not provide meaningful reasults.
        /// </summary>
        /// <param name="xAxis">Whether to go right (1) or left (-1).</param>
        /// <param name="yAxis">Whether to go up (1) or down (-1).</param>
        /// <param name="zAxis">Whether to go forward (1) or backward (-1).</param>
        /// <returns>
        /// The max available thrust (if thrust axis were independent).
        /// </returns>
        public DVector3 GetMaxAvailableThrust(int xAxis, int yAxis, int zAxis)
        {
            DVector3 thrust;
            thrust.X = xAxis == 0 ? 0.0 : xAxis > 0 ? this.MaxThrust.X : this.MinThrust.X;
            thrust.Y = yAxis == 0 ? 0.0 : yAxis > 0 ? this.MaxThrust.Y : this.MinThrust.Y;
            thrust.Z = zAxis == 0 ? 0.0 : zAxis > 0 ? this.MaxThrust.Z : this.MinThrust.Z;
            return (this.AfterburnerEngaged ? this.AfterburnerFactor : 1) * thrust;
        }

        public DVector3 ClampThrust(DVector3 thrust, out int maxedAxis)
        {
            DVector3 fac = new DVector3(1, 1, 1);

            if (thrust.X > this.MaxThrust.X)
            {
                fac.X = this.MaxThrust.X / thrust.X;
            }
            else if (thrust.X < this.MinThrust.X)
            {
                fac.X = this.MinThrust.X / thrust.X;
            }

            if (thrust.Y > this.MaxThrust.Y)
            {
                fac.Y = this.MaxThrust.Y / thrust.Y;
            }
            else if (thrust.Y < this.MinThrust.Y)
            {
                fac.Y = this.MinThrust.Y / thrust.Y;
            }

            if (thrust.Z > this.MaxThrust.Z)
            {
                fac.Z = this.MaxThrust.Z / thrust.Z;
            }
            else if (thrust.Z < this.MinThrust.Z)
            {
                fac.Z = this.MinThrust.Z / thrust.Z;
            }

            maxedAxis = -1;

            if (fac.X < fac.Y && fac.X < fac.Z)
            {
                maxedAxis = 0;
                return thrust * fac.X;
            }
            else if (fac.Y < fac.Z)
            {
                maxedAxis = 1;
                return thrust * fac.Y;
            }
            else if (fac.Z < 1)
            {
                maxedAxis = 2;
                return thrust * fac.Z;
            }

            return thrust;
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

            // current velocity vector
            DVector3 velocityVector = this.Speed * this.FlightDirection;

            // update current position
            this.Position += 0.5 * acc * time * time + velocityVector * time;

            // update current velocity vector.
            velocityVector += acc * time;
            this.Speed = velocityVector.Normalize();

            // just to make sure flight Direction is not set to an invalid direction
            if (this.Speed > 0)
            {
                this.FlightDirection = velocityVector;
            }
        }
    }
}
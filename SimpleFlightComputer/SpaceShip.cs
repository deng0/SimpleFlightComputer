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

        /// <summary>
        /// The afterburner factor.
        /// </summary>
        public double AfterburnerFactor = 2;

        /// <summary>
        /// The current ship orientation in the world.
        /// </summary>
        public Quaternion Orientation { get; private set; } = Quaternion.Identity;

        /// <summary>
        /// The current position [m].
        /// </summary>
        public DVector3 WorldPosition;

        /// <summary>
        /// The current speed [m/s].
        /// </summary>
        public double Speed;

        /// <summary>
        /// The world flight direction or 0 if not moving.
        /// </summary>
        public DVector3 WorldFlightDirection { get; private set; }

        /// <summary>
        /// The flight direction (normalized vector) in ship orientation or 0 if not moving.
        /// </summary>
        public DVector3 FlightDirection;

        /// <summary>
        /// The desired velocity vector [m/s] in ship orientation as specified by the pilot's input.
        /// </summary>
        public double DesiredSpeed;

        /// <summary>
        /// The desired flight direction (normalized vector) in ship orientation as specified by the pilot's input.
        /// When DesiredSpeed is set to 0 this can still be used to allow brakesteering when AntiDrift is enabled.
        /// </summary>
        public DVector3 DesiredFlightDirection;

        /// <summary>
        /// Whether the pilot has currently engaged afterburner.
        /// </summary>
        public bool AfterburnerEngaged;

        /// <summary>
        /// Gets the maximum thrust [N] available along the required thrust axis.
        /// If the power along thrust axis is not independent this does not provide meaningful results.
        /// </summary>
        /// <param name="xAxis">Whether to go right (1) or left (-1).</param>
        /// <param name="yAxis">Whether to go up (1) or down (-1).</param>
        /// <param name="zAxis">Whether to go forward (1) or backward (-1).</param>
        /// <returns>
        /// The max available thrust [N] (if thrust axis were independent).
        /// </returns>
        public DVector3 GetMaxAvailableThrust(int xAxis, int yAxis, int zAxis)
        {
            DVector3 thrust;
            thrust.X = xAxis == 0 ? 0.0 : xAxis > 0 ? this.MaxThrust.X : this.MinThrust.X;
            thrust.Y = yAxis == 0 ? 0.0 : yAxis > 0 ? this.MaxThrust.Y : this.MinThrust.Y;
            thrust.Z = zAxis == 0 ? 0.0 : zAxis > 0 ? this.MaxThrust.Z : this.MinThrust.Z;
            return (this.AfterburnerEngaged ? this.AfterburnerFactor : 1) * thrust;
        }

        /// <summary>
        /// Clamps the thrust [N] to the ship's capabilities.
        /// </summary>
        /// <param name="thrust">The thrust.</param>
        /// <param name="maxedAxis">The maxed axis [0=x, 1=y, z=2]. Or -1 if no thrust axis is maxed.</param>
        /// <returns>The clamped thrust [N].</returns>
        public DVector3 ClampThrust(DVector3 thrust, out int maxedAxis)
        {
            DVector3 fac = new DVector3(1, 1, 1);

            double abFac = this.AfterburnerEngaged ? this.AfterburnerFactor : 1;

            DVector3 maxThrust = abFac * this.MaxThrust;
            DVector3 minThrust = abFac * this.MinThrust;

            if (thrust.X > maxThrust.X)
            {
                fac.X = maxThrust.X / thrust.X;
            }
            else if (thrust.X < minThrust.X)
            {
                fac.X = minThrust.X / thrust.X;
            }

            if (thrust.Y > maxThrust.Y)
            {
                fac.Y = maxThrust.Y / thrust.Y;
            }
            else if (thrust.Y < minThrust.Y)
            {
                fac.Y = minThrust.Y / thrust.Y;
            }

            if (thrust.Z > maxThrust.Z)
            {
                fac.Z = maxThrust.Z / thrust.Z;
            }
            else if (thrust.Z < minThrust.Z)
            {
                fac.Z = minThrust.Z / thrust.Z;
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
        /// Sets the world flight direction. Normalized vector.
        /// </summary>
        /// <param name="worldFlightDirection">The world flight direction.</param>
        public void SetWorldFlightDirection(DVector3 worldFlightDirection)
        {
            this.WorldFlightDirection = worldFlightDirection;
            this.UpdateOrientation(this.Orientation);
        }

        /// <summary>
        /// Updates the ship orientation.
        /// </summary>
        /// <param name="orientation">The orientation.</param>
        public void UpdateOrientation(Quaternion orientation)
        {
            this.Orientation = orientation;

            // update flight direction relative to ship orientation
            this.FlightDirection = this.Orientation.Inverted().Transform(this.WorldFlightDirection);
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

            // compute velocity change
            DVector3 velocityChange = 0.5 * acc * time * time + velocityVector * time;

            // transform to world
            DVector3 worldVelocityChange = this.Orientation.Transform(velocityChange);

            // update current position
            this.WorldPosition += worldVelocityChange;

            // update current velocity vector.
            velocityVector += acc * time;
            this.Speed = velocityVector.Normalize();

            // just to make sure spaceship comes to full stop if within small tolerance
            if (this.Speed < 0.0001)
            {
                this.Speed = 0.0;
                this.FlightDirection = DVector3.Zero;
            }
            else
            {
                this.FlightDirection = velocityVector;
            }

            // also update WorldVelocityVector
            this.WorldFlightDirection = this.Orientation.Transform(this.FlightDirection);
        }
    }
}
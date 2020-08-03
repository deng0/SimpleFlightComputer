using System;

namespace SimpleFlightComputer
{
    public struct Quaternion
    {
        /// <summary>
        /// The identity <see cref="Quaternion"/> (0, 0, 0, 1).
        /// </summary>
        public static readonly Quaternion Identity = new Quaternion(0.0f, 0.0f, 0.0f, 1.0f);

        /// <summary>
        /// The X component of the quaternion.
        /// </summary>
        public float X;

        /// <summary>
        /// The Y component of the quaternion.
        /// </summary>
        public float Y;

        /// <summary>
        /// The Z component of the quaternion.
        /// </summary>
        public float Z;

        /// <summary>
        /// The W component of the quaternion.
        /// </summary>
        public float W;

        /// <summary>
        /// Calculates the length of the quaternion.
        /// </summary>
        /// <returns>The length of the quaternion.</returns>
        /// <remarks>
        /// <see cref="Quaternion.LengthSquared"/> may be preferred when only the relative length is needed
        /// and speed is of the essence.
        /// </remarks>
        public float Length()
        {
            return (float)Math.Sqrt((this.X * this.X) + (this.Y * this.Y) + (this.Z * this.Z) + (this.W * this.W));
        }

        /// <summary>
        /// Calculates the squared length of the quaternion.
        /// </summary>
        /// <returns>The squared length of the quaternion.</returns>
        /// <remarks>
        /// This method may be preferred to <see cref="Quaternion.Length"/> when only a relative length is needed
        /// and speed is of the essence.
        /// </remarks>
        public float LengthSquared()
        {
            return (this.X * this.X) + (this.Y * this.Y) + (this.Z * this.Z) + (this.W * this.W);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Quaternion"/> struct.
        /// </summary>
        /// <param name="x">Initial value for the X component of the quaternion.</param>
        /// <param name="y">Initial value for the Y component of the quaternion.</param>
        /// <param name="z">Initial value for the Z component of the quaternion.</param>
        /// <param name="w">Initial value for the W component of the quaternion.</param>
        public Quaternion(float x, float y, float z, float w)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.W = w;
        }

        /// <summary>
        /// Conjugates and renormalizes the quaternion.
        /// </summary>
        public Quaternion Inverted()
        {
            float lengthSq = this.LengthSquared();
            if (lengthSq > 1e-6f)
            {
                lengthSq = 1.0f / lengthSq;
                return new Quaternion(-this.X * lengthSq, -this.Y * lengthSq, -this.Z * lengthSq, this.W * lengthSq);
            }
            return this;
        }

        public DVector3 Transform(in DVector3 vector)
        {
            double x = this.X + this.X;
            double y = this.Y + this.Y;
            double z = this.Z + this.Z;
            double wx = this.W * x;
            double wy = this.W * y;
            double wz = this.W * z;
            double xx = this.X * x;
            double xy = this.X * y;
            double xz = this.X * z;
            double yy = this.Y * y;
            double yz = this.Y * z;
            double zz = this.Z * z;

            return new DVector3(
                ((vector.X * ((1.0 - yy) - zz)) + (vector.Y * (xy - wz))) + (vector.Z * (xz + wy)),
                ((vector.X * (xy + wz)) + (vector.Y * ((1.0 - xx) - zz))) + (vector.Z * (yz - wx)),
                ((vector.X * (xz - wy)) + (vector.Y * (yz + wx))) + (vector.Z * ((1.0 - xx) - yy)));
        }
    }
}
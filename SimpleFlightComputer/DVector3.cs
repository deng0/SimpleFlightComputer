using System;

namespace SimpleFlightComputer
{
    /// <summary>
    /// A double precision 3D Vector.
    /// </summary>
    public struct DVector3 : IEquatable<DVector3>
    {
        /// <summary>
        /// Gets the zero vector.
        /// </summary>
        public static readonly DVector3 Zero = new DVector3(0, 0, 0);

        /// <summary>
        /// The X coordinate.
        /// </summary>
        public double X;

        /// <summary>
        /// The Y coordinate.
        /// </summary>
        public double Y;

        /// <summary>
        /// The Z coordinate.
        /// </summary>
        public double Z;

        /// <summary>
        /// Initializes a new instance of the <see cref="DVector3" /> struct.
        /// </summary>
        /// <param name="_X">The X coordinate.</param>
        /// <param name="_Y">The Y coordinate.</param>
        /// <param name="_Z">The Z coordinate.</param>
        public DVector3(double _X, double _Y, double _Z)
        {
            this.X = _X;
            this.Y = _Y;
            this.Z = _Z;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = this.X.GetHashCode();
                hashCode = (hashCode * 397) ^ this.Y.GetHashCode();
                hashCode = (hashCode * 397) ^ this.Z.GetHashCode();
                return hashCode;
            }
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="T:System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            return obj is DVector3 other && this == other;
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        ///   <see langword="true" /> if the current object is equal to the <paramref name="other" /> parameter; otherwise, <see langword="false" />.
        /// </returns>
        public bool Equals(DVector3 other)
        {
            return this == other;
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            return $"DVector3 [{this.X}, {this.Y}, {this.Z}]";
        }

        /// <summary>
        /// Implements the + operator.
        /// </summary>
        /// <param name="left">The left argument.</param>
        /// <param name="right">The right argument.</param>
        /// <returns>The result of the operator.</returns>
        public static DVector3 operator +(in DVector3 left, in DVector3 right)
        {
            return new DVector3(left.X + right.X, left.Y + right.Y, left.Z + right.Z);
        }

        /// <summary>
        /// Negates the coordinates.
        /// </summary>
        /// <param name="vec">The vector.</param>
        /// <returns>The negated vector.</returns>
        public static DVector3 operator -(in DVector3 vec)
        {
            return new DVector3(-vec.X, -vec.Y, -vec.Z);
        }

        /// <summary>
        /// Implements the - operator.
        /// </summary>
        /// <param name="left">The left argument.</param>
        /// <param name="right">The right argument.</param>
        /// <returns>The result of the operator.</returns>
        public static DVector3 operator -(in DVector3 left, in DVector3 right)
        {
            return new DVector3(left.X - right.X, left.Y - right.Y, left.Z - right.Z);
        }

        /// <summary>
        /// Implements the * operator.
        /// </summary>
        /// <param name="left">The left argument.</param>
        /// <param name="right">The right argument.</param>
        /// <returns>The result of the operator.</returns>
        public static DVector3 operator *(in DVector3 left, double right)
        {
            return new DVector3(left.X * right, left.Y * right, left.Z * right);
        }

        /// <summary>
        /// Scales a vector by the given value.
        /// </summary>
        /// <param name="value">The vector to scale.</param>
        /// <param name="scale">The amount by which to scale the vector.</param>
        /// <returns>The scaled vector.</returns>
        public static DVector3 operator /(in DVector3 value, in double scale)
        {
            return new DVector3(value.X / scale, value.Y / scale, value.Z / scale);
        }

        /// <summary>
        /// Scales a vector by the given value.
        /// </summary>
        /// <param name="value">The vector to scale.</param>
        /// <param name="scale">The amount by which to scale the vector.</param>
        /// <returns>The scaled vector.</returns>
        public static DVector3 operator /(in DVector3 value, in DVector3 scale)
        {
            return new DVector3(value.X / scale.X, value.Y / scale.Y, value.Z / scale.Z);
        }

        /// <summary>
        /// Implements the * operator.
        /// </summary>
        /// <param name="left">The left argument.</param>
        /// <param name="right">The right argument.</param>
        /// <returns>The result of the operator.</returns>
        public static DVector3 operator *(double right, in DVector3 left)
        {
            return new DVector3(left.X * right, left.Y * right, left.Z * right);
        }

        /// <summary>
        /// Implements the == operator.
        /// </summary>
        /// <param name="left">The left argument.</param>
        /// <param name="right">The right argument.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(in DVector3 left, in DVector3 right)
        {
            return left.X == right.X && left.Y == right.Y && left.Z == right.Z;
        }

        /// <summary>
        /// Implements the != operator.
        /// </summary>
        /// <param name="left">The left argument.</param>
        /// <param name="right">The right argument.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(in DVector3 left, in DVector3 right)
        {
            return left.X != right.X || left.Y != right.Y || left.Z != right.Z;
        }

        /// <summary>
        /// Returns a new vector with X maximum of X and Y maximum of Y and Z maximum of Z.
        /// </summary>
        /// <param name="a">A vector.</param>
        /// <param name="b">Another vector.</param>
        /// <returns>A new vector with X maximum of X and Y maximum of Y and Z maximum of Z.</returns>
        public static DVector3 Max(in DVector3 a, in DVector3 b)
        {
            return new DVector3(Math.Max(a.X, b.X), Math.Max(a.Y, b.Y), Math.Max(a.Z, b.Z));
        }

        /// <summary>
        /// Returns a new vector with X minimum of X and Y minimum of Y and Z minimum of Z.
        /// </summary>
        /// <param name="a">A vector.</param>
        /// <param name="b">Another vector.</param>
        /// <returns>A new vector with X minimum of X and Y minimum of Y and Z minimum of Z.</returns>
        public static DVector3 Min(in DVector3 a, in DVector3 b)
        {
            return new DVector3(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y), Math.Min(a.Z, b.Z));
        }

        /// <summary>
        /// Scales a vector by the the values of another vector.
        /// </summary>
        /// <param name="a">The vector to scale.</param>
        /// <param name="b">The vector to use as scale.</param>
        /// <returns>The scaled vector.</returns>
        public static DVector3 Scale(in DVector3 a, in DVector3 b)
        {
            return new DVector3(a.X * b.X, a.Y * b.Y, a.Z * b.Z);
        }

        /// <summary>
        /// Scales a vector by the inverse of the values of another vector.
        /// </summary>
        /// <param name="a">The vector to scale.</param>
        /// <param name="b">The vector to use as inverse scale.</param>
        /// <returns>The scaled vector.</returns>
        public static DVector3 InvScale(in DVector3 a, in DVector3 b)
        {
            return new DVector3(a.X / b.X, a.Y / b.Y, a.Z / b.Z);
        }

        /// <summary>
        /// Computes the cross product of two vectors.
        /// </summary>
        /// <param name="a">Vector a.</param>
        /// <param name="b">Vector b.</param>
        /// <returns>The cross product.</returns>
        public static DVector3 Cross(in DVector3 a, in DVector3 b)
        {
            return new DVector3(a.Y * b.Z - a.Z * b.Y, a.Z * b.X - a.X * b.Z, a.X * b.Y - a.Y * b.X);
        }

        /// <summary>
        /// Computes the dot product of two vectors.
        /// </summary>
        /// <param name="a">Vector a.</param>
        /// <param name="b">Vector b.</param>
        /// <returns>The dot product.</returns>
        public static double Dot(in DVector3 a, in DVector3 b)
        {
            return a.X * b.X + a.Y * b.Y + a.Z * b.Z;
        }
    }

    /// <summary>
    /// DVector3 Extensions.
    /// </summary>
    public static class DVector3Extensions
    {
        /// <summary>
        /// Returns the length of the vector.
        /// </summary>
        /// <returns>The length of the vector.</returns>
        public static double Length(this in DVector3 _this)
        {
            return Math.Sqrt(_this.X * _this.X + _this.Y * _this.Y + _this.Z * _this.Z);
        }

        /// <summary>
        /// Return the squared length of the vector.
        /// </summary>
        /// <returns>The squared length of the vector.</returns>
        public static double LengthSq(this in DVector3 _this)
        {
            return _this.X * _this.X + _this.Y * _this.Y + _this.Z * _this.Z;
        }

        /// <summary>
        /// Normalizes the vector and returns the old length.
        /// </summary>
        public static double Normalize(this ref DVector3 _this)
        {
            double len = _this.Length();
            _this.X /= len;
            _this.Y /= len;
            _this.Z /= len;
            return len;
        }
    }
}
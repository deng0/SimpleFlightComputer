using System;

namespace SimpleFlightComputer
{
    public static class Helpers
    {
        public static bool IsZero(double val)
        {
            return Math.Abs(val) < 1E-14;
        }
    }
}
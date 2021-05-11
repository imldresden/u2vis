using UnityEngine;

namespace u2vis.Utilities
{
    public static class CoordinateHelper
    {
        public static void ToPolarCoordinates(float x, float y, out float rho, out float theta)
        {
            rho = Mathf.Sqrt(x * x + y * y);
            theta = Mathf.Atan2(y, x);
        }

        public static void ToCartesianCoordinates(float rho, float theta, out float x, out float y)
        {
            x = rho * Mathf.Cos(theta);
            y = rho * Mathf.Sin(theta);
        }
    }
}

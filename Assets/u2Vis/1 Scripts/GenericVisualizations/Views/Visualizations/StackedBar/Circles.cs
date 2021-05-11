using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace u2vis
{
    public static class Circles
    {
        public static List<Vector3> CreatePartMesh(float startAngle, float endAngle, float radius, int segments)
        {
            List<Vector3> partMesh = new List<Vector3>();
            int segmentsOfPart = (int)(((endAngle - startAngle) / (Mathf.PI * 2)) * segments);
            for (int segment = 1; segment <= segmentsOfPart + 1; segment++)
            {
                float angle = startAngle + segment * ((endAngle - startAngle) / segmentsOfPart);
                partMesh.Add(SphericalToCartesian(radius, angle, 0));
            }
            return partMesh;
        }

        public static List<int> CreateRound(int startIndex, int length, int correction)
        {
            List<int> round = new List<int>();
            for (int i = 0; i < length; i++)
            {
                round.Add(i + startIndex + length + 1 + correction);
                round.Add(i + startIndex);
                round.Add(i + startIndex + 1);
                round.Add(i + startIndex + length + 1 + correction);
                round.Add(i + startIndex + 1);
                round.Add(i + startIndex + length + 2 + correction);
            }
            return round;
        }
        
        public static Vector3 SphericalToCartesian(float radius, float polar, float elevation)
        {
            Vector3 outCart = new Vector3();
            float a = radius * Mathf.Cos(elevation);
            outCart.x = a * Mathf.Cos(polar);
            outCart.z = radius * Mathf.Sin(elevation);
            outCart.y = a * Mathf.Sin(polar);
            return outCart;
        }
    }
}

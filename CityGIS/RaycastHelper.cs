using System;
using System.Windows.Media.Media3D;

namespace CityGIS
{
    public static class RaycastHelper
    {
        /// <summary>
        /// Tests whether a ray intersects an axis-aligned bounding box (AABB).
        /// Returns true if the ray hits the box, with the hit distance in <paramref name="tHit"/>.
        /// </summary>
        public static bool RayIntersectsBox(Point3D origin, Vector3D dir, Point3D min, Point3D max, out double tHit)
        {
            tHit = 0;
            double tmin = double.NegativeInfinity;
            double tmax = double.PositiveInfinity;

            if (Math.Abs(dir.X) < 1e-12)
            {
                if (origin.X < min.X || origin.X > max.X) return false;
            }
            else
            {
                double t1 = (min.X - origin.X) / dir.X;
                double t2 = (max.X - origin.X) / dir.X;
                if (t1 > t2) { (t1, t2) = (t2, t1); }
                tmin = Math.Max(tmin, t1);
                tmax = Math.Min(tmax, t2);
                if (tmin > tmax) return false;
            }

            if (Math.Abs(dir.Y) < 1e-12)
            {
                if (origin.Y < min.Y || origin.Y > max.Y) return false;
            }
            else
            {
                double t1 = (min.Y - origin.Y) / dir.Y;
                double t2 = (max.Y - origin.Y) / dir.Y;
                if (t1 > t2) { (t1, t2) = (t2, t1); }
                tmin = Math.Max(tmin, t1);
                tmax = Math.Min(tmax, t2);
                if (tmin > tmax) return false;
            }

            if (Math.Abs(dir.Z) < 1e-12)
            {
                if (origin.Z < min.Z || origin.Z > max.Z) return false;
            }
            else
            {
                double t1 = (min.Z - origin.Z) / dir.Z;
                double t2 = (max.Z - origin.Z) / dir.Z;
                if (t1 > t2) { (t1, t2) = (t2, t1); }
                tmin = Math.Max(tmin, t1);
                tmax = Math.Min(tmax, t2);
                if (tmin > tmax) return false;
            }

            tHit = tmin >= 0 ? tmin : tmax;
            return tHit >= 0;
        }
    }
}

using System.Windows.Media.Media3D;
using Xunit;

namespace CityGIS.Tests
{
    public class RaycastHelperTests
    {
        // A 10×10×10 box centred at the origin, base at z=0
        private static readonly Point3D BoxMin = new(-5, -5, 0);
        private static readonly Point3D BoxMax = new(5, 5, 10);

        [Fact]
        public void RayFromAbove_HitsBox()
        {
            var origin = new Point3D(0, 0, 20);
            var dir = new Vector3D(0, 0, -1);

            Assert.True(RaycastHelper.RayIntersectsBox(origin, dir, BoxMin, BoxMax, out double t));
            Assert.Equal(10, t, 5);
        }

        [Fact]
        public void RayMissesBox_ReturnsFalse()
        {
            var origin = new Point3D(20, 20, 20);
            var dir = new Vector3D(0, 0, -1);

            Assert.False(RaycastHelper.RayIntersectsBox(origin, dir, BoxMin, BoxMax, out _));
        }

        [Fact]
        public void RayPointingAway_ReturnsFalse()
        {
            var origin = new Point3D(0, 0, 20);
            var dir = new Vector3D(0, 0, 1);

            Assert.False(RaycastHelper.RayIntersectsBox(origin, dir, BoxMin, BoxMax, out _));
        }

        [Fact]
        public void RayFromInsideBox_ReturnsTrue()
        {
            var origin = new Point3D(0, 0, 5);
            var dir = new Vector3D(0, 0, 1);

            Assert.True(RaycastHelper.RayIntersectsBox(origin, dir, BoxMin, BoxMax, out double t));
            Assert.True(t >= 0);
        }

        [Fact]
        public void RayParallelToFace_OutsideBox_ReturnsFalse()
        {
            var origin = new Point3D(10, 0, 5);
            var dir = new Vector3D(0, 1, 0);

            Assert.False(RaycastHelper.RayIntersectsBox(origin, dir, BoxMin, BoxMax, out _));
        }

        [Fact]
        public void RayFromSide_HitsBox()
        {
            var origin = new Point3D(20, 0, 5);
            var dir = new Vector3D(-1, 0, 0);

            Assert.True(RaycastHelper.RayIntersectsBox(origin, dir, BoxMin, BoxMax, out double t));
            Assert.Equal(15, t, 5);
        }

        [Fact]
        public void RayDiagonal_HitsBox()
        {
            var origin = new Point3D(20, 20, 20);
            var dir = new Vector3D(-1, -1, -1);
            dir.Normalize();

            Assert.True(RaycastHelper.RayIntersectsBox(origin, dir, BoxMin, BoxMax, out _));
        }

        [Fact]
        public void RayDiagonal_AwayFromBox_ReturnsFalse()
        {
            var origin = new Point3D(20, 20, 20);
            var dir = new Vector3D(1, 1, 1);
            dir.Normalize();

            Assert.False(RaycastHelper.RayIntersectsBox(origin, dir, BoxMin, BoxMax, out _));
        }
    }
}

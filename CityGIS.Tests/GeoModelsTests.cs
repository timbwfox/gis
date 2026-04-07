using Xunit;
using CityGIS.Models;

namespace CityGIS.Tests
{
    public class GeoModelsTests
    {
        [Fact]
        public void Vector3_DefaultConstructor_AllZeros()
        {
            var v = new Vector3();
            Assert.Equal(0, v.X);
            Assert.Equal(0, v.Y);
            Assert.Equal(0, v.Z);
        }

        [Fact]
        public void Vector3_ParameterizedConstructor_SetsValues()
        {
            var v = new Vector3(1.5, 2.5, 3.5);
            Assert.Equal(1.5, v.X);
            Assert.Equal(2.5, v.Y);
            Assert.Equal(3.5, v.Z);
        }

        [Fact]
        public void Color_DefaultConstructor_DefaultGray()
        {
            var c = new Color();
            Assert.Equal(200, c.R);
            Assert.Equal(200, c.G);
            Assert.Equal(200, c.B);
        }

        [Fact]
        public void Color_ClampsNegativeValues_ToZero()
        {
            var c = new Color(-10, -20, -30);
            Assert.Equal(0, c.R);
            Assert.Equal(0, c.G);
            Assert.Equal(0, c.B);
        }

        [Fact]
        public void Color_ClampsHighValues_To255()
        {
            var c = new Color(300, 400, 500);
            Assert.Equal(255, c.R);
            Assert.Equal(255, c.G);
            Assert.Equal(255, c.B);
        }

        [Fact]
        public void Color_ValidValues_Preserved()
        {
            var c = new Color(100, 150, 200);
            Assert.Equal(100, c.R);
            Assert.Equal(150, c.G);
            Assert.Equal(200, c.B);
        }

        [Fact]
        public void Color_BoundaryValues_Preserved()
        {
            var c = new Color(0, 255, 128);
            Assert.Equal(0, c.R);
            Assert.Equal(255, c.G);
            Assert.Equal(128, c.B);
        }

        [Fact]
        public void Building_DefaultProperties_AreNull()
        {
            var b = new Building();
            Assert.Null(b.Id);
            Assert.Null(b.Name);
            Assert.Null(b.Type);
            Assert.Null(b.Position);
            Assert.Null(b.Size);
            Assert.Null(b.Color);
        }

        [Fact]
        public void Road_DefaultProperties_InitializesPointsList()
        {
            var r = new Road();
            Assert.NotNull(r.Points);
            Assert.Empty(r.Points);
        }

        [Fact]
        public void CityData_DefaultProperties_InitializesCollections()
        {
            var cd = new CityData();
            Assert.NotNull(cd.Buildings);
            Assert.NotNull(cd.Roads);
            Assert.Empty(cd.Buildings);
            Assert.Empty(cd.Roads);
        }
    }
}

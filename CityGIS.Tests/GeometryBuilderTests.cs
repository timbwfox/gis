using Xunit;
using CityGIS.Models;

namespace CityGIS.Tests
{
    public class GeometryBuilderTests
    {
        private static Building CreateTestBuilding(
            double x = 0, double y = 0, double z = 0,
            double w = 10, double d = 10, double h = 10)
        {
            return new Building
            {
                Id = "test",
                Name = "Test",
                Type = "test",
                Position = new Vector3(x, y, z),
                Size = new Vector3(w, d, h),
                Color = new Color(200, 200, 200)
            };
        }

        [Fact]
        public void CreateBuildingBox_ProducesCorrectVertexCount()
        {
            var building = CreateTestBuilding();
            var mesh = GeometryBuilder.CreateBuildingBox(building);

            // 6 faces × 2 triangles × 3 vertices = 36
            Assert.Equal(36, mesh.Positions.Count);
            Assert.Equal(36, mesh.TriangleIndices.Count);
        }

        [Fact]
        public void CreateBuildingBox_VerticesSpanCorrectBounds()
        {
            var building = CreateTestBuilding(x: 0, y: 0, z: 0, w: 20, d: 10, h: 30);
            var mesh = GeometryBuilder.CreateBuildingBox(building);

            double minX = double.MaxValue, maxX = double.MinValue;
            double minY = double.MaxValue, maxY = double.MinValue;
            double minZ = double.MaxValue, maxZ = double.MinValue;

            foreach (var p in mesh.Positions)
            {
                if (p.X < minX) minX = p.X;
                if (p.X > maxX) maxX = p.X;
                if (p.Y < minY) minY = p.Y;
                if (p.Y > maxY) maxY = p.Y;
                if (p.Z < minZ) minZ = p.Z;
                if (p.Z > maxZ) maxZ = p.Z;
            }

            // half width = 10, half depth = 5
            Assert.Equal(-10, minX, 5);
            Assert.Equal(10, maxX, 5);
            Assert.Equal(-5, minY, 5);
            Assert.Equal(5, maxY, 5);
            Assert.Equal(0, minZ, 5);
            Assert.Equal(30, maxZ, 5);
        }

        [Fact]
        public void CreateBuildingBox_OffsetPosition_ShiftsVertices()
        {
            var building = CreateTestBuilding(x: 100, y: 50, z: 10, w: 20, d: 10, h: 30);
            var mesh = GeometryBuilder.CreateBuildingBox(building);

            double minX = double.MaxValue, maxX = double.MinValue;
            double minZ = double.MaxValue, maxZ = double.MinValue;

            foreach (var p in mesh.Positions)
            {
                if (p.X < minX) minX = p.X;
                if (p.X > maxX) maxX = p.X;
                if (p.Z < minZ) minZ = p.Z;
                if (p.Z > maxZ) maxZ = p.Z;
            }

            Assert.Equal(90, minX, 5);   // 100 - 10
            Assert.Equal(110, maxX, 5);  // 100 + 10
            Assert.Equal(10, minZ, 5);   // z base
            Assert.Equal(40, maxZ, 5);   // z + height
        }

        [Fact]
        public void CreateRoadMesh_LessThanTwoPoints_ReturnsEmptyMesh()
        {
            var road = new Road
            {
                Id = "r1", Name = "Short", Type = "test", Width = 10,
                Points = new() { new Vector3(0, 0, 0) },
                Color = new Color()
            };
            var mesh = GeometryBuilder.CreateRoadMesh(road);
            Assert.Empty(mesh.Positions);
        }

        [Fact]
        public void CreateRoadMesh_TwoPoints_ProducesQuad()
        {
            var road = new Road
            {
                Id = "r1", Name = "Test", Type = "test", Width = 10,
                Points = new() { new Vector3(0, 0, 0), new Vector3(100, 0, 0) },
                Color = new Color()
            };
            var mesh = GeometryBuilder.CreateRoadMesh(road);

            // 1 segment × 2 triangles × 3 vertices = 6
            Assert.Equal(6, mesh.Positions.Count);
            Assert.Equal(6, mesh.TriangleIndices.Count);
        }

        [Fact]
        public void CreateRoadMesh_ThreePoints_ProducesTwoQuads()
        {
            var road = new Road
            {
                Id = "r1", Name = "Test", Type = "test", Width = 10,
                Points = new()
                {
                    new Vector3(0, 0, 0),
                    new Vector3(50, 0, 0),
                    new Vector3(100, 0, 0)
                },
                Color = new Color()
            };
            var mesh = GeometryBuilder.CreateRoadMesh(road);

            // 2 segments × 2 triangles × 3 vertices = 12
            Assert.Equal(12, mesh.Positions.Count);
        }
    }
}

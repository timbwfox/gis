using System;
using System.IO;
using Newtonsoft.Json.Linq;
using Xunit;
using CityGIS.Models;

namespace CityGIS.Tests
{
    public class CityDataLoaderTests : IDisposable
    {
        private readonly string _tempDir;

        public CityDataLoaderTests()
        {
            _tempDir = Path.Combine(Path.GetTempPath(), $"CityGIS_Tests_{Guid.NewGuid()}");
            Directory.CreateDirectory(_tempDir);
        }

        public void Dispose()
        {
            if (Directory.Exists(_tempDir))
                Directory.Delete(_tempDir, true);
        }

        private string WriteTempJson(string json)
        {
            var path = Path.Combine(_tempDir, "test.json");
            File.WriteAllText(path, json);
            return path;
        }

        // -----------------------------------------------------------------
        // LoadCityData – file-level tests
        // -----------------------------------------------------------------

        [Fact]
        public void LoadCityData_MissingFile_ThrowsFileNotFoundException()
        {
            Assert.Throws<FileNotFoundException>(() =>
                CityDataLoader.LoadCityData(Path.Combine(_tempDir, "nonexistent.json")));
        }

        [Fact]
        public void LoadCityData_ParsesCityMetadata()
        {
            var path = WriteTempJson("""
            {
                "city": { "name": "TestCity", "scale": 2.5, "center": { "latitude": 10.0, "longitude": 20.0 } },
                "buildings": [],
                "roads": []
            }
            """);

            var data = CityDataLoader.LoadCityData(path);

            Assert.Equal("TestCity", data.Name);
            Assert.Equal(2.5, data.Scale);
            Assert.Equal(10.0, data.Center.Latitude);
            Assert.Equal(20.0, data.Center.Longitude);
        }

        [Fact]
        public void LoadCityData_ParsesBuildings()
        {
            var path = WriteTempJson("""
            {
                "city": { "name": "Test" },
                "buildings": [
                    {
                        "id": "b1",
                        "name": "Hall",
                        "type": "gov",
                        "position": { "x": 1.0, "y": 2.0, "z": 3.0 },
                        "size": { "width": 10.0, "depth": 20.0, "height": 30.0 },
                        "color": { "r": 100, "g": 150, "b": 200 }
                    }
                ]
            }
            """);

            var data = CityDataLoader.LoadCityData(path);

            Assert.Single(data.Buildings);
            var b = data.Buildings[0];
            Assert.Equal("b1", b.Id);
            Assert.Equal("Hall", b.Name);
            Assert.Equal("gov", b.Type);
            Assert.Equal(1.0, b.Position.X);
            Assert.Equal(2.0, b.Position.Y);
            Assert.Equal(3.0, b.Position.Z);
            Assert.Equal(10.0, b.Size.X);
            Assert.Equal(20.0, b.Size.Y);
            Assert.Equal(30.0, b.Size.Z);
            Assert.Equal(100, b.Color.R);
            Assert.Equal(150, b.Color.G);
            Assert.Equal(200, b.Color.B);
        }

        [Fact]
        public void LoadCityData_ParsesRoads()
        {
            var path = WriteTempJson("""
            {
                "city": { "name": "Test" },
                "roads": [
                    {
                        "id": "r1",
                        "name": "Main St",
                        "type": "primary",
                        "width": 15.0,
                        "points": [
                            { "x": 0.0, "y": 0.0, "z": 0.0 },
                            { "x": 10.0, "y": 0.0, "z": 0.0 }
                        ],
                        "color": { "r": 50, "g": 50, "b": 50 }
                    }
                ]
            }
            """);

            var data = CityDataLoader.LoadCityData(path);

            Assert.Single(data.Roads);
            var r = data.Roads[0];
            Assert.Equal("r1", r.Id);
            Assert.Equal("Main St", r.Name);
            Assert.Equal("primary", r.Type);
            Assert.Equal(15.0, r.Width);
            Assert.Equal(2, r.Points.Count);
            Assert.Equal(50, r.Color.R);
        }

        [Fact]
        public void LoadCityData_MissingProperties_UsesDefaults()
        {
            var path = WriteTempJson("""
            {
                "city": {},
                "buildings": [{}]
            }
            """);

            var data = CityDataLoader.LoadCityData(path);

            Assert.Equal("Unknown City", data.Name);
            Assert.Equal(1.0, data.Scale);
            Assert.Single(data.Buildings);
            Assert.Equal("unknown", data.Buildings[0].Id);
            Assert.Equal("Building", data.Buildings[0].Name);
            Assert.Equal("structure", data.Buildings[0].Type);
        }

        [Fact]
        public void LoadCityData_EmptyJson_ReturnsEmptyCollections()
        {
            var path = WriteTempJson("{}");

            var data = CityDataLoader.LoadCityData(path);

            Assert.Empty(data.Buildings);
            Assert.Empty(data.Roads);
        }

        // -----------------------------------------------------------------
        // Internal parse methods
        // -----------------------------------------------------------------

        [Fact]
        public void ParseVector3_ValidObject_ReturnsCorrectValues()
        {
            var token = JObject.Parse("""{"x": 1.5, "y": 2.5, "z": 3.5}""");
            var result = CityDataLoader.ParseVector3(token);
            Assert.Equal(1.5, result.X);
            Assert.Equal(2.5, result.Y);
            Assert.Equal(3.5, result.Z);
        }

        [Fact]
        public void ParseVector3_NullToken_ReturnsDefault()
        {
            var result = CityDataLoader.ParseVector3(null);
            Assert.Equal(0, result.X);
            Assert.Equal(0, result.Y);
            Assert.Equal(0, result.Z);
        }

        [Fact]
        public void ParseSize_UsesWidthDepthHeight()
        {
            var token = JObject.Parse("""{"width": 10.0, "depth": 20.0, "height": 30.0}""");
            var result = CityDataLoader.ParseSize(token);
            Assert.Equal(10.0, result.X);
            Assert.Equal(20.0, result.Y);
            Assert.Equal(30.0, result.Z);
        }

        [Fact]
        public void ParseSize_FallsBackToXYZ()
        {
            var token = JObject.Parse("""{"x": 5.0, "y": 6.0, "z": 7.0}""");
            var result = CityDataLoader.ParseSize(token);
            Assert.Equal(5.0, result.X);
            Assert.Equal(6.0, result.Y);
            Assert.Equal(7.0, result.Z);
        }

        [Fact]
        public void ParseColor_ValidObject_ReturnsCorrectValues()
        {
            var token = JObject.Parse("""{"r": 100, "g": 150, "b": 200}""");
            var result = CityDataLoader.ParseColor(token);
            Assert.Equal(100, result.R);
            Assert.Equal(150, result.G);
            Assert.Equal(200, result.B);
        }

        [Fact]
        public void ParseColor_NullToken_ReturnsDefault()
        {
            var result = CityDataLoader.ParseColor(null);
            Assert.Equal(200, result.R);
            Assert.Equal(200, result.G);
            Assert.Equal(200, result.B);
        }
    }
}

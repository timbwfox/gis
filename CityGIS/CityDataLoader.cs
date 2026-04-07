using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;
using CityGIS.Models;

namespace CityGIS
{
    public class CityDataLoader
    {
        public static CityData LoadCityData(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"City data file not found: {filePath}");
            }

            string jsonContent = File.ReadAllText(filePath);
            JObject jObject = JObject.Parse(jsonContent);

            CityData cityData = new CityData();

            // Parse city information
            if (jObject["city"] is JObject cityInfo)
            {
                cityData.Name = cityInfo["name"]?.Value<string>() ?? "Unknown City";
                cityData.Scale = cityInfo["scale"]?.Value<double>() ?? 1.0;

                if (cityInfo["center"] is JObject center)
                {
                    cityData.Center = new CityCenter
                    {
                        Latitude = center["latitude"]?.Value<double>() ?? 0,
                        Longitude = center["longitude"]?.Value<double>() ?? 0
                    };
                }
            }

            // Parse buildings
            if (jObject["buildings"] is JArray buildingsArray)
            {
                foreach (JObject buildingObj in buildingsArray)
                {
                    Building building = new Building
                    {
                        Id = buildingObj["id"]?.Value<string>() ?? "unknown",
                        Name = buildingObj["name"]?.Value<string>() ?? "Building",
                        Type = buildingObj["type"]?.Value<string>() ?? "structure",
                        Position = ParseVector3(buildingObj["position"]),
                        Size = ParseSize(buildingObj["size"]),
                        Color = ParseColor(buildingObj["color"])
                    };
                    cityData.Buildings.Add(building);
                }
            }

            // Parse roads
            if (jObject["roads"] is JArray roadsArray)
            {
                foreach (JObject roadObj in roadsArray)
                {
                    Road road = new Road
                    {
                        Id = roadObj["id"]?.Value<string>() ?? "unknown",
                        Name = roadObj["name"]?.Value<string>() ?? "Road",
                        Type = roadObj["type"]?.Value<string>() ?? "street",
                        Width = roadObj["width"]?.Value<double>() ?? 10.0,
                        Color = ParseColor(roadObj["color"])
                    };

                    if (roadObj["points"] is JArray pointsArray)
                    {
                        foreach (JObject pointObj in pointsArray)
                        {
                            road.Points.Add(ParseVector3(pointObj));
                        }
                    }

                    cityData.Roads.Add(road);
                }
            }

            return cityData;
        }

        internal static Vector3 ParseVector3(JToken token)
        {
            if (token is JObject obj)
            {
                return new Vector3(
                    obj["x"]?.Value<double>() ?? 0,
                    obj["y"]?.Value<double>() ?? 0,
                    obj["z"]?.Value<double>() ?? 0
                );
            }
            return new Vector3();
        }

        internal static Vector3 ParseSize(JToken token)
        {
            if (token is JObject obj)
            {
                return new Vector3(
                    obj["width"]?.Value<double>() ?? obj["x"]?.Value<double>() ?? 0,
                    obj["depth"]?.Value<double>() ?? obj["y"]?.Value<double>() ?? 0,
                    obj["height"]?.Value<double>() ?? obj["z"]?.Value<double>() ?? 0
                );
            }
            return new Vector3();
        }

        internal static Color ParseColor(JToken token)
        {
            if (token is JObject obj)
            {
                return new Color(
                    obj["r"]?.Value<int>() ?? 200,
                    obj["g"]?.Value<int>() ?? 200,
                    obj["b"]?.Value<int>() ?? 200
                );
            }
            return new Color();
        }
    }
}

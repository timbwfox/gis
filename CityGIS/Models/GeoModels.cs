using System;
using System.Collections.Generic;

namespace CityGIS.Models
{
    public class Vector3
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public Vector3(double x = 0, double y = 0, double z = 0)
        {
            X = x;
            Y = y;
            Z = z;
        }
    }

    public class Color
    {
        public int R { get; set; }
        public int G { get; set; }
        public int B { get; set; }

        public Color(int r = 200, int g = 200, int b = 200)
        {
            R = Math.Max(0, Math.Min(255, r));
            G = Math.Max(0, Math.Min(255, g));
            B = Math.Max(0, Math.Min(255, b));
        }

        public System.Windows.Media.Color ToMediaColor()
        {
            return System.Windows.Media.Color.FromRgb((byte)R, (byte)G, (byte)B);
        }
    }

    public class Building
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 Size { get; set; }
        public Color Color { get; set; }
    }

    public class Road
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public double Width { get; set; }
        public List<Vector3> Points { get; set; } = new List<Vector3>();
        public Color Color { get; set; }
    }

    public class CityCenter
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    public class CityData
    {
        public string Name { get; set; }
        public CityCenter Center { get; set; }
        public double Scale { get; set; }
        public List<Building> Buildings { get; set; } = new List<Building>();
        public List<Road> Roads { get; set; } = new List<Road>();
    }
}

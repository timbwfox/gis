using System;
using System.Collections.Generic;
using System.Windows.Media.Media3D;
using CityGIS.Models;

namespace CityGIS
{
    public class GeometryBuilder
    {
        /// <summary>
        /// Creates a 3D box mesh for a building
        /// </summary>
        public static MeshGeometry3D CreateBuildingBox(Building building)
        {
            var mesh = new MeshGeometry3D();

            double x = building.Position.X;
            double y = building.Position.Y;
            double z = building.Position.Z;

            double w = building.Size.X / 2; // half width
            double d = building.Size.Y / 2; // half depth
            double h = building.Size.Z;      // height

            // Add box vertices and faces
            var p0 = new Point3D(x - w, y - d, z);
            var p1 = new Point3D(x + w, y - d, z);
            var p2 = new Point3D(x + w, y + d, z);
            var p3 = new Point3D(x - w, y + d, z);

            var p4 = new Point3D(x - w, y - d, z + h);
            var p5 = new Point3D(x + w, y - d, z + h);
            var p6 = new Point3D(x + w, y + d, z + h);
            var p7 = new Point3D(x - w, y + d, z + h);

            // Bottom
            AddTriangle(mesh, p0, p1, p2);
            AddTriangle(mesh, p0, p2, p3);

            // Top
            AddTriangle(mesh, p4, p6, p5);
            AddTriangle(mesh, p4, p7, p6);

            // Front
            AddTriangle(mesh, p0, p5, p1);
            AddTriangle(mesh, p0, p4, p5);

            // Back
            AddTriangle(mesh, p2, p7, p6);
            AddTriangle(mesh, p2, p3, p7);

            // Left
            AddTriangle(mesh, p0, p7, p4);
            AddTriangle(mesh, p0, p3, p7);

            // Right
            AddTriangle(mesh, p1, p5, p6);
            AddTriangle(mesh, p1, p6, p2);

            return mesh;
        }

        /// <summary>
        /// Creates a 3D model object for a building with material
        /// </summary>
        public static ModelUIElement3D CreateBuildingModel(Building building)
        {
            var mesh = CreateBuildingBox(building);
            var color = building.Color.ToMediaColor();

            var material = new DiffuseMaterial(
                new System.Windows.Media.SolidColorBrush(color));

            var model = new GeometryModel3D(mesh, material);

            var modelUIElement = new ModelUIElement3D { Model = model };
            return modelUIElement;
        }

        /// <summary>
        /// Creates a 3D mesh for a road (extruded line)
        /// </summary>
        public static MeshGeometry3D CreateRoadMesh(Road road)
        {
            var mesh = new MeshGeometry3D();

            if (road.Points.Count < 2)
                return mesh;

            double halfWidth = road.Width / 2;

            // Create tube along the road points
            for (int i = 0; i < road.Points.Count - 1; i++)
            {
                var p1 = road.Points[i];
                var p2 = road.Points[i + 1];

                // Calculate perpendicular direction
                double dx = p2.X - p1.X;
                double dy = p2.Y - p1.Y;
                double dist = Math.Sqrt(dx * dx + dy * dy);

                if (dist < 0.001)
                    continue;

                double perpX = -dy / dist;
                double perpY = dx / dist;

                // Create quad for this segment
                var corner1 = new Point3D(
                    p1.X + perpX * halfWidth, p1.Y + perpY * halfWidth, p1.Z);
                var corner2 = new Point3D(
                    p1.X - perpX * halfWidth, p1.Y - perpY * halfWidth, p1.Z);
                var corner3 = new Point3D(
                    p2.X + perpX * halfWidth, p2.Y + perpY * halfWidth, p2.Z);
                var corner4 = new Point3D(
                    p2.X - perpX * halfWidth, p2.Y - perpY * halfWidth, p2.Z);

                AddTriangle(mesh, corner1, corner2, corner4);
                AddTriangle(mesh, corner1, corner4, corner3);
            }

            return mesh;
        }

        /// <summary>
        /// Creates a 3D model object for a road
        /// </summary>
        public static ModelUIElement3D CreateRoadModel(Road road)
        {
            var mesh = CreateRoadMesh(road);
            var color = road.Color.ToMediaColor();

            var material = new DiffuseMaterial(
                new System.Windows.Media.SolidColorBrush(color));

            var model = new GeometryModel3D(mesh, material);
            var modelUIElement = new ModelUIElement3D { Model = model };

            return modelUIElement;
        }

        private static void AddTriangle(MeshGeometry3D mesh, Point3D p0, Point3D p1, Point3D p2)
        {
            int index = mesh.Positions.Count;
            mesh.Positions.Add(p0);
            mesh.Positions.Add(p1);
            mesh.Positions.Add(p2);
            mesh.TriangleIndices.Add(index);
            mesh.TriangleIndices.Add(index + 1);
            mesh.TriangleIndices.Add(index + 2);
        }
    }
}

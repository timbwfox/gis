using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf;
using CityGIS.Models;

namespace CityGIS
{
    public partial class MainWindow : Window
    {
        private CityData _cityData;
        private ModelVisual3D _roadsGroup;
        private ModelVisual3D _labelsGroup;
        private Dictionary<GeometryModel3D, Building> _buildingMap = new();
        private GeometryModel3D _selectedGeo;

        public MainWindow()
        {
            InitializeComponent();
            CityViewport.AddHandler(
                UIElement.MouseLeftButtonDownEvent,
                new MouseButtonEventHandler(Viewport_MouseLeftButtonDown),
                true);
            Loaded += (s, e) => LoadCityData();
        }

        private void LoadCityData()
        {
            try
            {
                // Find the city_data.json file
                string appDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string dataPath = Path.Combine(appDirectory, "Data", "city_data.json");

                StatusBlock.Text = $"Loading city data from: {dataPath}";

                if (!File.Exists(dataPath))
                {
                    // Try relative path
                    dataPath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "city_data.json");
                }

                _cityData = CityDataLoader.LoadCityData(dataPath);
                
                // Update UI
                CityNameBlock.Text = $"City: {_cityData.Name}";
                StatsBlock.Text = $"Buildings: {_cityData.Buildings.Count} | Roads: {_cityData.Roads.Count}";

                // Render the city
                RenderCity();
                
                StatusBlock.Text = "City loaded successfully. Right-click to rotate, middle-click to pan, scroll to zoom.";
            }
            catch (Exception ex)
            {
                StatusBlock.Text = $"Error loading city data: {ex.Message}";
                MessageBox.Show($"Error loading city data:\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RenderCity()
        {
            // Clear existing models
            CityViewport.Children.Clear();
            CityViewport.Children.Add(new DefaultLights());
            _buildingMap.Clear();
            _selectedGeo = null;

            // Roads group
            _roadsGroup = new ModelVisual3D();
            var roadsModel = new Model3DGroup();

            foreach (var road in _cityData.Roads)
            {
                try
                {
                    var roadElement = GeometryBuilder.CreateRoadModel(road);
                    roadsModel.Children.Add(roadElement.Model);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error rendering road {road.Name}: {ex.Message}", "Render Error");
                }
            }

            _roadsGroup.Content = roadsModel;
            CityViewport.Children.Add(_roadsGroup);

            // Render buildings individually so each can be clicked
            foreach (var building in _cityData.Buildings)
            {
                try
                {
                    var mesh = GeometryBuilder.CreateBuildingBox(building);
                    var color = building.Color.ToMediaColor();
                    var material = new DiffuseMaterial(new SolidColorBrush(color));
                    var geo = new GeometryModel3D(mesh, material);
                    geo.BackMaterial = material;
                    var visual = new ModelVisual3D { Content = geo };
                    _buildingMap[geo] = building;
                    CityViewport.Children.Add(visual);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error rendering building {building.Name}: {ex.Message}", "Render Error");
                }
            }

            // Create labels (hidden by default)
            _labelsGroup = new ModelVisual3D();
            CreateLabels();
            if (ShowNamesCheckBox.IsChecked == true)
                CityViewport.Children.Add(_labelsGroup);

            // Fit all elements in view
            CityViewport.ZoomExtents();
            UpdateCameraState();
        }

        private void CreateLabels()
        {
            foreach (var building in _cityData.Buildings)
            {
                var position = new Point3D(
                    building.Position.X,
                    building.Position.Y,
                    building.Position.Z + building.Size.Z + 2);

                var label = new BillboardTextVisual3D
                {
                    Text = building.Name,
                    Position = position,
                    Foreground = Brushes.White,
                    FontSize = 12,
                    Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(160, 0, 0, 0)),
                    Padding = new Thickness(4, 2, 4, 2)
                };
                _labelsGroup.Children.Add(label);
            }

            foreach (var road in _cityData.Roads)
            {
                if (road.Points.Count < 2)
                    continue;

                // Place label at the midpoint of the road
                int midIndex = road.Points.Count / 2;
                var midPoint = road.Points[midIndex];
                var position = new Point3D(midPoint.X, midPoint.Y, midPoint.Z + 3);

                var label = new BillboardTextVisual3D
                {
                    Text = road.Name,
                    Position = position,
                    Foreground = Brushes.Yellow,
                    FontSize = 10,
                    Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(140, 50, 50, 50)),
                    Padding = new Thickness(4, 2, 4, 2)
                };
                _labelsGroup.Children.Add(label);
            }
        }

        private void ShowNames_Changed(object sender, RoutedEventArgs e)
        {
            if (_labelsGroup == null)
                return;

            if (ShowNamesCheckBox.IsChecked == true)
            {
                if (!CityViewport.Children.Contains(_labelsGroup))
                    CityViewport.Children.Add(_labelsGroup);
            }
            else
            {
                CityViewport.Children.Remove(_labelsGroup);
            }
        }

        private void Viewport_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_buildingMap.Count == 0)
                return;

            if (CityViewport.Camera is not PerspectiveCamera camera)
                return;

            var viewport3D = CityViewport.Viewport;
            if (viewport3D == null)
                return;

            double vpW = viewport3D.ActualWidth;
            double vpH = viewport3D.ActualHeight;
            if (vpW < 1 || vpH < 1)
                return;

            var point = Mouse.GetPosition(viewport3D);

            // Build orthonormal camera basis
            var look = camera.LookDirection;
            look.Normalize();
            var right = Vector3D.CrossProduct(look, camera.UpDirection);
            right.Normalize();
            var up = Vector3D.CrossProduct(right, look);
            up.Normalize();

            double fovRad = camera.FieldOfView * Math.PI / 180.0;
            double tanHalf = Math.Tan(fovRad / 2.0);
            double aspect = vpW / vpH;

            double nx = (2.0 * point.X / vpW - 1.0) * tanHalf;
            double ny = (1.0 - 2.0 * point.Y / vpH) * tanHalf / aspect;

            var rayDir = look + nx * right + ny * up;
            rayDir.Normalize();
            var rayOrigin = camera.Position;

            // Find closest building via ray-AABB intersection
            GeometryModel3D closestGeo = null;
            double closestDist = double.MaxValue;

            foreach (var kvp in _buildingMap)
            {
                var b = kvp.Value;
                double hw = b.Size.X / 2, hd = b.Size.Y / 2;
                var boxMin = new Point3D(b.Position.X - hw, b.Position.Y - hd, b.Position.Z);
                var boxMax = new Point3D(b.Position.X + hw, b.Position.Y + hd, b.Position.Z + b.Size.Z);

                if (RaycastHelper.RayIntersectsBox(rayOrigin, rayDir, boxMin, boxMax, out double t) && t < closestDist)
                {
                    closestDist = t;
                    closestGeo = kvp.Key;
                }
            }

            if (closestGeo != null && _buildingMap.TryGetValue(closestGeo, out var building))
            {
                // Restore previous selection
                if (_selectedGeo != null)
                    SetBuildingHighlight(_selectedGeo, false);

                _selectedGeo = closestGeo;
                SetBuildingHighlight(closestGeo, true);

                // Show details
                DetailName.Text = building.Name;
                DetailType.Text = building.Type;
                DetailSize.Text = $"{building.Size.X} × {building.Size.Y} × {building.Size.Z}";
                DetailPosition.Text = $"({building.Position.X}, {building.Position.Y}, {building.Position.Z})";
                DetailsPanel.Visibility = Visibility.Visible;

                StatusBlock.Text = $"Selected: {building.Name}";
                e.Handled = true;
            }
        }

        private void SetBuildingHighlight(GeometryModel3D geo, bool highlight)
        {
            if (_buildingMap.TryGetValue(geo, out var building))
            {
                var color = highlight
                    ? Colors.Yellow
                    : building.Color.ToMediaColor();
                var material = new DiffuseMaterial(new SolidColorBrush(color));
                geo.Material = material;
                geo.BackMaterial = material;
            }
        }

        private void UpdateCameraState()
        {
            if (CityViewport.Camera is PerspectiveCamera cam)
            {
                var vp = CityViewport.Viewport;
                var ic = System.Globalization.CultureInfo.InvariantCulture;
                string F(double v) => v.ToString("R", ic);
                CameraState.Text = string.Join("|",
                    $"{F(cam.Position.X)},{F(cam.Position.Y)},{F(cam.Position.Z)}",
                    $"{F(cam.LookDirection.X)},{F(cam.LookDirection.Y)},{F(cam.LookDirection.Z)}",
                    $"{F(cam.UpDirection.X)},{F(cam.UpDirection.Y)},{F(cam.UpDirection.Z)}",
                    F(cam.FieldOfView),
                    $"{F(vp.ActualWidth)},{F(vp.ActualHeight)}");
            }
        }

        private void CloseDetails_Click(object sender, RoutedEventArgs e)
        {
            DetailsPanel.Visibility = Visibility.Collapsed;
            if (_selectedGeo != null)
            {
                SetBuildingHighlight(_selectedGeo, false);
                _selectedGeo = null;
            }
            StatusBlock.Text = "City loaded successfully. Right-click to rotate, middle-click to pan, scroll to zoom.";
        }

        private void ResetCamera_Click(object sender, RoutedEventArgs e)
        {
            CityViewport.ZoomExtents();
            UpdateCameraState();
            StatusBlock.Text = "Camera reset.";
        }

        private void ShowInfo_Click(object sender, RoutedEventArgs e)
        {
            if (_cityData == null)
                return;

            string info = $"City: {_cityData.Name}\n\n";
            info += $"Center: {_cityData.Center.Latitude:F4}°N, {Math.Abs(_cityData.Center.Longitude):F4}°W\n";
            info += $"Scale: {_cityData.Scale}\n\n";
            info += $"Buildings: {_cityData.Buildings.Count}\n";
            
            foreach (var building in _cityData.Buildings)
            {
                info += $"  • {building.Name} ({building.Type})\n";
            }

            info += $"\nRoads: {_cityData.Roads.Count}\n";
            
            foreach (var road in _cityData.Roads)
            {
                info += $"  • {road.Name} ({road.Type}) - {road.Width}m wide\n";
            }

            MessageBox.Show(info, "City Information", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}

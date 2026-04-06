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
            CityViewport.PreviewMouseLeftButtonDown += Viewport_PreviewMouseLeftButtonDown;
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

        private void Viewport_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var point = e.GetPosition(CityViewport);
            GeometryModel3D hitGeo = null;

            VisualTreeHelper.HitTest(
                CityViewport,
                null,
                result =>
                {
                    if (result is RayMeshGeometry3DHitTestResult meshResult &&
                        meshResult.ModelHit is GeometryModel3D geo &&
                        _buildingMap.ContainsKey(geo))
                    {
                        hitGeo = geo;
                        return HitTestResultBehavior.Stop;
                    }
                    return HitTestResultBehavior.Continue;
                },
                new PointHitTestParameters(point));

            if (hitGeo != null && _buildingMap.TryGetValue(hitGeo, out var building))
            {
                // Restore previous selection
                if (_selectedGeo != null)
                    SetBuildingHighlight(_selectedGeo, false);

                _selectedGeo = hitGeo;
                SetBuildingHighlight(hitGeo, true);

                // Show details
                DetailsBlock.Text = $"Name: {building.Name}\nType: {building.Type}\n" +
                    $"Size: {building.Size.X} × {building.Size.Y} × {building.Size.Z}\n" +
                    $"Position: ({building.Position.X}, {building.Position.Y}, {building.Position.Z})";
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
                geo.Material = new DiffuseMaterial(new SolidColorBrush(color));
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

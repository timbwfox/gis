using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf;
using CityGIS.Models;

namespace CityGIS
{
    public partial class MainWindow : Window
    {
        private CityData _cityData;
        private ModelVisual3D _buildingsGroup;
        private ModelVisual3D _roadsGroup;
        private ModelVisual3D _labelsGroup;

        public MainWindow()
        {
            InitializeComponent();
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

            // Create groups for buildings and roads
            _roadsGroup = new ModelVisual3D();
            _buildingsGroup = new ModelVisual3D();

            var roadsModel = new Model3DGroup();
            var buildingsModel = new Model3DGroup();

            // Render roads first (so they appear below buildings)
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

            // Render buildings
            foreach (var building in _cityData.Buildings)
            {
                try
                {
                    var buildingElement = GeometryBuilder.CreateBuildingModel(building);
                    buildingsModel.Children.Add(buildingElement.Model);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error rendering building {building.Name}: {ex.Message}", "Render Error");
                }
            }

            _roadsGroup.Content = roadsModel;
            _buildingsGroup.Content = buildingsModel;

            CityViewport.Children.Add(_roadsGroup);
            CityViewport.Children.Add(_buildingsGroup);

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

# City GIS 3D Viewer

A simple GIS desktop application built with C# and open-source libraries that displays roads and buildings of a city in 3D.

## Features

- **3D Visualization**: View city buildings and roads in 3D space using HelixToolkit
- **JSON Data Format**: Load city data from a structured JSON file
- **Interactive Camera**: 
  - Right-click and drag to rotate the view
  - Middle-click and drag to pan
  - Mouse wheel to zoom
  - Click "Reset Camera" to return to default view
- **Building Display**: 
  - 3D rectangular buildings with custom colors
  - Building types: government, public, commercial, medical, residential, recreation
- **Road Display**: 
  - Extruded roads with proper width and paths
  - Road types: primary, secondary
- **City Information**: Click "Show Info" to see details about all buildings and roads

## Architecture

The application consists of:

- **Models** (`Models/GeoModels.cs`): Data model classes for Vector3, Color, Building, Road, and CityData
- **CityDataLoader.cs**: Deserializes JSON city data into model objects
- **GeometryBuilder.cs**: Creates 3D geometry meshes from building and road data using WPF Media3D
- **MainWindow.xaml/xaml.cs**: WPF UI with 3D viewport and controls
- **Data/city_data.json**: Sample city data file

## Building Requirements

- .NET 10.0 or later
- Visual Studio 2026 or VS Code with C# extension
- NuGet (automatically handled by .NET)

## Dependencies

- **HelixToolkit.Wpf** (3.1.2): Open-source 3D graphics library for WPF
- **NetTopologySuite** (2.6.0): Open-source spatial geometry library
- **Newtonsoft.Json** (13.0.4): JSON serialization/deserialization

## How to Build

### Using Visual Studio
1. Open `gis.sln` in Visual Studio 2026
2. Right-click the project → "Restore NuGet Packages"
3. Press F5 or click "Start Debugging"

### Using .NET CLI
```bash
cd CityGIS
dotnet restore
dotnet build
dotnet run
```

## Data Format

The `Data/city_data.json` file defines the city with this structure:

```json
{
  "city": {
    "name": "City Name",
    "center": {"latitude": 0.0, "longitude": 0.0},
    "scale": 1.0
  },
  "buildings": [
    {
      "id": "unique_id",
      "name": "Building Name",
      "type": "building_type",
      "position": {"x": 0.0, "y": 0.0, "z": 0.0},
      "size": {"width": 50.0, "depth": 40.0, "height": 35.0},
      "color": {"r": 200, "g": 100, "b": 50}
    }
  ],
  "roads": [
    {
      "id": "unique_id",
      "name": "Road Name",
      "type": "primary",
      "width": 20.0,
      "points": [
        {"x": 0.0, "y": 0.0, "z": -0.1},
        {"x": 100.0, "y": 0.0, "z": -0.1}
      ],
      "color": {"r": 100, "g": 100, "b": 100}
    }
  ]
}
```

### Parameters

- **Building coordinates**: X (east-west), Y (north-south), Z (elevation)
- **Size**: Width, Depth, Height in meters
- **Colors**: RGB values (0-255)

## Sample City Data

The included `city_data.json` contains an imaginary city "SimploCity" with:
- 6 buildings (Town Hall, Library, Mall, Hospital, Residential Complex, Park Pavilion)
- 5 roads (Main Street, Central Avenue, East Boulevard, West Road, Hospital Drive)

## Controls

| Action | Control |
|--------|---------|
| Rotate | Right-click + drag |
| Pan | Middle-click + drag |
| Zoom | Mouse wheel |
| Reset Camera | Click "Reset Camera" button |
| View Info | Click "Show Info" button |
| Select Building | Left-click a building |

## Extending the Application

### Add More Buildings
Edit `Data/city_data.json` and add entries to the `buildings` array with unique IDs.

### Change Colors
Modify the `color` field in building or road objects (0-255 RGB values).

### Create New City
Replace `city_data.json` with your own data following the same JSON schema.

### Customize Building Types
Edit the `building.type` field to categorize buildings by any classification system.

## E2E Testing

The project includes BDD-style end-to-end tests using Behave and pywinauto that drive the WPF application through the Windows UI Automation framework.

### Test Structure

    tests/
    ├── requirements.txt
    └── features/
        ├── environment.py
        ├── show_names_toggle.feature
        └── steps/
            └── show_names_steps.py

### Prerequisites

- Python 3.10 or later
- The application must be built first (`dotnet build`)

### Running the Tests

    cd tests
    pip install -r requirements.txt
    behave

### UI Automation IDs

Every XAML element exposes an `AutomationProperties.AutomationId` for reliable test targeting:

| AutomationId | Element | Description |
|--------------|---------|-------------|
| lblAppTitle | TextBlock | Application title label |
| txtCityName | TextBlock | Loaded city name |
| btnResetCamera | Button | Resets the 3D camera |
| btnShowInfo | Button | Shows city info dialog |
| chkShowNames | CheckBox | Toggles building/road name labels |
| vpCity | HelixViewport3D | Main 3D viewport |
| pnlDetails | Border | Building details panel container |
| lblDetailsTitle | TextBlock | Building Details heading |
| btnCloseDetails | Button | Closes the details panel |
| lblName | TextBlock | Name label |
| txtName | TextBlock | Selected building name value |
| lblType | TextBlock | Type label |
| txtType | TextBlock | Selected building type value |
| lblSize | TextBlock | Size label |
| txtSize | TextBlock | Selected building size value |
| lblPosition | TextBlock | Position label |
| txtPosition | TextBlock | Selected building position value |
| txtStatus | TextBlock | Status bar message |
| txtStats | TextBlock | Building/road count stats |

### Writing New Tests

1. Add a new `.feature` file under `tests/features/`.
2. Implement step definitions in `tests/features/steps/`.
3. Use `context.main_window.child_window(auto_id="<id>")` to locate elements by their AutomationId.
4. The `environment.py` file handles launching and closing the app for each scenario.

## Open-Source Libraries

- **HelixToolkit** (MIT License): 3D graphics framework
- **NetTopologySuite** (LGPL License): Spatial geometry suite
- **Newtonsoft.Json** (MIT License): JSON handling

## Future Enhancements

- Import real GIS data (GeoJSON, Shapefile)
- Building details panel with more information
- Terrain/elevation map support
- Path routing between buildings
- Street-level camera view
- Data export capabilities
- Performance optimizations for large cities

## License

This project is provided as-is for educational purposes.

# Quick Start Guide

## Building & Running

### Prerequisites
- .NET 6.0 SDK or later installed
- Visual Studio 2022, VS Code, or another IDE supporting C#

### Build Steps

1. **Restore Dependencies**
   ```bash
   cd f:\Personal\Work\Personal\gis\CityGIS
   dotnet restore
   ```

2. **Build the Project**
   ```bash
   dotnet build
   ```

3. **Run the Application**
   ```bash
   dotnet run
   ```

The application will launch and load the sample city data from `Data/city_data.json`.

## Application Features

### Initial Load
- City data automatically loads on startup from `Data/city_data.json`
- The 3D view shows all buildings (boxes) and roads (extruded lines)
- Camera automatically zooms to fit all objects

### Navigation
- **Rotate**: Right-click and drag your mouse
- **Pan**: Middle-click and drag your mouse  
- **Zoom**: Scroll mouse wheel up/down

### UI Controls
- **Reset Camera**: Restores default zoom/angle view
- **Show Info**: Displays list of all buildings and roads in the city

### Status Bar
- Shows current city name
- Displays building and road counts
- Shows helpful hints during interactions

## Project Structure

```
CityGIS/
├── CityGIS.csproj           # Project file with dependencies
├── Models/
│   └── GeoModels.cs         # Data model classes
├── CityDataLoader.cs        # JSON deserialization
├── GeometryBuilder.cs       # 3D mesh creation
├── MainWindow.xaml          # UI layout
├── MainWindow.xaml.cs       # UI logic
├── App.xaml                 # Application entry point
├── App.xaml.cs
├── Program.cs               # Main entry point
└── Data/
    └── city_data.json       # City data in JSON format
```

## Customizing the City Data

### Adding a New Building

Open `Data/city_data.json` and add an entry to the `buildings` array:

```json
{
  "id": "bld_007",
  "name": "My Building",
  "type": "commercial",
  "position": {"x": 50.0, "y": 50.0, "z": 0.0},
  "size": {"width": 40.0, "depth": 30.0, "height": 25.0},
  "color": {"r": 180, "g": 100, "b": 100}
}
```

### Adding a New Road

Add an entry to the `roads` array:

```json
{
  "id": "road_006",
  "name": "New Street",
  "type": "secondary",
  "width": 15.0,
  "points": [
    {"x": -100.0, "y": 100.0, "z": -0.1},
    {"x": 0.0, "y": 100.0, "z": -0.1},
    {"x": 100.0, "y": 100.0, "z": -0.1}
  ],
  "color": {"r": 120, "g": 120, "b": 120}
}
```

### Coordinate System

- **X-axis**: East-West (positive = East)
- **Y-axis**: North-South (positive = North)  
- **Z-axis**: Up-Down (elevation, positive = up)

All measurements are in meters.

## Troubleshooting

### Application crashes at startup
- Check that `Data/city_data.json` exists in the output directory
- Verify JSON syntax (use an online JSON validator if unsure)
- Check console output for detailed error messages

### 3D viewport is blank
- Click "Reset Camera" button to fit view
- Make sure buildings aren't positioned too far from origin
- Check the status bar for error messages

### Cannot build project
- Ensure .NET 6.0+ is installed: `dotnet --version`
- Restore packages: `dotnet restore`
- Check for syntax errors in C# files

## Next Steps

For more information, see [README.md](README.md) in the root directory.

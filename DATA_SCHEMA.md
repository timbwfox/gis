# City Data Schema Reference

Complete JSON schema for creating custom city data files.

## Root Structure

```json
{
  "city": { ... },
  "buildings": [ ... ],
  "roads": [ ... ]
}
```

## City Metadata

```json
{
  "city": {
    "name": "string",            // Required: City name
    "center": {
      "latitude": number,        // Required: Latitude (decimal degrees)
      "longitude": number        // Required: Longitude (decimal degrees)
    },
    "scale": number             // Optional: Default 1.0
  }
}
```

## Buildings Array

Each building object represents a 3D rectangular structure:

```json
{
  "buildings": [
    {
      "id": "string",           // Required: Unique identifier (e.g., "bld_001")
      "name": "string",         // Required: Building name
      "type": "string",         // Optional: Building type (e.g., "commercial", "residential")
      "position": {
        "x": number,            // Required: X coordinate (East-West, meters)
        "y": number,            // Required: Y coordinate (North-South, meters)
        "z": number             // Required: Z coordinate (Elevation, meters)
      },
      "size": {
        "width": number,        // Required: Width in meters (X axis)
        "depth": number,        // Required: Depth in meters (Y axis)
        "height": number        // Required: Height in meters (Z axis)
      },
      "color": {
        "r": number,            // Required: Red component (0-255)
        "g": number,            // Required: Green component (0-255)
        "b": number             // Required: Blue component (0-255)
      }
    }
  ]
}
```

### Building Types
Common types for categorization:
- `government` - Government buildings
- `public` - Public facilities
- `commercial` - Shops, offices
- `residential` - Housing
- `medical` - Hospitals, clinics
- `recreation` - Parks, museums
- `industrial` - Factories, warehouses
- `transportation` - Stations, terminals
- `education` - Schools, universities

## Roads Array

Each road represents an extruded line following a path:

```json
{
  "roads": [
    {
      "id": "string",           // Required: Unique identifier (e.g., "road_001")
      "name": "string",         // Required: Road name
      "type": "string",         // Optional: Road type (e.g., "primary", "secondary")
      "width": number,          // Required: Road width in meters
      "points": [
        {
          "x": number,          // Required: X coordinate
          "y": number,          // Required: Y coordinate
          "z": number           // Required: Z coordinate (usually -0.1 to be below buildings)
        }
      ],
      "color": {
        "r": number,            // Required: Red component (0-255)
        "g": number,            // Required: Green component (0-255)
        "b": number             // Required: Blue component (0-255)
      }
    }
  ]
}
```

### Road Types
Common types:
- `primary` - Main roads, highways
- `secondary` - Secondary roads
- `tertiary` - Local roads
- `residential` - Residential streets
- `pedestrian` - Walkways
- `bicycle` - Bike lanes

## Color Reference

### Common Colors (RGB)

| Name | R | G | B | Usage |
|------|---|---|---|-------|
| Gray | 128 | 128 | 128 | Roads |
| Light Gray | 200 | 200 | 200 | Buildings |
| Brown | 200 | 100 | 50 | Historic buildings |
| Blue | 100 | 150 | 255 | Government |
| Red | 255 | 100 | 100 | Emergency |
| Green | 100 | 200 | 100 | Parks |
| Yellow | 255 | 255 | 100 | Commercial |
| White | 255 | 255 | 255 | Light buildings |
| Black | 50 | 50 | 50 | Dark buildings |

## Coordinate System

### Axes
- **X-axis**: East-West (positive = East)
- **Y-axis**: North-South (positive = North)
- **Z-axis**: Up-Down (positive = Up)

### Units
All measurements are in meters.

### Best Practices
- Place city center near origin (0, 0)
- Use consistent Z-coordinates for ground level (e.g., 0 for buildings, -0.1 for roads)
- Keep coordinates within ±500 for best performance
- Position roads slightly below ground (z = -0.1) to prevent z-fighting

## Example: Complete City

```json
{
  "city": {
    "name": "Example City",
    "center": {
      "latitude": 51.5074,
      "longitude": -0.1278
    },
    "scale": 1.0
  },
  "buildings": [
    {
      "id": "bld_001",
      "name": "Central Station",
      "type": "transportation",
      "position": {"x": 0, "y": 0, "z": 0},
      "size": {"width": 100, "depth": 80, "height": 50},
      "color": {"r": 180, "g": 140, "b": 100}
    },
    {
      "id": "bld_002",
      "name": "City Hall",
      "type": "government",
      "position": {"x": 150, "y": 0, "z": 0},
      "size": {"width": 60, "depth": 50, "height": 45},
      "color": {"r": 100, "g": 150, "b": 255}
    }
  ],
  "roads": [
    {
      "id": "road_001",
      "name": "Main Street",
      "type": "primary",
      "width": 25,
      "points": [
        {"x": -200, "y": 0, "z": -0.1},
        {"x": 200, "y": 0, "z": -0.1}
      ],
      "color": {"r": 100, "g": 100, "b": 100}
    }
  ]
}
```

## Validation Notes

- All numeric values should be valid numbers (no strings)
- IDs must be unique within their respective arrays
- Arrays cannot be empty (should have at least 1 building or road)
- Colors must be integers between 0 and 255
- Position and size coordinates should be reasonable (typically ±1000 meters)

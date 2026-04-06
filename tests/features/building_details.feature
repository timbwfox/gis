Feature: Building Details
  As a user of the City GIS 3D Viewer
  I want to click on a building to see its details
  So that I can learn more about the buildings in the city

  Background:
    Given the City GIS application is running

  Scenario: Click on the Library shows its details
    Given the building details panel is not visible
     When I left-click on the "Library" building in the viewport
     Then the building details panel should be visible
      And the app displays controls and values as per the following bundle
        | element_name  | expected_value |
        | Building_Name | Library        |
        | Building_Type | public         |
      And the status bar should contain "Selected: Library"

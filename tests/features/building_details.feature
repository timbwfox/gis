Feature: Building Details
  As a user of the City GIS 3D Viewer
  I want to click on a building to see its details
  So that I can learn more about the buildings in the city

  Background:
    Given the City GIS application is running

  Scenario: Click on the Library shows its details
    Given "Building_Details_Label" is not displayed
     When I left-click on the "Library" building in the viewport
     Then "Building_Details_Label" is displayed
      And the app displays elements and values as per bundle:
        | Building_Name | Library              |
        | Building_Type | public               |
        | Status_Bar    | *Selected: Library*  |

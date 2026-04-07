Feature: Building Details
  As a user of the City GIS 3D Viewer
  I want to click on a building to see its details
  So that I can learn more about the buildings in the city

  Background:
    Given the City GIS application is running

  Scenario Outline: Click on a building shows its details
    Given "Building_Details_Label" is not displayed
     When I left-click on the "<building_name>" building in the viewport
     Then "Building_Details_Label" is displayed
      And the app displays elements and values as per bundle:
        | Building_Name | <building_name>              |
        | Building_Type | <building_type>              |
        | Status_Bar    | *Selected: <building_name>*  |

    Examples:
      | building_name         | building_type |
      | Town Hall             | government    |
      | Library               | public        |
      | City Center Mall      | commercial    |
      | Hospital              | medical       |
      | Residential Complex A | residential   |
      | Park Pavilion         | recreation    |

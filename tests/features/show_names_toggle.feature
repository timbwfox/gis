Feature: Show Names Toggle
  As a user of the City GIS 3D Viewer
  I want to toggle building and road name labels on and off
  So that I can identify features when needed without cluttering the view

  Background:
    Given the City GIS application is running

  Scenario: Enable Show Names displays labels
    Given the app displays elements and values as per bundle:
      | Show_Names_Checkbox | False |
     When I populate the app with the following bundle:
      | Show_Names_Checkbox | True |
     Then the app displays elements and values as per bundle:
      | Show_Names_Checkbox | True                        |
      | Status_Bar          | *City loaded successfully*  |

  Scenario: Disable Show Names hides labels
    Given the app displays elements and values as per bundle:
      | Show_Names_Checkbox | True |
     When I populate the app with the following bundle:
      | Show_Names_Checkbox | False |
     Then the app displays elements and values as per bundle:
      | Show_Names_Checkbox | False                       |
      | Status_Bar          | *City loaded successfully*  |

  Scenario: Toggle Show Names on then off returns to original state
    Given the app displays elements and values as per bundle:
      | Show_Names_Checkbox | False |
     When I populate the app with the following bundle:
      | Show_Names_Checkbox | True |
      And I populate the app with the following bundle:
      | Show_Names_Checkbox | False |
     Then the app displays elements and values as per bundle:
      | Show_Names_Checkbox | False |

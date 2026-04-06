Feature: Show Names Toggle
  As a user of the City GIS 3D Viewer
  I want to toggle building and road name labels on and off
  So that I can identify features when needed without cluttering the view

  Background:
    Given the City GIS application is running

  Scenario: Enable Show Names displays labels
    Given the "Show Names" checkbox is unchecked
     When I check the "Show Names" checkbox
     Then the "Show Names" checkbox should be checked
      And the status bar should contain "City loaded successfully"

  Scenario: Disable Show Names hides labels
    Given the "Show Names" checkbox is checked
     When I uncheck the "Show Names" checkbox
     Then the "Show Names" checkbox should be unchecked
      And the status bar should contain "City loaded successfully"

  Scenario: Toggle Show Names on then off returns to original state
    Given the "Show Names" checkbox is unchecked
     When I check the "Show Names" checkbox
      And I uncheck the "Show Names" checkbox
     Then the "Show Names" checkbox should be unchecked

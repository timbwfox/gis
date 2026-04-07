"""Page Object Model for the City GIS application.

Maps logical element names used in feature steps to UI Automation locators
so that steps can reference elements by a human-readable name.
"""

# Element name -> locator kwargs accepted by pywinauto's child_window()
ELEMENTS = {
    # Top Panel
    "App_Title":              {"auto_id": "lblAppTitle", "control_type": "Text"},
    "City_Name":              {"auto_id": "txtCityName", "control_type": "Text"},
    "Reset_Camera_Button":    {"auto_id": "btnResetCamera", "control_type": "Button"},
    "Show_Info_Button":       {"auto_id": "btnShowInfo", "control_type": "Button"},
    "Show_Names_Checkbox":    {"auto_id": "chkShowNames", "control_type": "CheckBox"},

    # 3D Viewport
    "Viewport":               {"auto_id": "vpCity"},

    # Building Details Panel
    "Building_Details_Label": {"auto_id": "lblBuildingDetails", "control_type": "Text"},
    "Close_Details_Button":   {"auto_id": "btnCloseDetails", "control_type": "Button"},
    "Building_Name_Label":    {"auto_id": "lblName", "control_type": "Text"},
    "Building_Name":          {"auto_id": "txtName", "control_type": "Text"},
    "Building_Type_Label":    {"auto_id": "lblType", "control_type": "Text"},
    "Building_Type":          {"auto_id": "txtType", "control_type": "Text"},
    "Building_Size_Label":    {"auto_id": "lblSize", "control_type": "Text"},
    "Building_Size":          {"auto_id": "txtSize", "control_type": "Text"},
    "Building_Position_Label": {"auto_id": "lblPosition", "control_type": "Text"},
    "Building_Position":      {"auto_id": "txtPosition", "control_type": "Text"},

    # Status Bar
    "Status_Bar":             {"auto_id": "txtStatus", "control_type": "Text"},
    "Stats":                  {"auto_id": "txtStats", "control_type": "Text"},
    "Camera_State":           {"auto_id": "txtCameraState", "control_type": "Text"},
}


def get_locator(element_name):
    """Return the locator dict for *element_name*, or raise if unknown."""
    try:
        return ELEMENTS[element_name]
    except KeyError:
        raise KeyError(
            f"Unknown element '{element_name}'. "
            f"Available elements: {', '.join(sorted(ELEMENTS))}"
        )


def find_element(context, element_name):
    """Locate and return the UI element on the main window."""
    locator = get_locator(element_name)
    return context.main_window.child_window(**locator)

"""Page Object Model for the Building Details panel.

Maps logical element names used in feature steps to UI Automation locators
so that steps can reference elements by a human-readable name.
"""

# Element name -> locator kwargs accepted by pywinauto's child_window()
ELEMENTS = {
    "Building_Name": {"auto_id": "txtName", "control_type": "Text"},
    "Building_Type": {"auto_id": "txtType", "control_type": "Text"},
    "Details_Title": {"auto_id": "lblDetailsTitle", "control_type": "Text"},
    "Status_Bar":    {"auto_id": "txtStatus", "control_type": "Text"},
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

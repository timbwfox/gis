import time

from behave import given, when, then
from pywinauto.timings import wait_until


# ---------------------------------------------------------------------------
# Helpers
# ---------------------------------------------------------------------------

def _get_detail_text(context, auto_id):
    """Read the text of a detail field by its AutomationId."""
    element = context.main_window.child_window(
        auto_id=auto_id, control_type="Text"
    )
    return element.window_text()


def _get_status_text(context):
    """Read the current status bar text."""
    status = context.main_window.child_window(
        auto_id="txtStatus", control_type="Text"
    )
    return status.window_text()


def _click_building_by_name(context, building_name):
    """Scan the viewport in a grid pattern to find and click a building."""
    viewport = context.main_window.child_window(auto_id="vpCity")
    rect = viewport.rectangle()
    w = rect.width()
    h = rect.height()

    steps = 9  # 9×9 grid gives good coverage
    for row in range(steps):
        for col in range(steps):
            x_off = int(w * (col + 0.5) / steps)
            y_off = int(h * (row + 0.5) / steps)

            viewport.click_input(coords=(x_off, y_off))
            time.sleep(0.3)

            try:
                name = _get_detail_text(context, "txtName")
                if name == building_name:
                    return True
            except Exception:
                continue

    return False


# ---------------------------------------------------------------------------
# Given
# ---------------------------------------------------------------------------

@given('the building details panel is not visible')
def step_details_not_visible(context):
    try:
        panel = context.main_window.child_window(auto_id="pnlDetails")
        assert not panel.is_visible(), \
            "Building details panel should not be visible initially"
    except Exception:
        pass  # Element not found means it is collapsed — expected


# ---------------------------------------------------------------------------
# When
# ---------------------------------------------------------------------------

@when('I left-click on the "{building_name}" building in the viewport')
def step_click_building(context, building_name):
    found = _click_building_by_name(context, building_name)
    assert found, (
        f"Could not find building '{building_name}' by clicking in the viewport"
    )


# ---------------------------------------------------------------------------
# Then
# ---------------------------------------------------------------------------

@then('the building details panel should be visible')
def step_details_visible(context):
    panel = context.main_window.child_window(auto_id="pnlDetails")
    wait_until(
        timeout=5, retry_interval=0.3,
        func=lambda: panel.is_visible(),
    )
    assert panel.is_visible(), "Building details panel should be visible"


@then('the building name should be "{expected_name}"')
def step_check_name(context, expected_name):
    actual = _get_detail_text(context, "txtName")
    assert actual == expected_name, \
        f"Expected name '{expected_name}', got '{actual}'"


@then('the building type should be "{expected_type}"')
def step_check_type(context, expected_type):
    actual = _get_detail_text(context, "txtType")
    assert actual == expected_type, \
        f"Expected type '{expected_type}', got '{actual}'"

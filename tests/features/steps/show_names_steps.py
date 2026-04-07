from behave import given, then

from pages.app_page import find_element


# ---------------------------------------------------------------------------
# Helpers
# ---------------------------------------------------------------------------

def _get_status_text(context):
    """Read the current status bar text."""
    return find_element(context, "Status_Bar").window_text()


# ---------------------------------------------------------------------------
# Given
# ---------------------------------------------------------------------------

@given('the City GIS application is running')
def step_app_is_running(context):
    # Handled by environment.py before_scenario; just verify the window exists
    assert context.main_window.exists(), "Main window should be visible"


# ---------------------------------------------------------------------------
# Then
# ---------------------------------------------------------------------------

@then('the status bar should contain "{text}"')
def step_assert_status_contains(context, text):
    status = _get_status_text(context)
    assert text in status, (
        f'Expected status bar to contain "{text}", but got: "{status}"'
    )

from behave import given, when, then
from pywinauto.timings import wait_until

from pages.app_page import find_element


# ---------------------------------------------------------------------------
# Helpers
# ---------------------------------------------------------------------------

def _get_checkbox(context):
    """Locate the 'Show Names' CheckBox via the POM."""
    return find_element(context, "Show_Names_Checkbox")


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
# When
# ---------------------------------------------------------------------------

@when('I check the "Show Names" checkbox')
def step_check_checkbox(context):
    cb = _get_checkbox(context)
    if cb.get_toggle_state() == 0:
        cb.click_input()
    wait_until(
        timeout=5, retry_interval=0.3,
        func=lambda: cb.get_toggle_state() == 1,
    )


@when('I uncheck the "Show Names" checkbox')
def step_uncheck_checkbox(context):
    cb = _get_checkbox(context)
    if cb.get_toggle_state() == 1:
        cb.click_input()
    wait_until(
        timeout=5, retry_interval=0.3,
        func=lambda: cb.get_toggle_state() == 0,
    )


# ---------------------------------------------------------------------------
# Then
# ---------------------------------------------------------------------------

@then('the status bar should contain "{text}"')
def step_assert_status_contains(context, text):
    status = _get_status_text(context)
    assert text in status, (
        f'Expected status bar to contain "{text}", but got: "{status}"'
    )

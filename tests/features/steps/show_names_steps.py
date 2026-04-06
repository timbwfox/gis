from behave import given, when, then
from pywinauto.timings import wait_until


# ---------------------------------------------------------------------------
# Helpers
# ---------------------------------------------------------------------------

def _get_checkbox(context):
    """Locate the 'Show Names' CheckBox via UI Automation."""
    return context.main_window.child_window(
        title="Show Names", control_type="CheckBox"
    )


def _get_status_text(context):
    """Read the current status bar text."""
    status = context.main_window.child_window(
        auto_id="txtStatus", control_type="Text"
    )
    return status.window_text()


# ---------------------------------------------------------------------------
# Given
# ---------------------------------------------------------------------------

@given('the City GIS application is running')
def step_app_is_running(context):
    # Handled by environment.py before_scenario; just verify the window exists
    assert context.main_window.exists(), "Main window should be visible"


@given('the "Show Names" checkbox is unchecked')
def step_checkbox_is_unchecked(context):
    cb = _get_checkbox(context)
    toggle = cb.get_toggle_state()
    if toggle == 1:  # currently checked
        cb.click_input()
        wait_until(
            timeout=5, retry_interval=0.3,
            func=lambda: cb.get_toggle_state() == 0,
        )
    assert cb.get_toggle_state() == 0, "Precondition: checkbox should be unchecked"


@given('the "Show Names" checkbox is checked')
def step_checkbox_is_checked(context):
    cb = _get_checkbox(context)
    toggle = cb.get_toggle_state()
    if toggle == 0:  # currently unchecked
        cb.click_input()
        wait_until(
            timeout=5, retry_interval=0.3,
            func=lambda: cb.get_toggle_state() == 1,
        )
    assert cb.get_toggle_state() == 1, "Precondition: checkbox should be checked"


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

@then('the "Show Names" checkbox should be checked')
def step_assert_checked(context):
    cb = _get_checkbox(context)
    assert cb.get_toggle_state() == 1, "Checkbox should be checked (toggle state 1)"


@then('the "Show Names" checkbox should be unchecked')
def step_assert_unchecked(context):
    cb = _get_checkbox(context)
    assert cb.get_toggle_state() == 0, "Checkbox should be unchecked (toggle state 0)"


@then('the status bar should contain "{text}"')
def step_assert_status_contains(context, text):
    status = _get_status_text(context)
    assert text in status, (
        f'Expected status bar to contain "{text}", but got: "{status}"'
    )

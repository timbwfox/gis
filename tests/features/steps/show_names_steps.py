from behave import given


# ---------------------------------------------------------------------------
# Given
# ---------------------------------------------------------------------------

@given('the City GIS application is running')
def step_app_is_running(context):
    # Handled by environment.py before_scenario; just verify the window exists
    assert context.main_window.exists(), "Main window should be visible"

from behave import given, when, then
from pywinauto.timings import wait_until

from pages.app_page import find_element, get_locator


# ---------------------------------------------------------------------------
# Given / Then – reusable visibility steps
# ---------------------------------------------------------------------------

@given('"{element_name}" is not displayed')
@then('"{element_name}" is not displayed')
def step_element_not_displayed(context, element_name):
    element = find_element(context, element_name)
    assert not element.exists(timeout=0) or not element.is_visible(), \
        f'"{element_name}" should not be displayed'


@given('"{element_name}" is displayed')
@then('"{element_name}" is displayed')
def step_element_displayed(context, element_name):
    element = find_element(context, element_name)
    wait_until(
        timeout=5, retry_interval=0.3,
        func=lambda: element.exists(timeout=0),
    )
    assert element.exists(timeout=0) and element.is_visible(), \
        f'"{element_name}" should be displayed'


# ---------------------------------------------------------------------------
# When – reusable bundle populate step
# ---------------------------------------------------------------------------

@when('I populate the app as per bundle:')
def step_bundle_populate(context):
    for row in context.table:
        element_name = row[0]
        value = row[1]
        element = find_element(context, element_name)
        locator = get_locator(element_name)
        if locator.get("control_type") == "CheckBox":
            desired_state = 1 if value == "True" else 0
            if element.get_toggle_state() != desired_state:
                element.click_input()
            wait_until(
                timeout=5, retry_interval=0.3,
                func=lambda: element.get_toggle_state() == desired_state,
            )
        else:
            element.set_edit_text(value)


# ---------------------------------------------------------------------------
# Given / Then – reusable bundle verification step
# ---------------------------------------------------------------------------

@given('the app displays elements and values as per bundle:')
@then('the app displays elements and values as per bundle:')
def step_bundle_verify(context):
    errors = []
    for row in context.table:
        element_name = row[0]
        expected_value = row[1]
        element = find_element(context, element_name)
        if not element.exists(timeout=0) or not element.is_visible():
            errors.append(f'"{element_name}": element is not visible')
            continue
        locator = get_locator(element_name)
        if locator.get("control_type") == "CheckBox":
            expected_state = 1 if expected_value == "True" else 0
            actual_state = element.get_toggle_state()
            if actual_state != expected_state:
                errors.append(
                    f'"{element_name}": expected {"checked" if expected_state else "unchecked"}, '
                    f'got {"checked" if actual_state else "unchecked"}'
                )
        elif expected_value.startswith("*") and expected_value.endswith("*"):
            substring = expected_value[1:-1]
            actual = element.window_text()
            if substring not in actual:
                errors.append(
                    f'"{element_name}": expected to contain "{substring}", got "{actual}"'
                )
        else:
            actual = element.window_text()
            if actual != expected_value:
                errors.append(
                    f'"{element_name}": expected "{expected_value}", got "{actual}"'
                )
    assert not errors, "Bundle mismatches:\n  " + "\n  ".join(errors)

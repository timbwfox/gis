import json
import math
import os

from behave import given, when, then
from pywinauto.timings import wait_until

from pages.building_details_page import find_element


# ---------------------------------------------------------------------------
# Paths
# ---------------------------------------------------------------------------

_CITY_DATA_PATH = os.path.normpath(os.path.join(
    os.path.dirname(__file__), "..", "..", "..", "CityGIS", "Data", "city_data.json"
))


# ---------------------------------------------------------------------------
# Helpers
# ---------------------------------------------------------------------------

def _get_detail_text(context, auto_id):
    """Read the text of a detail field by its AutomationId."""
    element = context.main_window.child_window(
        auto_id=auto_id, control_type="Text"
    )
    return element.window_text()


def _load_building(building_name):
    """Load a building dict from city_data.json by name."""
    with open(_CITY_DATA_PATH, "r", encoding="utf-8") as f:
        data = json.load(f)
    for b in data["buildings"]:
        if b["name"] == building_name:
            return b
    raise ValueError(f"Building '{building_name}' not found in {_CITY_DATA_PATH}")


def _parse_camera_state(context):
    """Read the hidden txtCameraState element and parse camera parameters.

    Format: posX,posY,posZ|lookX,lookY,lookZ|upX,upY,upZ|fov|vpW,vpH
    """
    elem = context.main_window.child_window(
        auto_id="txtCameraState", control_type="Text"
    )
    text = elem.window_text()
    parts = text.split("|")
    pos = [float(v) for v in parts[0].split(",")]
    look = [float(v) for v in parts[1].split(",")]
    up = [float(v) for v in parts[2].split(",")]
    fov = float(parts[3])
    vp = [float(v) for v in parts[4].split(",")]
    return pos, look, up, fov, vp[0], vp[1]


def _normalize(v):
    length = math.sqrt(sum(c * c for c in v))
    return [c / length for c in v]


def _cross(a, b):
    return [
        a[1] * b[2] - a[2] * b[1],
        a[2] * b[0] - a[0] * b[2],
        a[0] * b[1] - a[1] * b[0],
    ]


def _dot(a, b):
    return sum(ai * bi for ai, bi in zip(a, b))


def _project_point(point3d, cam_pos, look_dir, up_dir, fov_deg, vp_w, vp_h):
    """Project a 3-D world point to 2-D viewport pixel coordinates.

    This is the exact inverse of the ray construction in
    MainWindow.Viewport_MouseLeftButtonDown.
    """
    look = _normalize(look_dir)
    right = _normalize(_cross(look, up_dir))
    up = _normalize(_cross(right, look))

    # Vector from camera to point
    d = [point3d[i] - cam_pos[i] for i in range(3)]

    d_look = _dot(d, look)
    if d_look <= 0:
        return None  # behind camera

    d_right = _dot(d, right)
    d_up = _dot(d, up)

    # Normalised offsets (match the app's ray maths)
    nx = d_right / d_look
    ny = d_up / d_look

    tan_half = math.tan(math.radians(fov_deg) / 2.0)
    aspect = vp_w / vp_h

    # Invert screen-to-ray equations
    px = vp_w * (nx / tan_half + 1.0) / 2.0
    py = vp_h * (1.0 - ny * aspect / tan_half) / 2.0
    return (px, py)


def _click_building_by_name(context, building_name):
    """Look up the building in the JSON, project its centre to screen
    coordinates, and click there."""
    bld = _load_building(building_name)
    pos = bld["position"]
    size = bld["size"]
    center = [
        pos["x"],
        pos["y"],
        pos["z"] + size["height"] / 2.0,
    ]

    cam_pos, look, up, fov, vp_w, vp_h = _parse_camera_state(context)
    screen = _project_point(center, cam_pos, look, up, fov, vp_w, vp_h)
    assert screen is not None, f"Building '{building_name}' is behind the camera"

    viewport = context.main_window.child_window(auto_id="vpCity")
    viewport.click_input(coords=(int(screen[0]), int(screen[1])))


# ---------------------------------------------------------------------------
# Given
# ---------------------------------------------------------------------------

@given('the building details panel is not visible')
def step_details_not_visible(context):
    # WPF Border has no automation peer, so use a child TextBlock as proxy
    title = context.main_window.child_window(
        auto_id="lblDetailsTitle", control_type="Text"
    )
    assert not title.exists(timeout=0), \
        "Building details panel should not be visible initially"


# ---------------------------------------------------------------------------
# When
# ---------------------------------------------------------------------------

@when('I left-click on the "{building_name}" building in the viewport')
def step_click_building(context, building_name):
    _click_building_by_name(context, building_name)


# ---------------------------------------------------------------------------
# Then
# ---------------------------------------------------------------------------

@then('the building details panel should be visible')
def step_details_visible(context):
    # WPF Border has no automation peer, so use a child TextBlock as proxy
    title = context.main_window.child_window(
        auto_id="lblDetailsTitle", control_type="Text"
    )
    wait_until(
        timeout=5, retry_interval=0.3,
        func=lambda: title.exists(timeout=0),
    )
    assert title.exists(timeout=0), "Building details panel should be visible"


@then('the "{element_name}" is populated with "{expected_value}"')
def step_element_populated(context, element_name, expected_value):
    element = find_element(context, element_name)
    actual = element.window_text()
    assert actual == expected_value, \
        f'Expected "{element_name}" to be "{expected_value}", got "{actual}"'


@then('the app displays controls and values as per the following bundle')
def step_bundle_populated(context):
    errors = []
    for row in context.table:
        element_name = row[0]
        expected_value = row[1]
        element = find_element(context, element_name)
        actual = element.window_text()
        if actual != expected_value:
            errors.append(
                f'"{element_name}": expected "{expected_value}", got "{actual}"'
            )
    assert not errors, "Bundle mismatches:\n  " + "\n  ".join(errors)

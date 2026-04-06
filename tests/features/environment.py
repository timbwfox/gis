import os
import subprocess
import time

from pywinauto import Application
from pywinauto.timings import wait_until

# Path to the built executable (adjust if using a different build config)
APP_EXE = os.path.join(
    os.path.dirname(__file__),
    "..", "..", "CityGIS", "bin", "Debug", "net10.0-windows", "CityGIS.exe"
)
WINDOW_TITLE = "City GIS 3D Viewer"
STARTUP_TIMEOUT = 15  # seconds to wait for the window to appear


def before_scenario(context, scenario):
    """Launch the application before each scenario."""
    exe_path = os.path.abspath(APP_EXE)
    if not os.path.isfile(exe_path):
        raise FileNotFoundError(
            f"CityGIS.exe not found at {exe_path}. "
            "Run 'dotnet build CityGIS\\CityGIS.csproj' first."
        )

    context.app = Application(backend="uia").start(exe_path)
    context.main_window = context.app.window(title=WINDOW_TITLE)
    context.main_window.wait("visible", timeout=STARTUP_TIMEOUT)

    # Wait for city data to finish loading
    _wait_for_city_loaded(context)


def after_scenario(context, scenario):
    """Close the application after each scenario."""
    try:
        if hasattr(context, "app") and context.app.is_process_running():
            context.app.kill()
    except Exception:
        pass


def _wait_for_city_loaded(context):
    """Wait until the status bar indicates the city has been loaded."""
    try:
        wait_until(
            timeout=STARTUP_TIMEOUT,
            retry_interval=0.5,
            func=lambda: _status_bar_contains(context, "City loaded successfully"),
        )
    except TimeoutError:
        pass  # Proceed anyway; the step assertions will catch real failures


def _status_bar_contains(context, text):
    """Check if any text element in the status bar contains the given text."""
    try:
        status = context.main_window.child_window(
            auto_id="StatusBlock", control_type="Text"
        )
        return text in status.window_text()
    except Exception:
        return False

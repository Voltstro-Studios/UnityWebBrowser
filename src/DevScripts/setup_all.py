from os import path
from subprocess import run
from platform import system

from download_cef_base import download_cef
from build_base import build_cef_engine, build_project
from build_base_macos import build_cef_engine_macos

# Setup All. Configure UWB repo on the current running platform

# Make sure CefGlue git module exists
cefglue_path = path.abspath(path.join(__file__, '../../ThirdParty/CefGlue/'))
if not path.exists(cefglue_path):
    print('CefGlue git submodule does not exist! Assuming user has not cloned repo recursively and will attempt to init and update submodules...')
    run(['git', 'submodule', 'init'])
    run(['git', 'submodule', 'update'])

# Build shared first, needed by everything
build_project('VoltstroStudios.UnityWebBrowser.Shared')

running_system = system()

# Windows
if running_system == 'Windows':
    download_cef('windows64')
    build_cef_engine('win-x64')

# Linux
if running_system == 'Linux':
    download_cef('linux64')
    build_cef_engine('linux-x64')

# MacOS
if running_system == 'Darwin':
    download_cef('macosx64')
    download_cef('macosarm64')

    build_cef_engine_macos('x64')
    build_cef_engine_macos('arm64')

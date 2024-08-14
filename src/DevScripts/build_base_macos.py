import subprocess
import shutil
import os

def build_cef_engine_macos(arch) -> None:
    """
    MacOS custom build script.

    MacOS needs a custom build process because of the .plist files and specific folder layout the build needs to be in
    """

    # Project path
    cef_engine_path = os.path.abspath(os.path.join(__file__, '../../UnityWebBrowser.Engine.Cef/Main'))
    cef_engine_subprocess_path = os.path.abspath(os.path.join(__file__, '../../UnityWebBrowser.Engine.Cef/SubProcess'))
    cef_engine_macos_path = os.path.abspath(os.path.join(__file__, '../../Packages/UnityWebBrowser.Engine.Cef.MacOS-{0}/Engine~/'.format(arch)))

    # Delete build dir
    cef_engine_build_path = os.path.abspath(os.path.join(__file__, '../../UnityWebBrowser.Engine.Cef/bin/Release/publish/osx-{0}/'.format(arch)))
    if os.path.exists(cef_engine_build_path):
        shutil.rmtree(cef_engine_build_path)

    # First, build main CEF engine project
    print('Building CEF Engine from {0}'.format(cef_engine_path))
    subprocess.run(['dotnet', 'publish', '-r=osx-{0}'.format(arch), '-c=Release'], cwd=cef_engine_path)

    # Build SubProcess
    print('Building CEF Engine SubProcess from {0}'.format(cef_engine_subprocess_path))
    subprocess.run(['dotnet', 'publish', '-r=osx-{0}'.format(arch), '-c=Release'], cwd=cef_engine_subprocess_path)

    cef_framework_path = os.path.abspath(os.path.join(__file__, '../../ThirdParty/Libs/cef/macos{0}/Release/Chromium Embedded Framework.framework'.format(arch)))

    cef_engine_app_path = os.path.join(cef_engine_build_path, 'UnityWebBrowser.Engine.Cef.app')
    cef_engine_app_contents_path = os.path.join(cef_engine_app_path, 'Contents')
    cef_engine_app_macos_path = os.path.join(cef_engine_app_contents_path, 'MacOS')
    cef_engine_app_resources_path = os.path.join(cef_engine_app_contents_path, 'Resources')
    cef_engine_app_frameworks_path = os.path.join(cef_engine_app_contents_path, 'Frameworks/')

    os.makedirs(cef_engine_app_macos_path, exist_ok=True)
    os.makedirs(cef_engine_app_resources_path, exist_ok=True)
    os.makedirs(cef_engine_app_frameworks_path, exist_ok=True)

    shutil.copy(os.path.join(cef_engine_build_path, 'UnityWebBrowser.Engine.Cef'), cef_engine_app_macos_path)
    shutil.copy(os.path.join(cef_engine_build_path, 'info.plist'), cef_engine_app_contents_path)
    shutil.copy(os.path.join(cef_engine_build_path, 'icon.icns'), cef_engine_app_resources_path)
    shutil.copytree(cef_framework_path, os.path.join(cef_engine_app_frameworks_path, 'Chromium Embedded Framework.framework'))

    # Copy the many different helper apps needed
    subprocess_types = [
        None,
        'GPU',
        'Plugin',
        'Renderer'
    ]

    for type in subprocess_types:
        if not type:
            name = ''
            plist_file = 'info-subprocess.plist'
        else:
            name = ' ({0})'.format(type)
            plist_file = 'info-subprocess-{0}.plist'.format(type.lower())

        bundle_name = 'UnityWebBrowser.Engine.Cef.SubProcess{0}.app/Contents'.format(name)
        cef_engine_subprocess_app_path = os.path.join(cef_engine_app_frameworks_path, bundle_name)
        cef_engine_subprocess_macos_path = os.path.join(cef_engine_subprocess_app_path, 'MacOS')

        os.makedirs(cef_engine_subprocess_macos_path, exist_ok=True)
        shutil.copy(os.path.join(cef_engine_build_path, plist_file), os.path.join(cef_engine_subprocess_app_path, 'info.plist'))
        shutil.copy(os.path.join(cef_engine_build_path, 'UnityWebBrowser.Engine.Cef.SubProcess'), os.path.join(cef_engine_subprocess_macos_path, 'UnityWebBrowser.Engine.Cef.SubProcess{0}'.format(name)))

    # Copy final app bundle to MacOS package
    if not os.path.exists(cef_engine_macos_path):
        os.makedirs(cef_engine_macos_path, exist_ok=True)

    cef_app_final_path = os.path.join(cef_engine_macos_path, 'UnityWebBrowser.Engine.Cef.app')
    if os.path.exists(cef_app_final_path):
        shutil.rmtree(cef_app_final_path)

    shutil.copytree(cef_engine_app_path, cef_app_final_path)

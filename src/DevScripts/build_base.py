import subprocess
import os

package_rid_mapping = {
    'win-x64': 'Win-x64',
    'linux-x64': 'Linux-x64'
}

def build_cef_engine(platform: str) -> None:
    """
    Base CEF engine build for Windows and Linux
    """
    if platform not in package_rid_mapping:
        raise Exception('Platform {0} is not valid!'.format(platform))
    
    platform_folder = package_rid_mapping[platform]

    cef_project_path = os.path.abspath(os.path.join(__file__, '../../UnityWebBrowser.Engine.Cef/Main/UnityWebBrowser.Engine.Cef.csproj'))
    cef_publish_path = os.path.abspath(os.path.join(__file__, '../../Packages/UnityWebBrowser.Engine.Cef.{0}/Engine~'.format(platform_folder)))

    print('Build CEF project \'{0}\' to \'{1}\''.format(cef_project_path, cef_publish_path))

    subprocess.run(['dotnet', 'publish', cef_project_path, '-r=' + platform, '-p:PublishDir=' + cef_publish_path, '--nologo'])



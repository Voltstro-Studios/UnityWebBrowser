import os
import re
import shutil

from urllib.request import urlretrieve
from tarfile import open as taropen

def download_cef(os_platform: str) -> str:
    """
    Downloads and extracts CEF binaries

    Returns where the extract tar contents are
    """

    # Get and read version file
    cef_version_file = os.path.abspath(os.path.join(__file__, '../../ThirdParty/CefGlue/CefGlue/Interop/version.g.cs'))
    if not os.path.exists(cef_version_file):
        raise Exception('Cef version file does not exist!')
    
    with open(cef_version_file, 'r') as version_file:
        version_file = version_file.read()

    # Find version in file
    matches = re.search(r'CEF_VERSION = \"(.*)\"', version_file)
    version_match = matches.group(1)

    # Get temp path
    temp_path = os.path.abspath(os.path.join(__file__, '../../ThirdParty/Libs/cef/temp/'))
    if not os.path.exists(temp_path):
        os.makedirs(temp_path, exist_ok=True)
    
    # Download compiled cef tar.bz2
    cef_tar_name = 'cef_binary_{0}_{1}_minimal'.format(version_match, os_platform)
    cef_tar_download_url = 'https://cef-builds.spotifycdn.com/{0}.tar.bz2'.format(cef_tar_name)
    cef_tar_path = os.path.join(temp_path, '{0}.tar.bz2'.format(cef_tar_name))

    print('Downloading cef tar.gz from \'{0}\' to \'{1}\''.format(cef_tar_download_url, cef_tar_path))
    urlretrieve(cef_tar_download_url, cef_tar_path)
    if not os.path.exists(cef_tar_path):
        raise Exception('Cef tar download filed!')
    
    # Extract tar.bz2
    cef_extracted_tar_path = os.path.abspath(os.path.join(__file__, '../../ThirdParty/Libs/cef/{0}'.format(os_platform)))
    if os.path.exists(cef_extracted_tar_path):
        shutil.rmtree(cef_extracted_tar_path)
    os.makedirs(cef_extracted_tar_path, exist_ok=True)

    print('Extracting \'{0}\' to \'{1}\'...'.format(cef_tar_path, cef_extracted_tar_path))

    cef_tar_name_length = len(cef_tar_name) + 1

    with taropen(cef_tar_path) as tar_file:
        cef_subdir_files = []
        for member in tar_file.getmembers():
            member.path = member.path[cef_tar_name_length:]
            cef_subdir_files.append(member)

        tar_file.extractall(cef_extracted_tar_path, members=cef_subdir_files)

    return cef_extracted_tar_path

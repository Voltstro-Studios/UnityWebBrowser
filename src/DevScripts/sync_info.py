# Script that syncs info such as versions

import json
from os import path
from shutil import copy, move
from typing import Any

def read_json_from_file(file_path: str) -> Any:
    with open(file_path, 'r') as f:
        json_content = json.load(f)
        return json_content
    
def write_json_to_file(content: Any, file_path: str) -> None:
    with open(file_path, 'w', encoding='utf-8') as f:
        json.dump(content, f, ensure_ascii=False, indent=2)

def sync_package(package: str, version: str, sub_version: str, license_path: str) -> None:
    package_path = path.abspath(path.join(__file__, '../../Packages/{0}/'.format(package)))
    package_json_path = path.join(package_path, 'package.json')

    package_json = read_json_from_file(package_json_path)

    new_package_version = version
    if sub_version:
        new_package_version += '-{0}'.format(sub_version)

    # Set new package version
    package_json['version'] = new_package_version

    # Deal with package dependencies
    dependencies = package_json.get('dependencies')
    if dependencies:
        for dep in dependencies:
            if not dep.startswith('dev.voltstro'):
                continue

            # Main dependencies will just be main version
            dep_version = version

            # Engine dependencies will have sub version
            if dep.find('engine') != -1:
                dep_version = new_package_version
                
            dependencies[dep] = dep_version
    package_json['dependencies'] = dependencies    

    write_json_to_file(package_json, package_json_path)
    copy(license_path, package_path)


# Get main version.json version
version_json_path = path.abspath(path.join(__file__, '../../version.json'))
version_json = read_json_from_file(version_json_path)

version = version_json['version']
license_path = path.abspath(path.join(__file__, '../../../LICENSE.md'))

# Sync CEF Engine version.json  
cef_engine_version_json_path = path.abspath(path.join(__file__, '../../UnityWebBrowser.Engine.Cef/version.json'))
cef_engine_version_json = read_json_from_file(cef_engine_version_json_path)
cef_engine_versions = cef_engine_version_json['version'].split('-')

cef_engine_versions[0] = version

cef_engine_version = '-'.join(cef_engine_versions)
cef_engine_version_json['version'] = cef_engine_version
write_json_to_file(cef_engine_version_json, cef_engine_version_json_path)

cef_version = cef_engine_versions[1]

# Package Name - Sub Version
packages = {
    'UnityWebBrowser': None,
    'UnityWebBrowser.Communication.Pipes': None,
    'UnityWebBrowser.Engine.Cef': cef_version,
    'UnityWebBrowser.Engine.Cef.Win-x64': cef_version,
    'UnityWebBrowser.Engine.Cef.Linux-x64': cef_version,
    'UnityWebBrowser.Engine.Cef.MacOS-x64': cef_version,
    'UnityWebBrowser.Engine.Cef.MacOS-arm64': cef_version,
}

for package in packages:
    sub_version = packages[package]
    sync_package(package, version, sub_version, license_path)

# Update assembly info
assembly_info_path = path.abspath(path.join(__file__, '../../Packages/UnityWebBrowser/Runtime/AssemblyInfo.cs'))
new_assembly_info_path = assembly_info_path + '.new'

file_attributes = [
    '[assembly: AssemblyVersion("',
    '[assembly: AssemblyFileVersion("'
]

with open(assembly_info_path) as old, open(new_assembly_info_path, 'w') as new:
    for line in old:
        line = line.rstrip()
        for attribute in file_attributes:
            if(line.startswith(attribute)):
                line = line[:len(attribute)] +  version + '")]'

        new.write(line + '\n')

move(new_assembly_info_path, assembly_info_path)

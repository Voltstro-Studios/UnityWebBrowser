import subprocess

from os.path import join
from glob import glob

from download_cef_base import download_cef

cef_extracted_tar_path = download_cef('linux64')

print('Stripping binaries with sstrip')
dynamic_binaries = glob(join(cef_extracted_tar_path, 'Release', '*.so*'))
for binary in dynamic_binaries:
    subprocess.run(['strip', binary])

import os
import glob

files = glob.glob('*.png')
suffix = '_icon'

for full_file_name in files:
    file_name, file_ext = os.path.splitext(full_file_name)
    new_file_name = file_name + suffix + file_ext
    os.rename(full_file_name, new_file_name)
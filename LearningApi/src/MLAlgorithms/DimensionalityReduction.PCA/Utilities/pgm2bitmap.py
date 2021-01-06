import numpy as np
import os
from scipy import misc                     # for loading image

INPUT_DATA_DIR = "./../InputData"
OUTPUT_DATA_DIR = "./../OutputResult"

for root_path, dirs, files in os.walk(INPUT_DATA_DIR):
    for image_file in files:
        if image_file.endswith(".pgm"):
            full_path = root_path + "/" + image_file
            D = misc.imread(full_path)
            misc.imsave(full_path.replace(".pgm", ".bmp"), D)
            misc.toimage()

for root_path, dirs, files in os.walk(OUTPUT_DATA_DIR):
    for image_file in files:
        if image_file.endswith(".pgm"):
            full_path = root_path + "/" + image_file
            D = misc.imread(full_path)
            misc.imsave(full_path.replace(".pgm", ".bmp"), D)

import numpy as np
import os
from scipy import misc                     # for loading image

INPUT_DATA_DIR = "./../InputData"

for root_path, dirs, files in os.walk(INPUT_DATA_DIR):
    for image_file in files:
        if image_file.endswith(".bmp"):
            full_path = root_path + "/" + image_file
            D = misc.imread(full_path)
            np.savetxt(fname=(full_path.replace(".bmp", ".bmp.csv")), X=D, fmt="%5.5f", delimiter=",")
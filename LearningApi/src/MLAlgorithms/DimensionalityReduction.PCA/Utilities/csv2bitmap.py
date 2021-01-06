import os
from numpy import genfromtxt
from scipy import misc

OUTPUT_RESULT_DIR = "./../OutputResult"

for root_path, dirs, files in os.walk(OUTPUT_RESULT_DIR):
    for result_file in files:
        if result_file.endswith(".bmp.csv"):
            full_path = root_path + "/" + result_file
            result_data = genfromtxt(full_path, delimiter=',')
            misc.imsave(full_path.replace(".bmp.csv", ".bmp"), result_data)
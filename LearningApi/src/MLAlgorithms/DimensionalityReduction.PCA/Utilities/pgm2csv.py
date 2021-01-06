import numpy as np
import os
from scipy import misc                     # for loading image

INPUT_DATA_DIR = "./../InputData"

for root_path, dirs, files in os.walk(INPUT_DATA_DIR):
    for image_file in files:
        if image_file.endswith(".pgm"):
            full_path = root_path + "/" + image_file
            D = misc.imread(full_path)
            np.savetxt(fname=(full_path.replace(".pgm", ".pgm.csv")), X=D, fmt="%5.5f", delimiter=",")


# def read_pgm(pgmf):
#     """Return a raster of integers from a PGM as a list of lists."""
#     assert pgmf.readline() == 'P5\n'
#     (width, height) = [int(i) for i in pgmf.readline().split()]
#     depth = int(pgmf.readline())
#     assert depth <= 255
#
#     raster = []
#     for y in range(height):
#         row = []
#         for y in range(width):
#             row.append(ord(pgmf.read(1)))
#         raster.append(row)
#     return raster
#
#
# f = open("./../InputData/face1.pgm", 'rb')
# print read_pgm(f)

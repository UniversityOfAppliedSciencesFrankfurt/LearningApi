import os
import array
import sys
from numpy import genfromtxt

OUTPUT_RESULT_DIR = "./../OutputResult"

for root_path, dirs, files in os.walk(OUTPUT_RESULT_DIR):
    for result_file in files:
        if result_file.endswith(".pgm.csv"):
            try:
                full_path = root_path + "/" + result_file
                result_data = genfromtxt(full_path, delimiter=',')
                height = result_data.shape[0]  # equal number of row
                width = result_data.shape[1]  # equal number of column
                pgmHeader = 'P5' + '\n' + str(width) + '  ' + str(height) + '  ' + str(255) + '\n'
                result_image_file = full_path.replace(".pgm.csv", ".pgm")
                buff = array.array('B')
                for item in result_data.flatten():
                    if int(item) < 0:
                        buff.append(0)
                    else:
                        buff.append(int(item))

                fout = open(result_image_file, 'wb')
                fout.write(pgmHeader)
                buff.tofile(fout)
                fout.close()
            except IOError as e:
                print e
                sys.exit()
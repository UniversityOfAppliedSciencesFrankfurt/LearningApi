import matplotlib.pyplot as plt
import numpy as np
import sys

delimiter = ";"

print(sys.argv)

if len(sys.argv) < 4:
    print("WARNING: Invalid arguments.")
    print("Compatible with Python3 only")
    print("python .\plotStat.py <CSV file with data> <output image file> <with (1) or without (0) header>")
    print("File example: stat.csv")
    print("digit;number of samples;avgcorrcoeff;mincorrcoeff;maxcorrcoeff;deviation")
    print("0;5923;0.8756;-0.2286;0.9999;0.1150")
    print("1;6742;0.9368;0.2353;1.0000;0.0529")
    print("2;5958;0.8794;0.0124;0.9991;0.0931")
    print("")
    print("py .\plotStat.py stat.csv stat.png 1")
    print("")

fileName = sys.argv[1]
saveFile = sys.argv[2]
header = sys.argv[3]
colTitles = []
avgs = []
mins = []
maxs = []
digits = []
deviations = []
lines = open(fileName, 'r')
for line in lines:
    tokens = line.strip().split(delimiter)
    if header == '1':
        for token in tokens:
            colTitles.append(token)
        header = '0'
    else:
        digits.append(tokens[colTitles.index("digit")])
        avgs.append(float(tokens[colTitles.index("avgcorrcoeff")]))
        mins.append(float(tokens[colTitles.index("mincorrcoeff")]))
        maxs.append(float(tokens[colTitles.index("maxcorrcoeff")]))
        deviations.append(float(tokens[colTitles.index("deviation")]))

plt.title('Statistical result of Cross-correlation coefficients of each MNIST digit')
plt.xlabel('Digits');
plt.ylabel('Value');
plt.errorbar(digits, avgs, deviations, fmt='ok',lw=3)
plt.errorbar(digits, avgs, [np.asarray(avgs) - np.asarray(mins), np.asarray(maxs) - np.asarray(avgs)], fmt='.k', ecolor='gray', lw=1)

plt.savefig(saveFile)

plt.show()
plt.close()
import matplotlib.pyplot as plt
import sys
import os
import math


def plotfunc(label, x, y, index):
    plt.figure()
    plt.title('Ring Projection of ' + label + '.' + index)
    a = max(y) + (10 - max(y)%10)
    plt.ylim(top=a)
    plt.plot(x, y)
    plt.savefig('{0}.{1}.png'.format(label, index))


delimiter = ';'

if len(sys.argv) < 2:
    print("WARNING: Invalid arguments.")
    print("Compatible with Python3 only")
    print("python .\plotRingProjectionFunction.py <Header File>")
    print("py .\plotRingProjectionFunction.py LetterA")
    print("")

Label = sys.argv[1]

count = 0
while os.path.isfile('./{0}.{1}.csv'.format(Label, count)):
    count += 1

functionX = []
functionY = []
for i in range(0, count):
    lines = open('./{0}.{1}.csv'.format(Label, i), 'r')
    isHeader = True
    for line in lines:
        tokens = line.strip().split(delimiter)
        if (isHeader):
            isHeader = False
        else:
            functionX.append(int(tokens[0]))
            functionY.append(int(tokens[1]))
    plotfunc(Label, functionX, functionY, str(i))
    functionX.clear()
    functionY.clear()

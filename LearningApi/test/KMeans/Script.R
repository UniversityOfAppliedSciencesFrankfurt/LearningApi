#install.packages("rgl")
library(rgl)

# functionsPlots is a function that plots 2d/3d functions. It loads the desired file, plots functions from startFunction to endFunction
#   filepath: path to file containing the functions
#   functionDimensions: 2 or 3 for 2d and 3d respectively
#   startFunction: the number of the first function to plot from file
#   endFunction: the number of the last function to plot from file
functionsPlots = function(filepath, functionDimensions, startFunction, endFunction) {

    # creating different colors
    color = rainbow(endFunction - startFunction + 1)

    # load all functions from file
    myFunctions = read.csv(file = filePath, header = FALSE)

    # 2 dimensions
    if (functionDimensions == 2) {
        # get x-coordinates and y-coordinates
        for (i in startFunction:endFunction) {
            x = Vectorize(myFunctions[2 * i - 1,])
            y = Vectorize(myFunctions[2 * i,])
            # plot
            if (i == startFunction) {
                # new 2d plot
                par(mar = c(5, 5, 5, 10))
                plot.default(x, y, type = "l", lwd = "2", col = color[i - startFunction + 1])
            }
            else {
                # add to 2d plot
                lines.default(x, y, type = "l", lwd = "2", col = color[i - startFunction + 1])
            }
        }
        # add legend
        legend("topright", legend = paste('Function', startFunction:endFunction), pch = 16, col = color, inset = c(-0.50, 0), xpd = TRUE)
    }
    # 3 dimensions
    if (functionDimensions == 3) {
        # get x-coordinates, y-coordinates and ´z-coordinates
        for (i in startFunction:endFunction) {
            x = Vectorize(myFunctions[3 * i - 2,])
            y = Vectorize(myFunctions[3 * i - 1,])
            z = Vectorize(myFunctions[3 * i,])
            # plot
            if (i == startFunction) {
                # new 3d plot
                par3d(windowRect = c(100, 100, 700, 700))
                plot3d(x, y, z, type = "l", lwd = "2", col = color[i - startFunction + 1])
            }
            else {
                # add to 3d plot
                lines3d(x, y, z, lwd = "2", col = color[i - startFunction + 1])
            }
        }
        # add legend
        legend3d("topright", legend = paste('Function', startFunction:endFunction), pch = 16, col = color)
    }
}

# centroidsPlots is a function that plots 2d/3d centroids of similar functions.
#   filepath: path to file containing the functions
#   numClusters: number of clusters used in training
centroidsPlots = function(filepath, numClusters) {

    # creating different colors
    color = rainbow(numClusters)

    # load all functions from file
    myCentroids = read.csv(file = filePath, header = FALSE)

    numFunctions = NROW(myCentroids) / numClusters
    dimensions = NCOL(myCentroids) - 1

    # 2 dimensions
    if (dimensions == 2) {
        # get x-coordinates and y-coordinates
        x = myCentroids$V1
        y = myCentroids$V2
        # set color vector
        colV = c(1:NROW(myCentroids))
        for (i in 1:numFunctions) {
            colV[i] = color[numClusters - (i %% numClusters)]
        }
        # new 2d plot
        par(mar = c(5, 5, 5, 10))
        plot.default(x, y, type = "p", col = colV[1:numFunctions])
        # add legend
        legend("topright", legend = paste('Cluster', 1:numClusters), pch = 16, col = colV[1:numClusters], inset = c(-0.50, 0), xpd = TRUE)
    }
    # 3 dimensions
    if (dimensions == 3) {
        # get x-coordinates, y-coordinates and ´z-coordinates
        for (i in startFunction:endFunction) {
            # get x-coordinates and y-coordinates
            x = myCentroids$V1
            y = myCentroids$V2
            z = myCentroids$V3
            # set color vector
            colV = c(1:NROW(myCentroids))
            for (i in 1:numFunctions) {
                colV[i] = color[numClusters - (i %% numClusters)]
            }
            # new 3d plot
            par3d(windowRect = c(100, 100, 700, 700))
            plot3d(x, y, z, type = "p", col = colV[1:numFunctions])
        }
        # add legend
        legend3d("topright", legend = paste('Cluster', 1:numClusters), pch = 16, col = colV[1:numClusters])
    }
}

### Plot a Function

# settings for plotting similar functions
filePath = "C:/Users/skiwa/Desktop/Thesis/Functions/F2/NRP10/F2 SimilarFunctions NRP10.csv"
functionDimensions = 3
#filePath = "~/KMeans/TestFiles/TestFile01.csv"
#functionDimensions = 2
startFunction = 5
endFunction = 13

# plot the function
#functionsPlots(filepath, functionDimensions, startFunction, endFunction)

### Plot centroids

# settings for plotting centroids of similar functions
filePath = "C:/Users/skiwa/Desktop/Thesis/Functions/F2/NRP10/F2 SimilarFunctions Centroids NRP10 KA2 C2 I500 R1.csv"
numClusters = 2

# plot the centroids
centroidsPlots(filePath, numClusters)


# Canny Edge Detection:
Canny edge detector is an edge detecting algorithm which uses a multi stage algorithm to detect a wide range of edges in images. 
## The criteria for the edge detection includes:
1.	Edge detection with low error rate, that means it should catch as many edges as possible in the given image.
2.	It localize on the center of the edge from the detected edge point.
3.	A given edge in the image are marked once and does not create false edges.

This algorithm follows the strict defined methods providing good and reliable detection 
## Process Procedure.
1.	Gaussian filter is applied to smooth the image in order to remove the noise
2.	Find the intensity gradient of the image
3.	Spurious response to detect the edges are taken care by applying the non/maximum suppression
4.	Potential edges are detected using double threshold 
5.	Strong edges are finalized.


~~~csharp
private int[,,] Vertical(Int16[,,] input)
        {
            Int16[,,] gray = grayscale(input);
            Int16[,,] data = median(gray);

            int[,,] result = new int[data.GetLength(0),data.GetLength(1),3];
         
                    int sum = 0;

                    for (int u = 0; u < 9; u++)
                        sum = sum + mask[u];

                    long r1 = Convert.ToInt64(sum);
                    long g1 = Convert.ToInt64(sum);
                    long b1 = Convert.ToInt64(sum);
                    g1 = Convert.ToInt64(g1 << 8);
                    b1 = Convert.ToInt64(b1 << 16);
                    int ans = Convert.ToInt16(((byte)r1 | (byte)g1 | (byte)b1));
                    result[ii, jj, 0] = ans;
                    result[ii, jj, 1] = ans;
                    result[ii, jj, 2] = ans;
                }
            }
   ~~~

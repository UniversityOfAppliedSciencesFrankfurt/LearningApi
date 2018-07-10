# Self Organizing Map

A self- organizing map is a clustering technique to help to uncover catego-ries in large datasets. It is a special type of unsupervised neural net-works, where neurons are arranged in a single and 2-dimentional grid, which are arranged in the shape of rectangles or hexagons. 
Through multiple iterations, neurons in the grid will gradually join to-gether around the areas with high density of data points. So, the areas with many neurons will form a cluster in the data. As the neurons move, they inadequately bend and twist the grid to more closely influence the overall shape of the data. 
![alt text](https://raw.githubusercontent.com/username/projectname/branch/path/to/img.png)
## How does it work :
SOM includes neurons in grid, which gradually adapt to the intrinsic shape of the data. The result allows visualizing data points and identifying clusters in a lower dimension. 
### SOM follows the below steps in iterative process:
**Step 0**: Randomly position the grid’s neurons in the data space.

**Step 1**: Select one data point, either randomly or systematically cy-cling through the dataset in order

**Step 2**: Find the neuron that is closest to the chosen data point. This neuron is called the Best Matching Unit (BMU).

**Step 3**: Move the BMU closer to that data point. The distance moved by the BMU is determined by a learning rate, which decreases after each itera-tion.

**Step 4**: Move the BMU’s neighbours closer to that data point as well, with farther away neighbours moving less. Neighbours are identified using a radius around the BMU, and the value for this radius decreases after each iteration.

**Step 5**: Update the learning rate and BMU radius, before repeating Steps 1 to 4. Iterate these steps until positions of neurons have been stabi-lized.

A list of food items are taken in a .csv file and it is name as Food.csv. this file is sent to the algorithm to create pattern, The code snippet is given below: 

StreamReader reader = File.OpenText(path + "\\SelfOrganizingMap\\Food.csv");
                ///<Summary>Ignore first line.
                reader.ReadLine();
                while (!reader.EndOfStream)
                {
                    string[] line = reader.ReadLine().Split(',');
                    labels.Add(line[0]);
                    double[] inputs = new double[dimensions];

                    for (int i = 0; i < dimensions; i++)
                    {
                        inputs[i] = double.Parse(line[i + 1]);
                    }
                    patterns.Add(inputs);
                }
                reader.Dispose();
                return patterns;
  });
            api.AddModule(new Map(3, 10, 0.000001));
            var r = api.Run() as Neuron[];

            

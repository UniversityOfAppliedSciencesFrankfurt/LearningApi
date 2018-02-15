When training the LearningApi pipeline with huge amount of data, the batching operation can be used.
Example of batching can be find in [this file](/LearningApi/test/Perceptron/PerceptronUnitTests.cs)

To implement batch operations, you should be aware of following:
- The first module in the pipeline must implement batching correctlly
- The training is started by *RunBatch()* method instead of *Run()* method.

The data provider module returns the data as usual. As long there is more data to return, module should return batch of data.
The method *RunBatch()* will invoke data provider module after training of the batch over and over again, as long the data provider returns a none empty data set. Additionally the module must set IsMoreDataAvailable on true of the context instance, when there is no more data to retrieve.
Following example demonstrates how to do this:

~~~ csharp
                // ... code omitted for simplicity...
                
                if (currentBatch < maxSamples / batchSize)
                {
                    List<double[]> batch = new List<double[]>();

                    batch.AddRange(data.Skip(currentBatch * batchSize).Take(batchSize));

                    ctx.IsMoreDataAvailable = true;

                    currentBatch++;

                    return batch.ToArray();
                }
                else
                {
                    // We return here when there is no more data.
                    // In this case set ctx.IsMoreDataAvailable on false and return null.
                    ctx.IsMoreDataAvailable = false;
                    return null;
                }
  ~~~

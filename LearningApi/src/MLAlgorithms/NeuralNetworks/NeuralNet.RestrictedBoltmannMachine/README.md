# RestrictedBoltzmannMachine Api
A Restricted Boltzmann machine (RBM) is a restricted bidirectionally connected networks of stochastic processing units,
 which can be interpreted as neural network models.
RBM can also act as a learning component that are composed to form Deep Belief Network.
Restricted Boltzmann Machine are usually trained using the Contrastive Divergence learning procedure, 
and require a certain amount of trials in order to decide the most effective values of numerical meta-parameters
such as the learning rate, the momentum, the decays.
In addition, it is useful to monitor the learning process and decide when to terminate the process.

A RBM as two set of nodes: the visibles and the hidden. 
Each set of nodes can act as either inputs or outputs relatively to the other set.
Each node has a value of zero or one and these values are calculated using probability approximation.
In this RBM API, visible nodes act as input, while the hiddne nodes provide output value.

The input is provide in the following format, which is an array of binary number define the boolean values of the specific features.

```<language>
  sample[0] = new double[] { 1, 1, 1, 0, 0, 0 };
```
> The above sample provides the values of 6 features.

To use the API, user must first provide the training sample, then decide the values of the meta-parameters of the API.

After that, user can call the algorithm and run the training process

```<language>
api.UseRestrictedBoltzmannMachine(InputsCount, HiddenNeurons, Iteration, LearningRates, Momentums, Decays);
IScore score = api.Run() as IScore;
```
To evaluate the data, after providing the test data set, user can use the predict function as follow
```<language>
 var m_testresult = api.Algorithm.Predict(m_testdata, api.Context);
```

The "Predict" function output provides two meanings.
The output will provide the "class", which contains the highest appearance probability features in the sample data,
and the probability that the test sample belongs to that specific "class".

In the results of the predict function, the number before the period symbol provide the ordinal number of the "class" which the test sample belongs to,
while the number after the period symbol provide the probability.
Take an example:

```<language>
 results[0] = 0.99955591710818992
 results[1] = 1.9997051197039344
```

> The first test data belongs to "class" 0 at 99.955%, while the second test data belongs to "class" 1, at 99.97 %




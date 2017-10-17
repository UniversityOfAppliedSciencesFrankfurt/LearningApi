# RestrictedBoltzmannMachine Api
Machine learning theoretical description of a Restricted Boltzmann Machine (RBM) is a restricted bidirectionally connected networks of stochastic processing units,
 which can be interpreted as neural network models.
RBM can also act as a learning component that are composed to form Deep Belief Network.
Restricted Boltzmann Machine are usually trained using the Contrastive Divergence learning procedure, 
and require a certain amount of trials in order to decide the most effective values of numerical meta-parameters
such as the learning rate, the momentum, the decays.
In addition, it is useful to monitor the learning process and decide when to terminate the process.

The RBM API is, in one way, can be described as a classification and categorized algorithm.
The training process of RBM will auto generate a number of categories.
Each category is a group of features that can describe approximately a large number of training sample.

> For example, the training dataset is a group of people, which need to be categorized in "man" and "woman". 
>
> The training phase will allow RBM automatically detetermine which and how many features are needed for each "category", which is "gender" in this case.
> The category "man" usually contains features "short hair", "big muscle", "tall", "no breast", "low voice", et cetera...
>
> When testing with a person, by checking if the person has a satisfied number of features, this algorithm will predict the gender of the person in question.
> The larger the number of features details in the training phase, the more exact this algorithm's prediction.


As an exmple how to use the algorithm, I will demonstrate step by step the Simple test.

Imagining you have 6 training objects. You wish to divide them into 2 unspecific and autogenerate classes, no matter what kind.
And you know they can be categoried by using 6 predefined features.

RBM will put each object through 6 feature tests and get the answer either "Yes" or "No" if the specific feature exists.
We will label each feature "A","B","C","D","E","F". We will also set 1 as "Yes" for a feature that the object contains, and 0 as "No".
Put them together in table with the collum is the object number, while the rows describe the value of the specific feature , we will have a training dataset like this:

> - Object    || A || B || C || D || E || F ||
> 
> -  [1]      || 1 || 1 || 1 || 0 || 0 || 0 ||
> 
> -  [2]      || 1 || 0 || 1 || 0 || 0 || 0 ||
> 
> -  [3]      || 1 || 1 || 1 || 0 || 0 || 0 ||
> 
> -  [4]      || 0 || 0 || 1 || 1 || 1 || 0 ||
> 
> -  [5]      || 0 || 0 || 1 || 1 || 0 || 0 ||
> 
> -  [6]      || 0 || 0 || 1 || 1 || 1 || 0 ||
> 
 
In programming language, the training dataset is formatted as following: 

```<language>
sample[0] = new double[] { 1, 1, 1, 0, 0, 0 };
sample[1] = new double[] { 1, 0, 1, 0, 0, 0 };
sample[2] = new double[] { 1, 1, 1, 0, 0, 0 };
sample[3] = new double[] { 0, 0, 1, 1, 1, 0 };
sample[4] = new double[] { 0, 0, 1, 1, 0, 0 };
sample[5] = new double[] { 0, 0, 1, 1, 1, 0 };
```


After providing the training dataset, user then need to decide the values of the meta-parameters of the API, including:

```<language>
int InputsCount = 6; // The number of available features

int HiddenNeurons = 2; // The number of classes that need to be separated into

int Iteration = 15000; // The number of training cycle

double LearningRates = 0.15; // The learning rate of the machine

double Momentums = 0.9; // The momentum of the machine

double Decays = 0.02; // The machine decay rate
```

Now user can call the algorithm and run the training process

```<language>
api.UseRestrictedBoltzmannMachine(InputsCount, HiddenNeurons, Iteration, LearningRates, Momentums, Decays);
IScore score = api.Run() as IScore;
```


When the training process finished, in the API's score, the vector value of the class will be provided in the same format as the training sample 
```<language>
class[0] = { 0, 0, 1, 1, 0, 0 };
class[1] = { 1, 1, 1, 0, 0, 0 };
```

Translate it to the same table, this is the result

> - Class     || A || B || C || D || E || F ||
> 
> -  [1]      || 0 || 0 || 1 || 1 || 0 || 0 ||
> 
> -  [2]      || 1 || 1 || 1 || 0 || 0 || 0 ||
> 
According to the table, one class contains features  "A","B","C", while the other contains features "C" and "D".

In other word, after finished the training process, the algorithm now able to determine in which category, or "class", that the incoming test object belongs to.

To check the algorithm, we will let it predict some test objects with their feature values:

> - Object    || A || B || C || D || E || F ||
> 
> -  [1]      || 1 || 1 || 1 || 0 || 0 || 0 ||
> 
> -  [2]      || 0 || 0 || 0 || 1 || 1 || 1 ||

Just by looking, we can see clearly object [1] will belongs to class [2], while object [2] should lie in class [1]. 
Now we need the machine prediction result to compare. First, input the objects feature values:

```<language>
double[][] m_testdata = new double[m_testcount][2];
m_testdata[0] = new double[] { 1, 1, 1, 0, 0, 0 };
m_testdata[1] = new double[] { 0, 0, 0, 1, 1, 1 };
```

After that , we can use the "Predict" function as following:
```<language>
 var m_testresult = api.Algorithm.Predict(m_testdata, api.Context);
```

In the results of the "Predict" function, the number before the "." symbol provide the ordinal number of the "class" which the test sample belongs to,
while the number after the "." symbol provide the probability.
Let check the result of the above operation:

```<language>
 results[0] = 1.99955591710818992
 results[1] = 0.9997051197039344
```

> The first test obejct belongs to "class" 1 with the probability at 99.955%, while the second test object belongs to "class" 1, at 99.97 %

So, the algorithm prediction is just as we guess. This proves the API is working good.

In conclusion, Restricted Boltzmann Machine is able to automatically create a compressed representation for a large group of data.
In practice, Deep Belief Network is a lot of RBM stacking upon each other. To put it simple, Deep Belief Network is a multi-layer categorized machine.
Increasing the number of "categorized" time, no matter how large a data, it can be divide into multiple classes each layer stack upon layer in training process.
Testing process is just simply reverse tracing back to the root of origin. 






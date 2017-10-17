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

> A practical example for the training and testing process is "gender" recognition based on observing a large group of people.
> The training dataset will be a large group of people with a close ratio between man and woman.
> The machine is required to divide - or categorize the training dataset in two classes. 
> A number of features- or traits - that is critical to determine the different between the two genders must be predefined. For example: voice pitch (high/low), hair (long/short), neck(long/short), height (tall/short), et cetera...
>
> In the training phase, by checking each training person using the above features, the machine will recognize that two certain group of features has very high appearance probability.
> Due to using unsupervise training, the machine will not know which group is "man" and which group is "woman".
> After the training phase, the machine know that each class will have a set of significant features that can be used to determine the upcoming test person.
>
> When testing with a person, by checking if the person satisfied a number of features that belong to a class, the machine can predict the class of the person, which is either "man" or "woman" in this case.
> It can also provide the probability of the person in question belongs to each class.
>


The following is an example of how to use the API:

Imagining you have 6 training objects. You wish to divide them into 2 unspecific and autogenerate classes, no matter what kind.
And you know they can be categorized by using 6 predefined features.

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






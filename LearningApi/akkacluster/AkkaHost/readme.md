
 Work Package 3	Analysis of Actor Model in context of AI/ML algorithms
Task	Exploration and analysis of several AI/ML algorithms from the point of view of horizontal scale (multiple nodes) and Actor Model. 
Output	-First prototype and proposal for scale based on actor model on example of an algorithm.
Time frame	Sept.-Dec. 2018

The goal of this work is to propose how to design and implement a machine learning algorithm, which can horizontally scale by using of actor programming model. This programing model is based on mathematical theory of computation, which is already implemented as a framework in many programming languages and on many platforms. By following this approach scalability of ML algorithms can be implemented on commodity (lower price) hardware by using well established frameworks.

#### What is AKKA.Host?
*AkkaHost* is the AKKA.NET based Cluster Host, which is used to horizontally scale computation of actors based on Akka.Net framework. Once the cluster is running akka.net client components can remotely instantiate actors in the *AkkaHost* cluster.

#### How to run cluster nodes ?
Open LearningAPI solution and build the project *AkkaHost*. Open command prompt and navigate to output folder. Folder might look like: 
~~~
C:\dev\git\LearningApi\LearningApi\akkacluster\AkkaHost\bin\Debug\netcoreapp2.1
~~~
Copy output of the folder to the machine, which should represent a single node in cluster. Repeat this for every node in the cluster. After you have copied (installed) the code to all required nodes, you will have to decide which nodes will be seed nodes in the cluster, before you can run it.
Assuming you want to run the cluster with 4 nodes Node1, Node2, Node3 and Node4. In this case you might chose two nodes to be seed nodes Node1 and Node2. In this case execute following command an all 4 nodes.
~~~
dotnet akkahost.dll --port  8081 --seedhosts "Node1:8081, Node3:8081" --sysname ClusterSystem
~~~

For testing purposes cluster can be started on the single box (on one node). Following example demonstrate how to run two cluster nodes on the local machine. To make this possible cluster nodes must be bound to different ports. Commoand line below shows how to start to cluster seed nodes.

Runs seed node on ports 8081 and 8082:
~~~
dotnet akkahost.dll --port  8081 --seedhosts "localhost:8081, localhost:8082" --sysname ClusterSystem

dotnet akkahost.dll --port  8082 --seedhosts "localhost:8081, localhost:8082" --sysname ClusterSystem
~~~


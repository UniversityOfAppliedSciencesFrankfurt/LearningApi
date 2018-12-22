
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

### Deployment to ACI?

*Deployment single container instance:*
az container create -g RG-AKKAHOST --name akkahost --image damir.azurecr.io/akkahost:v4 --ports 8081 --ip-address Public --cpu 2 --memory 1 --dns-name-label akkahost1 --environment-variables port=8081 seedhosts=akkahost1.westeurope.azurecontainer.io:8081 sysname=ClusterSystem hostname=0.0.0.0 publichostname=akkahost1.westeurope.azurecontainer.io --registry-username damir --registry-password I9FRge/uvCysqAKIF4WvyVh7XWCNww2P 


*Deployment two continer instances*

az container create -g RG-AKKAHOST --name akkahost1 --image damir.azurecr.io/akkahost:v4 --ports 8081 --ip-address Public --cpu 2 --memory 1 --dns-name-label akkahost1 --environment-variables port=8081 seedhosts="akkahost1.westeurope.azurecontainer.io:8081,akkahost2.westeurope.azurecontainer.io:8081" sysname=ClusterSystem hostname=0.0.0.0 publichostname=akkahost1.westeurope.azurecontainer.io --registry-username damir --registry-password kkjOvM=J/sEyYTBW6TFltwuV5qXkVH70

az container create -g RG-AKKAHOST --name akkahost2 --image damir.azurecr.io/akkahost:v4 --ports 8081 --ip-address Public --cpu 2 --memory 1 --dns-name-label akkahost2 --environment-variables port=8081 seedhosts="akkahost1.westeurope.azurecontainer.io:8081,akkahost2.westeurope.azurecontainer.io:8081" sysname=ClusterSystem hostname=0.0.0.0 publichostname=akkahost2.westeurope.azurecontainer.io --registry-username damir --registry-password kkjOvM=J/sEyYTBW6TFltwuV5qXkVH70


### How to build docker image?
Open comman dprompt and navigate to folder C:\dev\git\LearningApi\LearningApi.
Run following command:
~~~
 docker build --rm -f "akkacluster/AkkaHost/Dockerfile" -t akkahost:v1 .
~~~

To run the container execute one of following commands:

~~~
docker run -it --rm -p:8081:8081 akkahost:v1 --port 8081 --sysname ClusterSystem --seedhosts=localhost:8081 --hostname=0.0.0.0 --publichostname=DADO-SR1
~~~

~~~
docker run -it --rm -p:8081:8081 akkahost:v1 --port 8081 --sysname ClusterSystem --seedhosts=DADO-SR1:8081 --hostname=0.0.0.0 --publichostname=DADO-SR1
~~~

### Debug Arguments

Paste this in debug properties and hit F5 to run the host:

~~~
--port  8081  --sysname ClusterSystem --seedhosts=localhost:8081 --hostname=localhost --publichostname=localhost
~~~

~~~
--port  8081  --sysname ClusterSystem --seedhosts=DADO-SR1:8081 --hostname=0.0.0.0 --publichostname=DADO-SR1
~~~


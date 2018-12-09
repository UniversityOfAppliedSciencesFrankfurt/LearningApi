
#### How to run nodes from command line?

Runs seed node on 8081.
dotnet akkahost.dll --port  8081 --seedhosts "localhost:8081, localhost:8082" --sysname ClusterSystem
 
Runs another seed node on 8081
dotnet akkahost.dll --port  8082 --seedhosts "localhost:8081, localhost:8082" --sysname ClusterSystem
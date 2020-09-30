# My Exercises


## Exercise I

**Azure Account Confirmation**

The azure account was accessed with the  az login command:	`az login`
	
The default azure subscription details are as below:

> 		[
> 		  {
> 			"cloudName": "AzureCloud",
> 			"homeTenantId": "66c5e13f-8c43-4359-b2e8-51775c6d298d",
> 			"id": "2f682baa-1721-4f09-8203-d661d7c1443e",
> 			"isDefault": true,
> 			"managedByTenants": [],
> 			"name": "Azure for Students",
> 			"state": "Enabled",
> 			"tenantId": "66c5e13f-8c43-4359-b2e8-51775c6d298d",
> 			"user": {
> 			  "name": "arinze.okpagu@stud.fra-uas.de",
> 			  "type": "user"
> 			}
> 		  }
> 		]

The [Azure CLI][2] command `az account list` is also used to show the list of subscriptions for the logged in account.

The `az account set` is used to set a preferred subscription to be the current active subscription if a different subscription was set to default:

```
az account set --subscription 2f682baa-1721-4f09-8203-d661d7c1443e

```

Here, the `2f682baa-1721-4f09-8203-d661d7c1443e` is the id of the preferred subscription.

**Docker Installation & Verification**

After installation of [docker desktop][1], the version of docker could be verified by running the `docker version` command:

```
Client: Docker Engine - Community
 Version:           19.03.12
 API version:       1.40
 Go version:        go1.13.10
 Git commit:        48a66213fe
 Built:             Mon Jun 22 15:43:18 2020
 OS/Arch:           windows/amd64
 Experimental:      false

Server: Docker Engine - Community
 Engine:
  Version:          19.03.12
  API version:      1.40 (minimum version 1.24)
  Go version:       go1.13.10
  Git commit:       48a66213fe
  Built:            Mon Jun 22 15:57:30 2020
  OS/Arch:          windows/amd64
  Experimental:     false

```

A test of the successful installation could be carried out by running the `hello-world` Docker image.

Below is the snippet of executing the `docker run hello-world` command:

```
docker run hello-world
Unable to find image 'hello-world:latest' locally
latest: Pulling from library/hello-world
dc9560809954: Pull complete   
1fde5d28e17a: Pull complete   
566994b78a4c: Pull complete  
Digest: sha256:49a1c8800c94df04e9658809b006fd8a686cab8028d33cfba2cc049724254202
Status: Downloaded newer image for hello-world:latest

Hello from Docker!
This message shows that your installation appears to be working correctly.
...

```

Running `docker image ls` command would list the hello-world image just downloaded as follows:

```
docker image ls
REPOSITORY                              TAG                   IMAGE ID            CREATED             SIZE
hello-world                             latest                b98257792ecf        3 weeks ago         251MB
mcr.microsoft.com/dotnet/core/runtime   2.1-nanoserver-1903   36b980a7d6c7        3 months ago        326MB

```

## Exercise 2 - Docker in Azure

**Working with Images**

Running the `docker pull` command with the image name is used to retrieve an image of choice from the Docker registry.
It saves the image on the local machine and makes it available for running containers.

The exercise was carried out using a simple ASP.NET Core web app image; *mcr.microsoft.com/dotnet/core/samples:aspnetapp*

```
docker pull mcr.microsoft.com/dotnet/core/samples:aspnetapp
```

Upon successful fetching of the image, the `docker tag SOURCE_IMAGE[:TAG] TARGET_IMAGE[:TAG]` command is used 
to create relevant or user-friendly tag for the image. This  step is optional as the final image to be pushed would
need be tagged appropriately according to the destination repository.

```
docker tag  mcr.microsoft.com/dotnet/core/samples:aspnetapp exercise2:v2
```
The tagging creates a target image that refers to the source image. Using the new image, *exercise2:v2* to execute the `docker run` command with 
`--publish`  and `--detach` options would run the container instance in the background and publish the container's http port 80 to port 9090 of the local machine. 

```
docker run --publish 9090:80 --detach exercise2:v2
```

Running `docker ps` command is used to show the list of running containers and in this case;  *exercise2:v2* as follows:

```
docker ps
CONTAINER ID        IMAGE               COMMAND                  CREATED             STATUS              PORTS                  NAMES
76ed4c8edb59        exercise2:v2        "dotnet aspnetapp.dll"   7 seconds ago       Up 5 seconds        0.0.0.0:9090->80/tcp   pedantic_moore

```

To avoid containers running endlessly, running `docker stop` command with the specific container ID would terminate the running process.

**Pushing Image to Azure**

Before pushing image to Azure, a resource group and container registry needs to be created. 
I created my resoure group and contianer registry using the following commands:

```
az login
az acr create --resource-group RG_Docker --name TasksRegistry --sku Basic --admin-enabled true
```

Prior to pushing the image, the image needs to be tagged accordingly; `docker tag [IMAGE-ID] [registryserver]/[repositoryname]:[tagname]` to refer to the  appropriate registry server uisng the commands:

```
docker tag exercise2:v2 tasksregistry.azurecr.io/rinzx/cloud_computing_ss2020:exercise2
```
Here, *registryserver* = **tasksregistry.azurecr.io**, *repository name* = **rinzx/cloud_computing_ss2020** and *tagname* = **exercise2**

Upon successful tagging, I executed the command; `docker push [registryserver]/[repositoryname]:[tagname]>`, to push to azure registry.

```
docker push tasksregistry.azurecr.io/rinzx/cloud_computing_ss2020:exercise2
```


**Pushing Image to DockerHub**

To successfully push to docker hub registry, run the `docker login -u [USERNAME]` command and type in the password:

```
docker login -u rinzx
Password:
Login Succeeded
```

This is followed by running the tag `docker tag [IMAGE-ID] [Docker ID]/[Repository Name]:[tagname]` and push command; `docker push [Docker ID]/[Repository Name]:[tagname]` respectively:

```
docker tag exercise2:v2 rinzx/cloud_computing_ss2020:exercise2
docker push rinzx/cloud_computing_ss2020:exercise2
```

The links to repository for the **Images** and  web app for this exercise are listed as follows: 
1. [MyDockerhub][3]
2. [MyAzure]
3. [Web App][4]

The images can pulled from any of the repository using the following commands:
```
docker pull rinzx/cloud_computing_ss2020:exercise2
docker pull tasksregistry.azurecr.io/rinzx/cloud_computing_ss2020:exercise2
```

## Exercise 3 - Host a web application with Azure App service

The web application project was designed using the ASP.NET Core Web Application template in Visual Studio.

Visual Studio provisioned easy means to publish the application to Azure App Service upon design completion by clicking on the project and selecting `Publish` and following through the susbequent onscreen tasks and instructions

Please click [here][3] to visit the deployed web application or copy and paste the URL link below on a web browser:
```
https://rinzx-webtask-ex3.azurewebsites.net/
```
The Dockerfile and source code of the hosted application can be found here: [Exercise 3][6]

Adding Docker Support to the wep application project in Visual Studio provides means to create a Dockerfile for the project that includes references to all required project for the final solution.

The next set of commands were respectively to achieve the following:
  1.  Build the docker image from the Dockerfile
  2.  Create a  new tag for the image
  3.  Log in to the Azure Container Registry
  4.  Push the image into Azure Container Registry

```

```

 

1. Provide the public URL of the webapplication.
2. Provide the URL to the source code of the hosted application. (Source code somwhere or the the docker image, or ??)
3. Provide URL to the docker file
4. Provide AZ scripts used to bublish the application.

#### Exercise 4 - Deploy and run the containerized app in AppService

1. Provide URL to the docker image of your application (Docker Hub / Azure Registry)
2. Provide the public URL to the running application. 

#### Exercise 5 - Blob Storage

Provide the URL to to blob storage container under your account.
We should find some containers and blobs in there.
Following are mandatory:
- Input
Contains training files

- Output
Contains output of the traned models

- 'Test' for playing and testing.
you should provide here SAS Url to 2-3 files in this container with time expire 1 year.

#### Exercise 6 - Table Storage

Provide us access to the account which you have used for table exersises.

#### Exercise 7 - Queue Storage

Provide us access to the account which you have used for queue exersises.



[1]: <https://www.docker.com/products/docker-desktop> 
[2]: <https://docs.microsoft.com/en-us/cli/azure/reference-index?view=azure-cli-latest>
[3]: <https://hub.docker.com/r/rinzx/cloud_computing_ss2020/tags>
[4]: <https://webtask1.azurewebsites.net/>
[5]: <https://rinzx-webtask-ex3.azurewebsites.net/>
[6]: <https://github.com/UniversityOfAppliedSciencesFrankfurt/se-cloud-2019-2020/tree/Arinze.Okpagu/MyWork/Cloud_Computing%20Exercises/Exercise_3/Exercise_3_Web_Application>
[7]: <>
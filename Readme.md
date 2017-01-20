# Microservice Code Template [![Build Status](https://travis-ci.org/PageUpPeopleOrg/microservice-bootstrap.svg?branch=master)](https://travis-ci.org/PageUpPeopleOrg/microservice-bootstrap)

A git repo that gives you a headstart on your own Microservice in [Dotnet Core](https://www.microsoft.com/net/core).

*_The intention is reduce the entry barrier in going with Microservices for .NET folks._*

The repo will help you to bootstrap two kind of Microservices,
* Web service
* Worker service

This will help you have a head start with,

1. Running a ASP.NET Microservice
2. Containerizing your service with Docker
3. Head start with CI / CD with Travis CI
4. Consume basic Kinesis event data streams

## Get Started

1. Clone the repo
2. Ensure you have Docker toolbox installed. Good news, you don't even need asp.net vNext installed.
3. Setup environment variables (see below).
4. Set unique Kinesis stream name and worker id (see below).
5. Run `docker-compose up -d` from root folder of repo.
6. Yay all up and running, Done!

### Kinesis 

The code integrates with Kinesis by taking the source of the third-party library, KinesisNet, and updating it to be compatible with the .NET Core framework.

To begin consuming events, a KManager object is created using the appropriate environment variables and defining a unique workerId and kinesisStreamName. We then call kManager.Consume.Start by passing in an IRecordProcessor.

When a bad message comes through ... (To continue)

### Setup environment variables

The following environment variables are required:
* AWS_ACCESS_KEY_ID
* AWS_SECRET_ACCESS_KEY
* AWS_SESSION_TOKEN
* ENV
* DC
* REGION

The AWS environment variables can be obtained from the Amazon using the Power User Account.

The other variables will be obtained automatically during deployment. If you want to run locally (ie. Visual Studio), you must set these environment variables for your OS.

### What if I just want worker service?
* Remove WebService & WebServer.UnitTests projects from Visual Studio and delete the folder
* Remove its references from docker compose file
* Remove sections of travis yml referring to web service

### What if I just want Web Service?
* Remove WorkerService & WorkerService.UnitTests projects from Visual Studio and delete the folder.
* Remove its references from docker compose file
* Remove sections of travis yml referring to worker service

### How to ensure it is all good

Head to `http://192.168.99.100:4000/api/values` to see to web service running. Pat your own back!

Run `docker logs microservicebootstrap_workerservice_1` and see "Hello world" to confirm, your worker service is running. Pay your own back again!

### Deployment

Uncomment the relevant lines in deploy.sh to get DC2-7 deployment running.

## TODO

1. include basic dependency injection
2. Create a dotnet core branch - for enthusiasts
3. Include code coverage & complexity metrics
4. document: explain Dynamo table for current state storage of kinesis integration
5. document: explain what happens when a bad kinesis message comes in
6. document: missing features from LaunchPad TuneUp

## Template idea

The service is designed in mind considering the Principles of Microservices as documented by Sam Newman.

It's a [good video to watch](https://vimeo.com/131632250), if you haven't already.

#### Principles of Microservices, by Sam Newman

![Sam Newman's Principles of Microservices](https://raw.githubusercontent.com/PageUpPeopleOrg/microservice-bootstrap/master/principles.png "Principles of Microservices, by Sam Newman")

### Deployment story
The template provides way to package the service as Docker container and uses Travis CI for CI.
However, I won't go into
1. how to manage the containers or
2. how to deploy the containers or
3. how to set up &/ manage docker cluster.

An option is to use Aws ECS / ECR to managing docker cluster and found it to be working (especially when your infra is hosted on aws).

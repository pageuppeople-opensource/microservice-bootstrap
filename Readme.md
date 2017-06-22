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
2. Ensure you have [Docker installed](https://store.docker.com/search?offering=community&type=edition).
3. Have Visual studio 2017 or any other dotnet compatible IDE handy
5. Run `docker-compose up` from root folder of repo.
6. Yay all up and running, Done!

### What if I just want worker service?
* Remove WebService & WebServer.UnitTests projects from Visual Studio and delete the folder
* Remove its references from docker compose file
* Remove sections of travis yml referring to web service

### What if I just want Web Service?
* Remove WorkerService & WorkerService.UnitTests projects from Visual Studio and delete the folder.
* Remove its references from docker compose file
* Remove sections of travis yml referring to worker service

### How to ensure it is all good

#### For Web service
Head to `http://localhost:4001/healthcheck` to see to web service running. Pat your own back!

#### For Worker service
Run `docker-compose logs` and see "Hello world" to confirm, your worker service is running. Pay your own back again!

#### Kinesis consumer for Worker service
Having a kinesis consumer for worker service is an work in progress.
Current idea is to see if we can have kinesis consumer that can be injected into the worker service using something like @sbarski [KinesisNet](https://github.com/sbarski/KinesisNet). It is supporting core yet, but if you are intersested, please head over to [#29](/../../issues/29).

### Deployment (Need to ensure this is working.. work in progress..)

Uncomment the relevant lines in deploy.sh to get DC2-7 deployment running.

## TODO

1. include basic dependency injection
2. Include code coverage & complexity metrics

## Template idea

The service is designed in mind considering the Principles of Microservices as documented by Sam Newman.

It's a [good video to watch](https://vimeo.com/131632250), if you haven't already.

#### Principles of Microservices, by Sam Newman

![Sam Newman's Principles of Microservices](https://raw.githubusercontent.com/PageUpPeopleOrg/microservice-bootstrap/master/principles.png "Principles of Microservices, by Sam Newman")

### Deployment story
The template provides way to package the service as Docker container and uses Travis CI for CI.

An option is to use Aws ECS / ECR to managing docker cluster and found it to be working (especially when your infra is hosted on aws).

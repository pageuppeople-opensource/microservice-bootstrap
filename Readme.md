# Microservice bootstrap

A git repo that gives you a headstart on your own Microservice in ASP.NET vNext stack.

*_The intention is reduce the entry barrier in going with Microservices for .NET folks._*

The repo will help you to bootstrap two kind of Microservices,
* Web service
* Worker service

The repo in its current form supports dnx451 (_meaning it will be running on top of mono, if used with docker_).

This will help you have a head start with,

1. Running a ASP.NET Microservice
1. Containerizing your service with Docker
2. Head start with CI / CD with Travis CI

## Get Started

1. Clone the repo
2. Ensure you have Docker toolbox installed. Good news, you don't even need asp.net vNext installed.
3. Run `docker-compose up -d` from root folder of repo.
4. Yay all up and running, Done!

### What if I just want worker service?
* Remove WebService & WebServer.UnitTests projects from Visual Studio and delete the folder
* Remove its references from docker compose file

### What if I just want Web Service?
* Remove WorkerService & WorkerService.UnitTests projects from Visual Studio and delete the folder.
* Remove its references from docker compose file

### How to ensure it is all good

Head to `http://192.168.99.100:5000/api/values` to see to web service running. Pat your own back!

Run `docker logs microservicebootstrap_workerservice_1` and see "Hello world" to confirm, your worker service is running. Pay your own back again!

## TODO

1. include logging
2. include basic dependency injection
5. include travis ci process
4. Visual studio support

## Bootstrap idea

The service is designed in mind considering the Principles of Microservices as documented by Sam Newman.

It a [good video to watch](https://vimeo.com/131632250), if you haven't already.

#### Principles of Microservices, by Sam Newman

![Sam Newman's Principles of Microservices](https://raw.githubusercontent.com/PageUpPeopleOrg/microservice-bootstrap/master/principles.png "Principles of Microservices, by Sam Newman")

### Deployment story
The template provides way to package the service as Docker container and uses Travis CI for CI.
However, I won't go into
1. how to manage the containers or
2. how to deploy the containers or
3. how to set up &/ manage docker cluster.

An option is to use Aws ECS / ECR to managing docker cluster and found it to be working (especially when your infra is hosted on aws).

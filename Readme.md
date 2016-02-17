# Microservice bootstrap

A git repo that you can clone and create your own Microservice in ASP.NET vNext stack.

The repo will support two types of services,
* web services
* worker service

The repo in its current form supports dnx451 (meaning it will be running in mono on production)

This will help you have a head start with,

1. Running a ASP.NET Microservice
1. Containerizing your service with Docker
2. Head start with CI / CD with Travis CI


## TODO

1. include logging
2. include basic dependency injection
3. Docker file has debian hardcoded, consider avoiding
4. aspnet vnext version rc-update1 vs rc-final
5. include travis ci 
6. docker-compose enable

## Service design

The service is designed in mind considering the Principles of Microservices as documented by Sam Newman.

It a [good video to watch](https://vimeo.com/131632250), if you haven't already.

#### Principles of Microservices, by Sam Newman

![Sam Newman's Principles of Microservices](https://raw.githubusercontent.com/PageUpPeopleOrg/microservice-bootstrap/master/principles.png "Principles of Microservices, by Sam Newman")

## Get Started

1. Clone the repo

### Bootstrapping worker service

### Bootstrapping web service

### Deployment story
The template provides way to package the service as Docker container and uses Travis CI for CI.
However, I won't go into
1. how to manage the containers or
2. how to deploy the containers or
3. how to set up &/ manage docker cluster.

An option is to use Aws ECS / ECR to managing docker cluster and found it to be working (especially when your infra is hosted on aws).

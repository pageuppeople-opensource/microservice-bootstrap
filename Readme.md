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

## Service design

The service is designed in mind considering the Principles of Microservices as documented by Sam Newman.

It a [good video to watch](https://vimeo.com/131632250), if you haven't already.

#### Diagram of Principles of Microservices, by Sam Newman

<img src="http://samnewman.io/talks/img/principles.png" alt="Sam Newman's Principles of Microservices"/>

## Get Started

1. Clone the repo

### Bootstrapping worker service

### Bootstrapping web service

## What it will NOT get into?

* Will not solve / recommend any DI framework
* Will not solve horizontal scalability
* Will not solve the deployment workflow.

Though, I have used and recommend Aws ECS / ECR to managing docker cluster and deployment.

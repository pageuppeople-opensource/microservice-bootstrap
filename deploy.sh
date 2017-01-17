#!/bin/bash

DC0_REPO_URL=047651431481.dkr.ecr.us-east-1.amazonaws.com

WEB_REPO_NAME=launchpad-web
WORKER_REPO_NAME=launchpad-worker

export AWS_DEFAULT_REGION="ap-southeast-2"

push_docker_image()
{
    echo "####### Pushing docker images"
    $(aws ecr get-login --region ap-southeast-2 ) # does docker login
    set -x

    echo "####### Pushing Web docker image"
    docker tag WEB_REPO_NAME:latest $DC0_REPO_URL/WEB_REPO_NAME:$BUILD_NO
    docker push $DC0_REPO_URL/WEB_REPO_NAME:$BUILD_NO
	
    echo "####### Pushing Worker docker image"
    docker tag WORKER_REPO_NAME:latest $DC0_REPO_URL/WORKER_REPO_NAME:$BUILD_NO
    docker push $DC0_REPO_URL/WORKER_REPO_NAME:$BUILD_NO
    set +x

    echo "####### Finished pushing docker images"
}

update_task_definition () {
   
    echo "####### Updating task definition with new version"
	#TODO: template cluster name "launchpad"
    `pwd`/ecs-deploy-update-task-definition -c launchpad -n $WEB_REPO_NAME -i $DC0_REPO_URL/$WEB_REPO_NAME -t 200 -r $AWS_DEFAULT_REGION -b $BUILD_NO

    `pwd`/ecs-deploy-update-task-definition -c launchpad -n $WORKER_REPO_NAME -i $DC0_REPO_URL/$WORKER_REPO_NAME -t 200 -r $AWS_DEFAULT_REGION -b $BUILD_NO

    echo "####### Finished updating task definition with new version"
}

wait_for_completion () {
   
    echo "####### Waiting for new version task to be up and running"

    `pwd`/ecs-deploy-wait-for-completion -c launchpad -n $WEB_REPO_NAME -i $DC0_REPO_URL/$WEB_REPO_NAME -t 200 -r $AWS_DEFAULT_REGION -b $BUILD_NO

    `pwd`/ecs-deploy-wait-for-completion -c launchpad -n $WORKER_REPO_NAME -i $DC0_REPO_URL/$WORKER_REPO_NAME -t 200 -r $AWS_DEFAULT_REGION -b $BUILD_NO

    echo "####### Task is running with new version"
}

deploy() {
    echo "####### Setting up DC0 credentials"
	export AWS_ACCESS_KEY_ID=$AWS_ACCESS_KEY_ID_DC0
    export AWS_SECRET_ACCESS_KEY=$AWS_SECRET_ACCESS_KEY_DC0
	
    echo "####### Pushing Docker images to DC0"
    push_docker_image
	
    echo "####### Update task definition for to DC0"
    update_task_definition

}

# dont deploy if we are building a pull request
if [ "$TRAVIS_PULL_REQUEST" != "false" ]; then
    echo "####### Not deploying on pull request"
    exit 0
fi

#deploy to all DC0 on master branch
if [ "$TRAVIS_BRANCH" == "master" ]; then
    deploy
    wait_for_completion
    exit 0
fi
#!/bin/bash


DC0_REPO_URL=047651431481.dkr.ecr.us-east-1.amazonaws.com
DC2_5_REPO_URL=342212725307.dkr.ecr.us-east-1.amazonaws.com
DC7_REPO_URL=356994454909.dkr.ecr.us-east-1.amazonaws.com

#TODO: template repo name
WEB_REPO_NAME=launchpad-web
WORKER_REPO_NAME=launchpad-worker

export AWS_DEFAULT_REGION="us-east-1"

push_docker_image()
{
    local REPO_URL=$1

    echo "####### Pushing docker images"
    $(aws ecr get-login --region us-east-1 ) # does docker login
    set -x

    echo "####### Pushing Web docker image"
    docker tag $WEB_REPO_NAME:latest $REPO_URL/$WEB_REPO_NAME:$BUILD_NO
    docker push $REPO_URL/$WEB_REPO_NAME:$BUILD_NO

    echo "####### Pushing Worker docker image"
    docker tag $WORKER_REPO_NAME:latest $REPO_URL/$WORKER_REPO_NAME:$BUILD_NO
    docker push $REPO_URL/$WORKER_REPO_NAME:$BUILD_NO
    set +x

    echo "####### Finished pushing docker images"
}

update_task_definition () {
    local REGION_ENDPOINT=$1
    local REPO_URL=$2

    echo "####### Updating task definition with new version"
	#TODO: template cluster name "launchpad"
    `pwd`/ecs-deploy-update-task-definition -c Launchpad-Team -n $WEB_REPO_NAME -i $REPO_URL/$WEB_REPO_NAME -t 200 -r $REGION_ENDPOINT -b $BUILD_NO

    `pwd`/ecs-deploy-update-task-definition -c Launchpad-Team -n $WORKER_REPO_NAME -i $REPO_URL/$WORKER_REPO_NAME -t 200 -r $REGION_ENDPOINT -b $BUILD_NO

    echo "####### Finished updating task definition with new version"
}

wait_for_completion () {
    local REGION_ENDPOINT=$1
    local REPO_URL=$2

    echo "####### Waiting for new version task to be up and running"
	#TODO: template cluster name "launchpad"
    `pwd`/ecs-deploy-wait-for-completion -c Launchpad-Team -n $WEB_REPO_NAME -i $REPO_URL/$WEB_REPO_NAME -t 200 -r $REGION_ENDPOINT -b $BUILD_NO

    `pwd`/ecs-deploy-wait-for-completion -c Launchpad-Team -n $WORKER_REPO_NAME -i $REPO_URL/$WORKER_REPO_NAME -t 200 -r $REGION_ENDPOINT -b $BUILD_NO

    echo "####### Task is running with new version"
}

deploy_to_all() {
	echo "####### Setting up DC0 credentials"
	export AWS_ACCESS_KEY_ID=$AWS_ACCESS_KEY_ID_DC0
	export AWS_SECRET_ACCESS_KEY=$AWS_SECRET_ACCESS_KEY_DC0

	echo "####### Pushing Docker images to DC0"
	push_docker_image $DC0_REPO_URL

	echo "####### Update task definition for to DC0"
	update_task_definition ap-southeast-2 $DC0_REPO_URL

	#TODO
	#echo "####### Setting up DC2 - DC5 credentials"
	#export AWS_ACCESS_KEY_ID=$AWS_ACCESS_KEY_ID_DC2_5
	#export AWS_SECRET_ACCESS_KEY=$AWS_SECRET_ACCESS_KEY_DC2_5
	#
	#echo "####### Pushing Docker images to DC2-DC5"
	#push_docker_image $DC2_5_REPO_URL
	#
	#echo "####### Update task definition for DC2-DC5"
	#update_task_definition ap-southeast-2 $DC2_5_REPO_URL
	#update_task_definition eu-west-1 $DC2_5_REPO_URL
	#update_task_definition us-east-1 $DC2_5_REPO_URL
	#update_task_definition ap-southeast-1 $DC2_5_REPO_URL
	#
	#echo "####### Setting up DC7 credentials"
	#export AWS_ACCESS_KEY_ID=$AWS_ACCESS_KEY_ID_DC7
	#export AWS_SECRET_ACCESS_KEY=$AWS_SECRET_ACCESS_KEY_DC7
	#
	#echo "####### Pushing Docker images to DC7"
	#push_docker_image $DC7_REPO_URL
	#
	#echo "####### Update task definition for DC7"
	#update_task_definition eu-west-1 $DC7_REPO_URL
}

wait_completion_for_all() {
   export AWS_ACCESS_KEY_ID=$AWS_ACCESS_KEY_ID_DC0
   export AWS_SECRET_ACCESS_KEY=$AWS_SECRET_ACCESS_KEY_DC0

   echo "###### Waiting on DC0"
   wait_for_completion ap-southeast-2 $DC0_REPO_URL

   #TODO
   #wait for all tasks to be up and running
   #echo "####### Setting up DC2 - DC5 credentials"
   #export AWS_ACCESS_KEY_ID=$AWS_ACCESS_KEY_ID_DC2_5
   #export AWS_SECRET_ACCESS_KEY=$AWS_SECRET_ACCESS_KEY_DC2_5
   #
   #echo "####### Waiting on DC2-DC5"
   #wait_for_completion ap-southeast-2 $DC2_5_REPO_URL
   #wait_for_completion eu-west-1 $DC2_5_REPO_URL
   #wait_for_completion us-east-1 $DC2_5_REPO_URL
   #wait_for_completion ap-southeast-1 $DC2_5_REPO_URL
	#
   #echo "####### Setting up DC7 credentials"
   #export AWS_ACCESS_KEY_ID=$AWS_ACCESS_KEY_ID_DC7
   #export AWS_SECRET_ACCESS_KEY=$AWS_SECRET_ACCESS_KEY_DC7
   #
   #echo "####### Wait for DC7"
   #wait_for_completion eu-west-1 $DC7_REPO_URL
}

# dont deploy if we are building a pull request
if [ "$TRAVIS_PULL_REQUEST" != "false" ]; then
    echo "####### Not deploying on pull request"
    exit 0
fi

#deploy to all DC0 on master branch
if [ "$TRAVIS_BRANCH" == "master" ]; then
    deploy_to_all
    wait_completion_for_all
    exit 0
fi

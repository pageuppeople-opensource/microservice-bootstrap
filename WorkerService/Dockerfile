FROM microsoft/aspnet:1.0.0-rc1-update1-coreclr

RUN mkdir -p /app

# Running dnu restore on projection.json in tmp folder.
# This is only a optimization.
# This optimization enables to cache the dnu restore layer by docker effectively.
# Essentially, this means, docker is only rebuild this layer when project.json is modified.
ADD project.json /tmp/project.json
RUN cd /tmp && dnu restore
RUN cp -a /tmp/project.lock.json /app

COPY . /app

WORKDIR /app
RUN ["dnu", "restore"]

ENTRYPOINT ["dnx", "-p", "project.json", "run"]

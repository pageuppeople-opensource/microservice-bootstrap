FROM microsoft/aspnet:1.0.0-rc1-update1

RUN printf "deb http://ftp.us.debian.org/debian jessie main\n" >> /etc/apt/sources.list
RUN apt-get -qq update && apt-get install -qqy sqlite3 libsqlite3-dev && rm -rf /var/lib/apt/lists/*

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

EXPOSE 5000/tcp
ENTRYPOINT ["dnx", "-p", "project.json", "web"]

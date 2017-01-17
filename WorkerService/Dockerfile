FROM microsoft/dotnet:1.1.0-sdk-projectjson

COPY . ./app
WORKDIR ./app
RUN dotnet restore

CMD ["dotnet", "run"]
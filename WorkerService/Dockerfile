FROM microsoft/aspnetcore-build:2 AS build-env

COPY . /app
WORKDIR /app
RUN dotnet publish -c Release -o /app/out

# Build runtime image
FROM microsoft/aspnetcore:2
WORKDIR /app

COPY --from=build-env /app/out ./

ENTRYPOINT ["dotnet", "WorkerService.dll"]

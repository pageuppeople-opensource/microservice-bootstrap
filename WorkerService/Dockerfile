FROM microsoft/dotnet:1.1.2-sdk

COPY . /app

WORKDIR /app

RUN dotnet restore

RUN dotnet build -c Release

CMD ["dotnet", "run", "-c", "Release"]
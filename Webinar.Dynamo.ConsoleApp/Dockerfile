#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/runtime:3.1-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Webinar.Dynamo.ConsoleApp/Webinar.Dynamo.ConsoleApp.csproj", "Webinar.Dynamo.ConsoleApp/"]
COPY ["Webinar.Dynamo.Repository/Webinar.Dynamo.Repository.csproj", "Webinar.Dynamo.Repository/"]
RUN dotnet restore "Webinar.Dynamo.ConsoleApp/Webinar.Dynamo.ConsoleApp.csproj"
COPY . .
WORKDIR "/src/Webinar.Dynamo.ConsoleApp"
RUN dotnet build "Webinar.Dynamo.ConsoleApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Webinar.Dynamo.ConsoleApp.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Webinar.Dynamo.ConsoleApp.dll"]

#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Webinar.Dynamo.LocationsApi/Webinar.Dynamo.LocationsApi.csproj", "Webinar.Dynamo.LocationsApi/"]
COPY ["Webinar.Dynamo.Domain/Webinar.Dynamo.Domain.csproj", "Webinar.Dynamo.Domain/"]
COPY ["Webinar.Dynamo.Repository/Webinar.Dynamo.Repository.csproj", "Webinar.Dynamo.Repository/"]
RUN dotnet restore "Webinar.Dynamo.LocationsApi/Webinar.Dynamo.LocationsApi.csproj"
COPY . .
WORKDIR "/src/Webinar.Dynamo.LocationsApi"
RUN dotnet build "Webinar.Dynamo.LocationsApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Webinar.Dynamo.LocationsApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Webinar.Dynamo.LocationsApi.dll"]
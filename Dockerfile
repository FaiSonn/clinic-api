FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ClinicApi/ClinicApi.csproj ClinicApi/
RUN dotnet restore ClinicApi/ClinicApi.csproj
COPY ClinicApi/ ClinicApi/
RUN dotnet publish ClinicApi/ClinicApi.csproj -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app .
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "ClinicApi.dll"]

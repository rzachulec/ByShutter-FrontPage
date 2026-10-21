FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR app/
COPY . ./

RUN dotnet restore
RUN dotnet publish -o out

FROM mcr.microsoft.com/dotnet/sdk:10.0
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "ByShutterBackend.Web.dll"]
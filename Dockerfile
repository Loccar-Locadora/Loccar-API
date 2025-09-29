# Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY *.sln ./
COPY LoccarApplication/*.csproj ./LoccarApplication/
COPY LoccarDomain/*.csproj ./LoccarDomain/
COPY LoccarInfra/*.csproj ./LoccarInfra/
COPY LoccarLocadora/*.csproj ./LoccarLocadora/
    
RUN dotnet restore

COPY . .

WORKDIR /src/LoccarLocadora
RUN dotnet publish -c Release -o /app

# Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app ./

EXPOSE 8080
ENTRYPOINT ["dotnet", "LoccarWebapi.dll"]

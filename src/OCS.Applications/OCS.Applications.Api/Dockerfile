FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS prepare
ENV PATH="$PATH:/root/.dotnet/tools"
RUN dotnet tool install --global --no-cache dotnet-subset
WORKDIR /src
COPY ["OCS.Applications.Api/OCS.Applications.Api.csproj", "OCS.Applications.Api/"]
COPY ["OCS.Applications.Api/OCS.Applications.Api.csproj", "OCS.Applications.Api/"]
COPY ["OCS.Applications.Domain/OCS.Applications.Domain.csproj", "OCS.Applications.Domain/"]
COPY ["OCS.Applications.Services/OCS.Applications.Services.csproj", "OCS.Applications.Services/"]
COPY ["OCS.Applications.Infrastructure/OCS.Applications.Infrastructure.csproj", "OCS.Applications.Infrastructure/"]
COPY ["OCS.Applications.Contracts/OCS.Applications.Contracts.csproj", "OCS.Applications.Contracts/"]

RUN dotnet subset restore "OCS.Applications.Api/OCS.Applications.Api.csproj" --root-directory /src --output restore_subset/

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY --from=prepare /src/restore_subset ./
RUN dotnet restore "OCS.Applications.Api/OCS.Applications.Api.csproj"

COPY . .
WORKDIR "/src/OCS.Applications.Api"
RUN dotnet build "OCS.Applications.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "OCS.Applications.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OCS.Applications.Api.dll"]
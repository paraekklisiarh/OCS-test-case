FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS prepare
ENV PATH="$PATH:/root/.dotnet/tools"
RUN dotnet tool install --global --no-cache dotnet-subset
WORKDIR /src
COPY ["OSC.Applications.Api/OSC.Applications.Api.csproj", "OSC.Applications.Api/"]
COPY ["OSC.Applications.Api/OSC.Applications.Api.csproj", "OSC.Applications.Api/"]
COPY ["OSC.Applications.Domain/OSC.Applications.Domain.csproj", "OSC.Applications.Domain/"]
COPY ["OSC.Applications.Services/OSC.Applications.Services.csproj", "OSC.Applications.Services/"]
COPY ["OSC.Applications.Infrastructure/OSC.Applications.Infrastructure.csproj", "OSC.Applications.Infrastructure/"]
COPY ["OSC.Applications.Contracts/OSC.Applications.Contracts.csproj", "OSC.Applications.Contracts/"]

RUN dotnet subset restore "OSC.Applications.Api/OSC.Applications.Api.csproj" --root-directory /src --output restore_subset/

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY --from=prepare /src/restore_subset ./
RUN dotnet restore "OSC.Applications.Api/OSC.Applications.Api.csproj"

COPY . .
WORKDIR "/src/OSC.Applications.Api"
RUN dotnet build "OSC.Applications.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "OSC.Applications.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OSC.Applications.Api.dll"]
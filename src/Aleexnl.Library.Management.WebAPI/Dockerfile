FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Aleexnl.Library.Management.WebAPI/Aleexnl.Library.Management.WebAPI.csproj", "src/Aleexnl.Library.Management.WebAPI/"]
RUN dotnet restore "src/Aleexnl.Library.Management.WebAPI/Aleexnl.Library.Management.WebAPI.csproj"
COPY . .
WORKDIR "/src/src/Aleexnl.Library.Management.WebAPI"
RUN dotnet build "./Aleexnl.Library.Management.WebAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Aleexnl.Library.Management.WebAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Aleexnl.Library.Management.WebAPI.dll"]

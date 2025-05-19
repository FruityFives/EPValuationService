# Base image for runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# Build image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["ValuationServiceAPI/ValuationServiceAPI.csproj", "ValuationServiceAPI/"]
RUN dotnet restore "ValuationServiceAPI/ValuationServiceAPI.csproj"
COPY . .
WORKDIR "/src/ValuationServiceAPI"
RUN dotnet build "ValuationServiceAPI.csproj" -c Release -o /app/build

# Publish image
FROM build AS publish
RUN dotnet publish "ValuationServiceAPI.csproj" -c Release -o /app/publish

# Final runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ValuationServiceAPI.dll"]

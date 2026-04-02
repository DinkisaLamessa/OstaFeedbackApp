# ==========================
# Stage 1: Build
# ==========================
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy only csproj files first for caching
COPY ["OstaFeedbackApp.csproj", "./"]

# Restore NuGet packages (cached if csproj unchanged)
RUN dotnet restore "OstaFeedbackApp.csproj"

# Copy remaining source code
COPY . .

# Publish app to /app/publish in Release mode
RUN dotnet publish "OstaFeedbackApp.csproj" -c Release -o /app/publish /p:UseAppHost=false

# ==========================
# Stage 2: Runtime
# ==========================
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Copy published files from build stage
COPY --from=build /app/publish .

# Expose HTTP port for web app + SignalR
EXPOSE 80

# Optional environment variables (can be overridden)
ENV ASPNETCORE_URLS=http://+:80
ENV DOTNET_RUNNING_IN_CONTAINER=true
ENV DOTNET_USE_POLLING_FILE_WATCHER=true

# Start the app
ENTRYPOINT ["dotnet", "OstaFeedbackApp.dll"]

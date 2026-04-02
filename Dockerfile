<<<<<<< HEAD
# ==========================
# Build Stage
# ==========================
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["OstaFeedbackApp.csproj", "./"]
RUN dotnet restore "OstaFeedbackApp.csproj"

COPY . .
RUN dotnet publish "OstaFeedbackApp.csproj" -c Release -o /app/publish

# ==========================
# Runtime Stage
# ==========================
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 80

ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "OstaFeedbackApp.dll"]
=======
# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy the project file(s) - adjust the path to your .csproj
COPY ["OSTAFeedbackSystem.csproj", "."]
RUN dotnet restore

# Copy all source files and build
COPY . .
RUN dotnet publish -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
EXPOSE 80

# Copy published application
COPY --from=build /app/publish .

# Configure the entry point
ENTRYPOINT ["dotnet", "OSTAFeedbackSystem.dll"]
>>>>>>> a47f374 (Clean repo without large files)

# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project file and restore
COPY ["OstaFeedbackApp.csproj", "./"]
RUN dotnet restore "OstaFeedbackApp.csproj" --disable-parallel

# Copy everything else and publish
COPY . .
RUN dotnet publish "OstaFeedbackApp.csproj" -c Release -o /app/publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "OstaFeedbackApp.dll"]

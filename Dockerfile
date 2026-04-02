# ==========================
# Build Stage
# ==========================
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj and restore first (better caching)
COPY ["OstaFeedbackApp.csproj", "./"]
RUN dotnet restore "OstaFeedbackApp.csproj"

# Copy everything else
COPY . .

# Publish app
RUN dotnet publish "OstaFeedbackApp.csproj" -c Release -o /app/publish /p:UseAppHost=false

# ==========================
# Runtime Stage
# ==========================
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

# IMPORTANT for Render (dynamic port)
ENV ASPNETCORE_URLS=http://+:$PORT
ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 80

ENTRYPOINT ["dotnet", "OstaFeedbackApp.dll"]

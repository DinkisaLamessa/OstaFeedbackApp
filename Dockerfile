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

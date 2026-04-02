# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project and restore exact package versions
COPY ["OstaFeedbackApp.csproj", "./"]
RUN dotnet restore "OstaFeedbackApp.csproj" --disable-parallel

# Copy the rest of the files and publish
COPY . .
RUN dotnet publish "OstaFeedbackApp.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 80
ENTRYPOINT ["dotnet", "OstaFeedbackApp.dll"]

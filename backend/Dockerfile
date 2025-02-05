## Backend

# Step 1: Build the application using the .NET SDK image
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /src/Mafia/backend

# Copy the .csproj and restore dependencies
COPY ["src/Mafia.csproj", "."]

RUN dotnet restore "Mafia.csproj"

# Copy the rest of the application files
COPY src/ .

RUN dotnet build "Mafia.csproj" -c Release -o /app/build

# Build the app
RUN dotnet publish "Mafia.csproj" -c Release -o /app/publish

# Step 2: Set up the runtime environment (use a smaller image with just the runtime)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Copy the built application from the build image
COPY --from=build /app/publish .

# Set entry point to run the app
ENTRYPOINT ["dotnet", "Mafia.dll"]
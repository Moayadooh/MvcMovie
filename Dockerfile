# ----------------------------
# Build stage
# ----------------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy full source tree
COPY . .

# Restore dependencies
RUN dotnet restore MvcMovie.csproj

# Build and publish
RUN dotnet publish MvcMovie.csproj -c Release -o /app/publish --no-restore

# ----------------------------
# Runtime stage
# ----------------------------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Environment config
ENV ASPNETCORE_URLS=http://+:5005
EXPOSE 5005

# Copy only published output
COPY --from=build /app/publish ./

ENTRYPOINT ["dotnet", "MvcMovie.dll"]

# Use the official .NET image as a base image
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Use the SDK image for build
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["FileStorage/FileStorage.csproj", "FileStorage/"]
RUN dotnet restore "FileStorage/FileStorage.csproj"
COPY . .
WORKDIR "/src/FileStorage"
RUN dotnet build "FileStorage.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "FileStorage.csproj" -c Release -o /app/publish

# Final stage: use the ASP.NET runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FileStorage.dll"]

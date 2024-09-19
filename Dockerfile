# Use the official Microsoft .NET SDK image as a build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY ["CodeToImageGenerator.Web/CodeToImageGenerator.Web.csproj", "CodeToImageGenerator.Web/"]
COPY ["CodeToImageGenerator.Common/CodeToImageGenerator.Common.csproj", "CodeToImageGenerator.Common/"]
RUN dotnet restore "CodeToImageGenerator.Web/CodeToImageGenerator.Web.csproj"

# Copy the rest of the files and build
COPY . .
WORKDIR "/src/CodeToImageGenerator.Web"
RUN dotnet build "CodeToImageGenerator.Web.csproj" -c Release -o /app/build

# Publish
FROM build AS publish
RUN dotnet publish "CodeToImageGenerator.Web.csproj" -c Release -o /app/publish

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 80
EXPOSE 443
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CodeToImageGenerator.Web.dll"]
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

# Installing dependencies for Chromium
RUN apt-get update && apt-get install -y \
    wget \
    ca-certificates \
    fonts-liberation \
    libasound2 \
    libatk-bridge2.0-0 \
    libatk1.0-0 \
    libcups2 \
    libdbus-1-3 \
    libnss3 \
    libx11-xcb1 \
    libxcomposite1 \
    libxdamage1 \
    libxrandr2 \
    libgbm1 \
    libgtk-3-0 \
    libxshmfence1 \
    libu2f-udev \
    xdg-utils \
    libgconf-2-4 \
    --no-install-recommends && rm -rf /var/lib/apt/lists/*

# Downloading and installing Chromium
RUN wget https://dl.google.com/linux/direct/google-chrome-stable_current_amd64.deb \
    && apt-get update && apt-get install -y ./google-chrome-stable_current_amd64.deb \
    && rm google-chrome-stable_current_amd64.deb

# Setting environment variable for Chromium
ENV PUPPETEER_EXECUTABLE_PATH=/usr/bin/google-chrome-stable


# Creating new user
RUN groupadd -r pptruser && useradd -r -g pptruser -G audio,video pptruser \
    && mkdir -p /home/pptruser/Downloads \
    && chown -R pptruser:pptruser /home/pptruser \
    && chown -R pptruser:pptruser /app

EXPOSE 80
EXPOSE 443
COPY --from=publish /app/publish .

USER pptruser

ENTRYPOINT ["dotnet", "CodeToImageGenerator.Web.dll"]
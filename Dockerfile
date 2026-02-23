# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build

WORKDIR /src

# Copy solution and project files
COPY AI.Assistant.sln .
COPY AI.Assistant.Core/AI.Assistant.Core.csproj AI.Assistant.Core/
COPY AI.Assistant.Infrastructure/AI.Assistant.Infrastructure.csproj AI.Assistant.Infrastructure/
COPY AI.Assistant.Application/AI.Assistant.Application.csproj AI.Assistant.Application/
COPY AI.Assistant.Presentation.Plugins/AI.Assistant.Presentation.Plugins.csproj AI.Assistant.Presentation.Plugins/
COPY AI.Assistant.Presentation.Telegram/AI.Assistant.Presentation.Telegram.csproj AI.Assistant.Presentation.Telegram/

# Restore dependencies
RUN dotnet restore AI.Assistant.sln

# Copy source code
COPY . .

# Build application in Release mode
RUN dotnet build -c Release

# Publish to output directory
RUN dotnet publish -c Release -o /app/publish AI.Assistant.Presentation.Telegram/AI.Assistant.Presentation.Telegram.csproj

# Runtime stage
FROM mcr.microsoft.com/dotnet/runtime:10.0-alpine

# Install tzdata for timezone support
RUN apk add --no-cache tzdata

# Create non-root user
RUN addgroup -g 1001 -S appgroup && \
    adduser -u 1001 -S appuser -G appgroup

WORKDIR /app

# Copy published application from build stage
COPY --from=build --chown=appuser:appgroup /app/publish .

# Switch to non-root user
USER appuser

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=5s --retries=3 \
    CMD dotnet --version || exit 1

# Run application
ENTRYPOINT ["dotnet", "AI.Assistant.Presentation.Telegram.dll"]

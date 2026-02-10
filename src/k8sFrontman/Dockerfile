# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG TARGETARCH
WORKDIR /src

# Map Docker TARGETARCH to .NET RID
RUN if [ "$TARGETARCH" = "amd64" ]; then \
        echo "x64" > /tmp/rid; \
    elif [ "$TARGETARCH" = "arm64" ]; then \
        echo "arm64" > /tmp/rid; \
    else \
        echo "x64" > /tmp/rid; \
    fi

# Copy project file and restore dependencies
COPY ["k8sFrontman.csproj", "./"]
RUN DOTNET_RID=$(cat /tmp/rid) && \
    dotnet restore -r linux-$DOTNET_RID

# Copy source code
COPY . .

# Publish as single-file executable with optimizations
RUN DOTNET_RID=$(cat /tmp/rid) && \
    dotnet publish "k8sFrontman.csproj" -c Release -r linux-$DOTNET_RID \
    --no-restore \
    --self-contained true \
    -p:PublishSingleFile=true \
    -p:PublishTrimmed=true \
    -p:EnableCompressionInSingleFile=true \
    -p:IncludeNativeLibrariesForSelfExtracted=true \
    -p:InvariantGlobalization=false \
    -o /app/publish && \
    chmod +x /app/publish/k8sFrontman

# Runtime stage - use smaller runtime-deps image for self-contained apps
FROM mcr.microsoft.com/dotnet/runtime-deps:10.0-noble-chiseled-extra AS final
WORKDIR /app

# OCI annotations / labels
LABEL org.opencontainers.image.title="k8sFrontman" \
      org.opencontainers.image.description="Kubernetes Operator built with k8sOperator" \
      org.opencontainers.image.vendor="k8sOperator" \
      org.opencontainers.image.source="https://github.com/pmdevers/k8sOperator"

# Use specific non-root user ID for better security
USER $APP_UID

# Copy published executable
COPY --from=build --chown=$APP_UID:$APP_UID /app/publish/k8sFrontman .

# Set environment variables
ENV ASPNETCORE_ENVIRONMENT=Production \
    DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false \
    DOTNET_EnableDiagnostics=0 \
    DOTNET_GCHeapHardLimit=0x8000000 \
    DOTNET_GCHeapHardLimitPercent=75 \
    COMPlus_EnableDiagnostics=0

# Health check (operators typically expose metrics on :8080)
HEALTHCHECK --interval=30s --timeout=3s --start-period=10s --retries=3 \
  CMD ["./k8sFrontman", "health"] || exit 1


# Expose metrics port (standard for Kubernetes operators)
EXPOSE 8080

# Entrypoint
ENTRYPOINT ["./k8sFrontman"]
CMD ["operator"]

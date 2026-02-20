# ============================
# 1. Build stage (SDK)
# ============================
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

ARG VERSION=0.0.0-dev

WORKDIR /src

# Copy everything (solution + src + tests if you want)
COPY . .

# Build k8sFrontman for all platforms
RUN for rid in linux-x64 linux-arm64 win-x64 win-arm64 osx-x64 osx-arm64; do \
        name=$(echo "$rid" | sed 's/^win-/windows_/;s/^osx-/darwin_/;s/-/_/'); \
        echo "Building for $rid..."; \
        dotnet publish k8sFrontman.csproj -c Release \
            -r "$rid" \
            --self-contained true \
            /p:PublishSingleFile=true \
            /p:Version=${VERSION} \
            /p:PublishTrimmed=true \
            /p:EnableCompressionInSingleFile=true \
            /p:AssemblyName=k8s-frontman_${name} \
            -o /out; \
    done

# ============================
# 2. Runtime image for operator
# ============================
FROM mcr.microsoft.com/dotnet/runtime-deps:10.0-noble-chiseled AS operator

ARG APP_UID=1654

WORKDIR /

LABEL org.opencontainers.image.title="k8sFrontman" \
      org.opencontainers.image.description="Kubernetes Operator built with k8sOperator" \
      org.opencontainers.image.vendor="k8sOperator" \
      org.opencontainers.image.source="https://github.com/pmdevers/k8sOperator"

# Use specific non-root user ID for better security
USER $APP_UID

# Set environment variables
ENV ASPNETCORE_ENVIRONMENT=Production \
    DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false \
    DOTNET_EnableDiagnostics=0 \
    DOTNET_GCHeapHardLimit=0x8000000 \
    DOTNET_GCHeapHardLimitPercent=75 \
    COMPlus_EnableDiagnostics=0

# Copy published executable
COPY --from=build --chown=$APP_UID:$APP_UID /out/k8s-frontman_linux_x64 .

HEALTHCHECK --interval=30s --timeout=3s --start-period=10s --retries=3 \
  CMD ["/k8s-frontman_linux_x64", "health"] || exit 1

  # Expose metrics port (standard for Kubernetes operators)
EXPOSE 8080

ENTRYPOINT ["/k8s-frontman_linux_x64", "operator"]

# ============================
# 3. CLI export stage
# ============================
FROM scratch AS cli-export

ARG VERSION=0.0.0-dev

# Copy k8sFrontman binaries with naming format: k8s-frontman_{version}_{os}_{arch}
COPY --from=build /out/k8s-frontman_linux_x64 /k8s-frontman_${VERSION}_linux_x64
COPY --from=build /out/k8s-frontman_linux_arm64 /k8s-frontman_${VERSION}_linux_arm64
COPY --from=build /out/k8s-frontman_windows_x64.exe /k8s-frontman_${VERSION}_windows_x64.exe
COPY --from=build /out/k8s-frontman_windows_arm64.exe /k8s-frontman_${VERSION}_windows_arm64.exe
COPY --from=build /out/k8s-frontman_darwin_x64 /k8s-frontman_${VERSION}_darwin_x64
COPY --from=build /out/k8s-frontman_darwin_arm64 /k8s-frontman_${VERSION}_darwin_arm64

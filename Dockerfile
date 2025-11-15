# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project files
COPY ["SchoolManagement/SchoolManagement.csproj", "SchoolManagement/"]
RUN dotnet restore "SchoolManagement/SchoolManagement.csproj"

# Copy all source files
COPY SchoolManagement/ SchoolManagement/

# Build the application
WORKDIR "/src/SchoolManagement"
RUN dotnet build "SchoolManagement.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "SchoolManagement.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Create a non-root user
RUN adduser --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

# Copy published files
COPY --from=publish /app/publish .

# Expose ports
EXPOSE 8080
EXPOSE 8081

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD curl --fail http://localhost:8080/health || exit 1

# Set entry point
ENTRYPOINT ["dotnet", "SchoolManagement.dll"]

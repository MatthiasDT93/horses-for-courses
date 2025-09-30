# ----------------------------
# Build stage
# ----------------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and all project files
COPY HorsesForCourses.sln .
COPY HorsesForCourses.Core/ HorsesForCourses.Core/
COPY HorsesForCourses.MVC/ HorsesForCourses.MVC/
COPY HorsesForCourses.Service/ HorsesForCourses.Service/

RUN dotnet restore HorsesForCourses.sln

# Restore dependencies
RUN dotnet restore HorsesForCourses.sln

# Copy everything else
COPY . .

# Build and publish the MVC project (entrypoint)
WORKDIR /src/HorsesForCourses.MVC
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# ----------------------------
# Runtime stage
# ----------------------------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_FORWARDEDHEADERS_ENABLED=true

COPY --from=build /app/publish .

EXPOSE 8080
ENTRYPOINT ["dotnet", "HorsesForCourses.MVC.dll"]

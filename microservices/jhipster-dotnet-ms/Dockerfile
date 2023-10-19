# Based on Microsoft's docker samples for dotnet core
# available at https://github.com/dotnet/dotnet-docker/tree/master/samples/aspnetapp
FROM mcr.microsoft.com/dotnet/sdk:6.0-focal AS build
# First install nodejs - required to build generated client app
WORKDIR /app

#
#ENTANDO -> Disabled node installaton because we have only a microservice in this bundle
#
#RUN apt-get update -yq && apt-get install -yq curl
#RUN curl -sL https://deb.nodesource.com/setup_16.x | bash - && \
#    apt-get update && \
#    apt-get install -yq nodejs && \
#    rm -rf /var/lib/apt/lists/*

# Restore dependencies of .net core projects taking advantage of docker layer caching
COPY src/*/*.csproj ./src/
COPY Directory.Packages.props Directory.Packages.props
RUN for file in $(ls src/*.csproj); do mkdir -p ${file%.*} && mv $file ${file%.*}; done
RUN dotnet restore "src/JhipsterDotNetMS/JhipsterDotNetMS.csproj"

# Copy everything else and build app
COPY . ./
WORKDIR src/JhipsterDotNetMS
RUN dotnet publish "JhipsterDotNetMS.csproj" -c Release -o /app/out

# Final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:6.0-focal AS runtime
EXPOSE 8081

WORKDIR /app
COPY docker-entrypoint-back.sh .
RUN chmod +x /app/docker-entrypoint-back.sh
COPY --from=build /app/out .
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://*:8081

ENTRYPOINT ["./docker-entrypoint-back.sh"]

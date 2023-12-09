FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env
WORKDIR /App

# Copy everything
COPY . ./
# Restore as distinct layers
RUN dotnet restore
# Build and publish a release
RUN dotnet publish -c Release -o out

RUN dotnet tool install --global dotnet-ef --version 7
ENV PATH="$PATH:/root/.dotnet/tools"
RUN dotnet ef migrations add InitialCreateDocker
RUN dotnet ef database update

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /App
COPY --from=build-env /App/out .
ENTRYPOINT ["dotnet", "TattooStudioBooking.dll"]
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /wd

# Copy all project files and restore packages as distinct layers
COPY ./**/*.csproj ./
RUN ls | while read line; do mkdir $(basename $line .csproj) && mv $line $(basename $line .csproj); done
COPY ./Queuey.sln .
RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o publish

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build-env /wd/publish .
ENTRYPOINT ["dotnet", "Noppes.Queuey.Api.dll"]

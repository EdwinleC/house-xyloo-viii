FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

COPY HouseXyloo.Api.csproj ./
RUN dotnet restore

COPY . ./

RUN dotnet publish HouseXyloo.Api.csproj \
    -c Release \
    -o /app/publish \
    --no-restore


FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "HouseXyloo.Api.dll"]
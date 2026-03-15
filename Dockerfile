FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["WeddingApi.web/WeddingApi.web.csproj", "WeddingApi.web/"]
COPY ["WeddingApi.core/WeddingApi.core.csproj", "WeddingApi.core/"]
COPY ["WeddingApi.infrastructure/WeddingApi.infrastructure.csproj", "WeddingApi.infrastructure/"]
RUN dotnet restore "WeddingApi.web/WeddingApi.web.csproj"
COPY . .
WORKDIR "/src/WeddingApi.web"
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "WeddingApi.web.dll"]
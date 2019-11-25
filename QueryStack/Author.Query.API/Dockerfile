FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["QueryStack/Author.Query.API/Author.Query.API.csproj", "QueryStack/Author.Query.API/"]
COPY ["QueryStack/Author.Query.Persistence/Author.Query.Persistence.csproj", "QueryStack/Author.Query.Persistence/"]
COPY ["Common/Author.Core.Framework/Author.Core.Framework.csproj", "Common/Author.Core.Framework/"]
COPY ["Common/Author.Core.Services.Rediscache/Author.Core.Services.Rediscache.csproj", "Common/Author.Core.Services.Rediscache/"]
COPY ["QueryStack/Author.Query.Domain/Author.Query.Domain.csproj", "QueryStack/Author.Query.Domain/"]
COPY ["Common/Author.Core.Services.Persistence.CosmosDB/Author.Core.Services.Persistence.CosmosDB.csproj", "Common/Author.Core.Services.Persistence.CosmosDB/"]
RUN dotnet restore "QueryStack/Author.Query.API/Author.Query.API.csproj"
COPY . .
WORKDIR "/src/QueryStack/Author.Query.API"
RUN dotnet build "Author.Query.API.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Author.Query.API.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Author.Query.API.dll"]
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["QueryStack/Author.Query.New.API/Author.Query.New.API.csproj", "QueryStack/Author.Query.New.API/"]
COPY ["QueryStack/Author.Query.Persistence/Author.Query.Persistence.csproj", "QueryStack/Author.Query.Persistence/"]
COPY ["Common/Author.Core.Framework/Author.Core.Framework.csproj", "Common/Author.Core.Framework/"]
COPY ["Common/Author.Core.Services.Rediscache/Author.Core.Services.Rediscache.csproj", "Common/Author.Core.Services.Rediscache/"]
COPY ["QueryStack/Author.Query.Domain/Author.Query.Domain.csproj", "QueryStack/Author.Query.Domain/"]
COPY ["Common/Author.Core.Services.Persistence.CosmosDB/Author.Core.Services.Persistence.CosmosDB.csproj", "Common/Author.Core.Services.Persistence.CosmosDB/"]

RUN dotnet restore "QueryStack/Author.Query.New.API/Author.Query.New.API.csproj"
COPY . .
WORKDIR "/src/QueryStack/Author.Query.New.API"
RUN dotnet build "Author.Query.New.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Author.Query.New.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Author.Query.New.API.dll"]
FROM mcr.microsoft.com/dotnet/core/aspnet:2.1-stretch-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:2.1-stretch AS build
WORKDIR /src
COPY ["Common/Author.Core.ApiGateway/Author.Core.ApiGateway.csproj", "Common/Author.Core.ApiGateway/"]
RUN dotnet restore "Common/Author.Core.ApiGateway/Author.Core.ApiGateway.csproj"
COPY . .
WORKDIR "/src/Common/Author.Core.ApiGateway"
RUN dotnet build "Author.Core.ApiGateway.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Author.Core.ApiGateway.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Author.Core.ApiGateway.dll"]
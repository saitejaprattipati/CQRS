FROM mcr.microsoft.com/dotnet/core/aspnet:2.1-stretch-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:2.1-stretch AS build
WORKDIR /src
COPY ["Common/Author.Core.ApiGatewayMobile/Author.Core.ApiGatewayMobile.csproj", "Common/Author.Core.ApiGatewayMobile/"]
RUN dotnet restore "Common/Author.Core.ApiGatewayMobile/Author.Core.ApiGatewayMobile.csproj"
COPY . .
WORKDIR "/src/Common/Author.Core.ApiGatewayMobile"
RUN dotnet build "Author.Core.ApiGatewayMobile.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Author.Core.ApiGatewayMobile.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Author.Core.ApiGatewayMobile.dll"]
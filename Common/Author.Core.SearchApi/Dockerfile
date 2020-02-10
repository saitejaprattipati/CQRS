#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Common/Author.Core.SearchApi/Author.Core.SearchApi.csproj", "Common/Author.Core.SearchApi/"]
RUN dotnet restore "Common/Author.Core.SearchApi/Author.Core.SearchApi.csproj"
COPY . .
WORKDIR "/src/Common/Author.Core.SearchApi"
RUN dotnet build "Author.Core.SearchApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Author.Core.SearchApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Author.Core.SearchApi.dll"]
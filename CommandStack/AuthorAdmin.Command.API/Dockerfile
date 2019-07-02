#EXPOSE 4369 5671 5672 25672 15671 15672
#
#FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
#WORKDIR /app
#EXPOSE 80
#EXPOSE 443


FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["CommandStack/AuthorAdmin.Command.API/AuthorAdmin.Command.API.csproj", "CommandStack/AuthorAdmin.Command.API/"]
RUN dotnet restore "CommandStack/AuthorAdmin.Command.API/AuthorAdmin.Command.API.csproj"
COPY . .
WORKDIR "/src/CommandStack/AuthorAdmin.Command.API"
RUN dotnet build "AuthorAdmin.Command.API.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "AuthorAdmin.Command.API.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "AuthorAdmin.Command.API.dll"]
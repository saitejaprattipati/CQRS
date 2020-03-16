#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["CommandStack/Author.Command.API/Author.Command.API.csproj", "CommandStack/Author.Command.API/"]
COPY ["Common/Author.Core.Services.EventBus.RabbitMQ/Author.Core.Services.EventBus.RabbitMQ.csproj", "Common/Author.Core.Services.EventBus.RabbitMQ/"]
COPY ["Common/Author.Core.Services.EventBus/Author.Core.Services.EventBus.csproj", "Common/Author.Core.Services.EventBus/"]
COPY ["CommandStack/Author.Command.Service/Author.Command.Service.csproj", "CommandStack/Author.Command.Service/"]
COPY ["CommandStack/Author.Command.Persistence/Author.Command.Persistence.csproj", "CommandStack/Author.Command.Persistence/"]
COPY ["CommandStack/Author.Command.Domain/Author.Command.Domain.csproj", "CommandStack/Author.Command.Domain/"]
COPY ["Common/Author.Core.Framework/Author.Core.Framework.csproj", "Common/Author.Core.Framework/"]
COPY ["CommandStack/Author.Command.Events/Author.Command.Events.csproj", "CommandStack/Author.Command.Events/"]
COPY ["Common/Author.Core.Services.Persistence.CosmosDB/Author.Core.Services.Persistence.CosmosDB.csproj", "Common/Author.Core.Services.Persistence.CosmosDB/"]
COPY ["Common/Author.Core.Services.BlobStorage/Author.Core.Services.BlobStorage.csproj", "Common/Author.Core.Services.BlobStorage/"]
COPY ["Common/Author.Core.Services.EventBus.Azure/Author.Core.Services.EventBus.Azure.csproj", "Common/Author.Core.Services.EventBus.Azure/"]
RUN dotnet restore "CommandStack/Author.Command.API/Author.Command.API.csproj"
COPY . .
WORKDIR "/src/CommandStack/Author.Command.API"
RUN dotnet build "Author.Command.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Author.Command.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Author.Command.API.dll"]
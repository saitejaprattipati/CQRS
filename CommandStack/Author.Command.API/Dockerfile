##Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
##For more information, please see http://aka.ms/containercompat 
#
#FROM microsoft/dotnet:2.1-aspnetcore-runtime-nanoserver-1803 AS base
#WORKDIR /app
#EXPOSE 80
#EXPOSE 443
#
#FROM microsoft/dotnet:2.1-sdk-nanoserver-1803 AS build
#WORKDIR /src
#COPY CommandStack/Author.Command.API/Author.Command.API.csproj CommandStack/Author.Command.API/
#COPY CommandStack/Author.Command.Service/Author.Command.Service.csproj CommandStack/Author.Command.Service/
#COPY CommandStack/Author.Command.Persistence/Author.Command.Persistence.csproj CommandStack/Author.Command.Persistence/
#COPY CommandStack/Author.Command.Domain/Author.Command.Domain.csproj CommandStack/Author.Command.Domain/
#COPY CommandStack/Author.Command.Events/Author.Command.Events.csproj CommandStack/Author.Command.Events/
#RUN dotnet restore CommandStack/Author.Command.API/Author.Command.API.csproj
#COPY . .
#WORKDIR /src/CommandStack/Author.Command.API
#RUN dotnet build Author.Command.API.csproj -c Release -o /app
#
#FROM build AS publish
#RUN dotnet publish Author.Command.API.csproj -c Release -o /app
#
#FROM base AS final
#WORKDIR /app
#COPY --from=publish /app .
#ENTRYPOINT ["dotnet", "Author.Command.API.dll"]
#


#Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
#For more information, please see https://aka.ms/containercompat

FROM microsoft/dotnet:2.1-aspnetcore-runtime-nanoserver-1803 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM microsoft/dotnet:2.1-sdk-nanoserver-1803 AS build
WORKDIR /src
COPY ["CommandStack/Author.Command.API/Author.Command.API.csproj", "CommandStack/Author.Command.API/"]
RUN dotnet restore "CommandStack/Author.Command.API/Author.Command.API.csproj"
COPY . .
WORKDIR "/src/CommandStack/Author.Command.API"
RUN dotnet build "Author.Command.API.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Author.Command.API.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Author.Command.API.dll"]
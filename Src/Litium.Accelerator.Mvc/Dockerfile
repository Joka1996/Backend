#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

#Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
#For more information, please see https://aka.ms/containercompat
#Ändrat denna rad till som det är nu från FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
FROM registry.litium.cloud/runtime/litium:net6-latest AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
#KOMMENTAR AV MIG TEST TILL GIT
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Src/Litium.Accelerator.Mvc/Litium.Accelerator.Mvc.csproj", "Src/Litium.Accelerator.Mvc/"]
COPY ["Src/Litium.Accelerator.GraphQL/Litium.Accelerator.GraphQL.csproj", "Src/Litium.Accelerator.GraphQL/"]
COPY ["Src/Litium.Accelerator/Litium.Accelerator.csproj", "Src/Litium.Accelerator/"]
COPY ["Src/Litium.Accelerator.FieldTypes/Litium.Accelerator.FieldTypes.csproj", "Src/Litium.Accelerator.FieldTypes/"]
COPY ["Src/Litium.Accelerator.Elasticsearch/Litium.Accelerator.Elasticsearch.csproj", "Src/Litium.Accelerator.Elasticsearch/"]
COPY ["Src/Litium.Accelerator.Administration.Extensions/Litium.Accelerator.Administration.Extensions.csproj", "Src/Litium.Accelerator.Administration.Extensions/"]
RUN dotnet restore "Src/Litium.Accelerator.Mvc/Litium.Accelerator.Mvc.csproj"
COPY . .
WORKDIR "/src/Src/Litium.Accelerator.Mvc"
RUN dotnet build "Litium.Accelerator.Mvc.csproj" -c Debug -o /app/build

FROM build AS publish
RUN dotnet publish "Litium.Accelerator.Mvc.csproj" -c Debug -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Litium.Accelerator.Mvc.dll"]
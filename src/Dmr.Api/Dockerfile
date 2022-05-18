#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["DMR.Api/DMR.Api.csproj", "DMR.Api/"]
RUN dotnet restore "DMR.Api/DMR.Api.csproj"
COPY . .
WORKDIR "/src/DMR.Api"
RUN dotnet build "DMR.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DMR.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DMR.Api.dll"]
#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base

WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["LostFoundAngkasaPura.sln", "./"]
COPY ["LostFoundAngkasaPura/LostFoundAngkasaPura.csproj", "LostFoundAngkasaPura/"]
COPY ["LostFoundAngkasaPura.DAL/LostFoundAngkasaPura.DAL.csproj", "LostFoundAngkasaPura.DAL/"]
RUN dotnet restore "./LostFoundAngkasaPura/LostFoundAngkasaPura.csproj"
RUN dotnet restore "./LostFoundAngkasaPura.DAL/LostFoundAngkasaPura.DAL.csproj"
COPY . .
WORKDIR "/src/LostFoundAngkasaPura"
RUN dotnet build "LostFoundAngkasaPura.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "LostFoundAngkasaPura.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LostFoundAngkasaPura.dll"]

#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Manage Target/Manage Target.csproj", "Manage Target/"]
RUN dotnet restore "Manage Target/Manage Target.csproj"
COPY . .
WORKDIR "/src/Manage Target"
RUN dotnet build "Manage Target.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Manage Target.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Manage Target.dll"]
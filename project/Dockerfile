FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["LauBjuTizVezBra/LauBjuTizVezBra.csproj", "LauBjuTizVezBra/"]
RUN dotnet restore "LauBjuTizVezBra/LauBjuTizVezBra.csproj"
COPY . .
WORKDIR "/src/LauBjuTizVezBra"
RUN dotnet build "LauBjuTizVezBra.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "LauBjuTizVezBra.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY LauBjuTizVezBra/Infrastructure/Data/Game.db /app/Infrastructure/Data/
#COPY LauBjuTizVezBra/wwwroot/Images /app/wwwroot/Images
ENTRYPOINT ["dotnet", "LauBjuTizVezBra.dll"]
# Etapa de construcción
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar los archivos de solución y restaurar dependencias
COPY ["src/MyVectorApp/MyVectorApp.csproj", "src/MyVectorApp/"]
COPY ["src/MyVectorLib/MyVectorLib.csproj", "src/MyVectorLib/"]
RUN dotnet restore "src/MyVectorApp/MyVectorApp.csproj"

# Copiar todo y construir
COPY . .
WORKDIR "/src/src/MyVectorApp"
RUN dotnet build "MyVectorApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MyVectorApp.csproj" -c Release -o /app/publish

# Etapa de runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MyVectorApp.dll"]

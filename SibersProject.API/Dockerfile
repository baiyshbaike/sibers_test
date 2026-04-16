# Build stage / Этап сборка 
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project files for restore (layer caching)
COPY ["SibersProject.API/SibersProject.API.csproj", "SibersProject.API/"]
COPY ["SibersProject.BLL/SibersProject.BLL.csproj", "SibersProject.BLL/"]
COPY ["SibersProject.DAL/SibersProject.DAL.csproj", "SibersProject.DAL/"]

RUN dotnet restore "SibersProject.API/SibersProject.API.csproj"

#Copy all soruce files
COPY . .

WORKDIR "/src/SibersProject.API"
RUN dotnet build "SibersProject.API.csproj" -c Release -o /app/build

#Publish stage / Этап пабликация
FROM build AS publish
RUN dotnet publish "SibersProject.API.csproj" -c Release -o /app/publish /p:UseAppHost=false


#Runtime stage / Этап запуска
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SibersProject.API.dll"]

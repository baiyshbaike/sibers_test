# Build stage / Этап сборка 
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["SibersProjec.API/SibersProject.API.csproj", "SibersProject.API/"]
COPY ["SibersProjec.API/SibersProject.BLL.csproj", "SibersProject.BLL/"]
COPY ["SibersProjec.API/SibersProject.DAL.csproj", "SibersProject.DAL/"]

RUN dotnet restore "SibersProject.API/SibersProject.API.csproj"

#Copy all soruce files
COPY . .

WORKDIR "/src/SiversProject.API"
RUN dotnet build "SibersProject.APi.csproj" -c Release -o /app/build

#Publish stage / Этап пабликация
FROM build as publish
RUN dotnet publish "SibersProject.API.csproj" -c Realease -o /app/build/publish /p:UseAppHost=false


#Runtime stage / Этап запуска
FROM mcr.microsoft.com/dotnet/asp.net:10.0 AS final
WORKDIR /app
EXPOSE 8080
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SibersProject.API.dll"]

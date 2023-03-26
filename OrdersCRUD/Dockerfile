FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["OrdersCRUD.csproj", "OrdersCRUD/"]
RUN dotnet restore "OrdersCRUD/OrdersCRUD.csproj"
COPY . .
WORKDIR "/src/OrdersCRUD"
RUN dotnet build "OrdersCRUD.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "OrdersCRUD.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OrdersCRUD_assembly.dll"]

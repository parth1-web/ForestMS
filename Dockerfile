#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["LE.Web/LE.Web.csproj", "LE.Web/"]
COPY ["LE.Account.Factories/LE.Account.Factories.csproj", "LE.Account.Factories/"]
COPY ["LE.Account.Service/LE.Account.Service.csproj", "LE.Account.Service/"]
COPY ["LE.Account.Entities/LE.Account.Entities.csproj", "LE.Account.Entities/"]
COPY ["LE.Account.Common/LE.Account.Common.csproj", "LE.Account.Common/"]
COPY ["LE.Common/LE.Common.csproj", "LE.Common/"]
COPY ["LE.Account.Infrastructure/LE.Account.Infrastructure.csproj", "LE.Account.Infrastructure/"]
COPY ["LE.Account.Providers/LE.Account.Providers.csproj", "LE.Account.Providers/"]
COPY ["LE.Context/LE.Context.csproj", "LE.Context/"]
COPY ["LE.Billing.Entities/LE.Billing.Entities.csproj", "LE.Billing.Entities/"]
COPY ["LE.Billing.Common/LE.Billing.Common.csproj", "LE.Billing.Common/"]
COPY ["LE.Entities/LE.Entities.csproj", "LE.Entities/"]
COPY ["LE.Inventory.Entities/LE.Inventory.Entities.csproj", "LE.Inventory.Entities/"]
COPY ["LE.Inventory.Common/LE.Inventory.Common.csproj", "LE.Inventory.Common/"]
COPY ["LE.Service/LE.Service.csproj", "LE.Service/"]
COPY ["LE.Infrastructure/LE.Infrastructure.csproj", "LE.Infrastructure/"]
COPY ["LE.Billing.Context/LE.Billing.Context.csproj", "LE.Billing.Context/"]
COPY ["LE.Billing.Service/LE.Billing.Service.csproj", "LE.Billing.Service/"]
COPY ["LE.Billing.Infrastructure/LE.Billing.Infrastructure.csproj", "LE.Billing.Infrastructure/"]
COPY ["LE.Inventory.Service/LE.Inventory.Service.csproj", "LE.Inventory.Service/"]
COPY ["LE.Inventory.Infrastructure/LE.Inventory.Infrastructure.csproj", "LE.Inventory.Infrastructure/"]
COPY ["LE.Billing.Factories/LE.Billing.Factories.csproj", "LE.Billing.Factories/"]
COPY ["LE.Invenory.Context/LE.Invenory.Context.csproj", "LE.Invenory.Context/"]
RUN dotnet restore "./LE.Web/LE.Web.csproj"
COPY . .
WORKDIR "/src/LE.Web"
RUN dotnet build "./LE.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./LE.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LE.Web.dll"]
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY TscOrders.slnx .
COPY TscOrders.Core.Abstraction/TscOrders.Core.Abstraction.csproj TscOrders.Core.Abstraction/
COPY TscOrders.Core/TscOrders.Core.csproj TscOrders.Core/
COPY TscOrders.Data.Abstraction/TscOrders.Data.Abstraction.csproj TscOrders.Data.Abstraction/
COPY TscOrders.Data/TscOrders.Data.csproj TscOrders.Data/
COPY TscOrders.WebApi/TscOrders.WebApi.csproj TscOrders.WebApi/
COPY TscOrders.Tests/TscOrders.Tests.csproj TscOrders.Tests/
RUN dotnet restore TscOrders.WebApi/TscOrders.WebApi.csproj

COPY . .
RUN dotnet publish TscOrders.WebApi/TscOrders.WebApi.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

RUN addgroup --system --gid 1000 app && adduser --system --uid 1000 --ingroup app app
COPY --from=build /app .
RUN mkdir -p /app/data && chown -R app:app /app/data
USER app

ENV ASPNETCORE_URLS=http://+:8080
ENV ConnectionStrings__DefaultConnection="Data Source=/app/data/tscorders.db"
EXPOSE 8080

ENTRYPOINT ["dotnet", "TscOrders.WebApi.dll"]

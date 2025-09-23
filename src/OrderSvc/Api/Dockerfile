FROM mcr.microsoft.com/dotnet/sdk:9.0 AS restore
WORKDIR /src

# We still copy from repo root because the build context is root
COPY *.sln ./
COPY src/OrderSvc/Api/OrderSvc.Api.csproj src/OrderSvc/Api/
COPY src/OrderSvc/Application/OrderSvc.Application.csproj src/OrderSvc/Application/
COPY src/OrderSvc/Infrastructure/OrderSvc.Infrastructure.csproj src/OrderSvc/Infrastructure/
COPY src/OrderSvc/Domain/OrderSvc.Domain.csproj src/OrderSvc/Domain/
RUN dotnet restore src/OrderSvc/Api/OrderSvc.Api.csproj

FROM restore AS build
COPY . .
RUN dotnet publish src/OrderSvc/Api/OrderSvc.Api.csproj -c Release -o /out

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /out .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "OrderSvc.Api.dll"]

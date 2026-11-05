FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet publish PersonalFinanceAndExpenseTracker.WebAPI/PersonalFinanceAndExpenseTracker.WebAPI.csproj -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app .
ENV ASPNETCORE_URLS=http://+:10000
EXPOSE 10000
ENTRYPOINT ["dotnet", "PersonalFinanceAndExpenseTracker.WebAPI.dll"]

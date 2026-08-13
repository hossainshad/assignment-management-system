FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY AssignmentSystem.Core/ AssignmentSystem.Core/
COPY AssignmentSystem.Infrastructure/ AssignmentSystem.Infrastructure/
COPY AssignmentSystem.API/ AssignmentSystem.API/

RUN dotnet publish AssignmentSystem.API/AssignmentSystem.API.csproj -c Release -o /app/out

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/out .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "AssignmentSystem.API.dll"]
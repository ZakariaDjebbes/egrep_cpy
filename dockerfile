FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /app

COPY . ./

RUN dotnet restore
RUN dotnet publish egrep_cpy/egrep_cpy.csproj -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["./egrep_cpy", "-f", "babylon.txt", "-r", "Sargon", "-c", "-w"]
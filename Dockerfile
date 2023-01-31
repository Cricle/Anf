# FROM node:latest AS clientbuild
# WORKDIR /ClientApp
# COPY Platforms/Anf.Web/ClientApp/ .
# RUN npm install -g cnpm --registry=https://registry.npm.taobao.org
# RUN cnpm install
# RUN npm run build.prod

FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine3.16-arm64v8 AS serverbuild
WORKDIR /build
COPY . .
WORKDIR /build/Platforms/Anf.Web
RUN dotnet restore -r linux-musl-arm64 /p:PublishReadyToRun=true
RUN dotnet publish -c Release -o /app -r linux-musl-arm64 --self-contained true --no-restore /p:PublishTrimmed=true /p:PublishReadyToRun=true /p:PublishSingleFile=true

FROM mcr.microsoft.com/dotnet/runtime-deps:6.0-alpine3.16-arm64v8 AS final
EXPOSE 80 443
WORKDIR /app
COPY --from=serverbuild /app .
# COPY --from=clientbuild /ClientApp/dist wwwroot
COPY Platforms/Anf.Web/ClientApp/dist wwwroot
ENTRYPOINT ["./Anf.Web"]


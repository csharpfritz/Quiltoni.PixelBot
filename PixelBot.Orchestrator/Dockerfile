FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-alpine AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-alpine AS build
WORKDIR /src

RUN apk add --no-cache git

COPY ["PixelBot.Orchestrator/PixelBot.Orchestrator.csproj", "PixelBot.Orchestrator/"]
COPY ["PixelBot.Google/PixelBot.Google.csproj", "PixelBot.Google/"]
COPY ["Quiltoni.PixelBot.Core/Quiltoni.PixelBot.Core.csproj", "Quiltoni.PixelBot.Core/"]
COPY ["PixelBot.Games.GuessGame/PixelBot.Games.GuessGame.csproj", "PixelBot.Games.GuessGame/"]
COPY ["PixelBot.StandardFeatures/PixelBot.StandardFeatures.csproj", "PixelBot.StandardFeatures/"]
COPY ["PixelBot.UI/PixelBot.UI.csproj", "PixelBot.UI/"]
RUN dotnet restore "PixelBot.Orchestrator/PixelBot.Orchestrator.csproj"
COPY . .
WORKDIR "/src/PixelBot.Orchestrator"
RUN dotnet build "PixelBot.Orchestrator.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PixelBot.Orchestrator.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PixelBot.Orchestrator.dll"]
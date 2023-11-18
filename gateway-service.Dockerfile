FROM mcr.microsoft.com/dotnet/aspnet:7.0

WORKDIR /app

EXPOSE 80 

COPY Lab3Publish/GatewayService .

ENTRYPOINT ["dotnet", "Rsoi.Lab2.GatewayService.HttpApi.dll"]
FROM mcr.microsoft.com/dotnet/aspnet:7.0

WORKDIR /app

EXPOSE 80 

COPY Lab3Publish/RatingService .

ENTRYPOINT ["dotnet", "Rsoi.Lab3.RatingService.HttpApi.dll"]
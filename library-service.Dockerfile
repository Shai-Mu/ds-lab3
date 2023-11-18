FROM mcr.microsoft.com/dotnet/aspnet:7.0

WORKDIR /app

EXPOSE 80 

COPY Lab3Publish/LibraryService .

ENTRYPOINT ["dotnet", "Rsoi.Lab2.LibraryService.HttpApi.dll"]
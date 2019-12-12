#run in root development dir
dotnet new webapi -o CloudMovieDatabase
cd CloudMovieDatabase\
dotnet add package Microsoft.EntityFrameworkCore.Core -v 3.0.0
dotnet add package Microsoft.EntityFrameworkCore.Sqlite -v 3.0.0
dotnet add package Microsoft.EntityFrameworkCore.Design -v 3.0.0
dotnet watch run
#open http://localhost:5000/weatherforecast
#VS Code
code -r ..\CloudMovieDatabase\


#install dotnet-ef tool
dotnet tool install --global dotnet-ef

#database creation migration
dotnet ef migrations add InitialCreate -v

#create database
dotnet ef database update

#install aspnet code generator tool
dotnet tool install -g dotnet-aspnet-codegenerator --version 3.0.0
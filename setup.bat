#run in root development dir
dotnet new webapi -o CloudMovieDatabase
cd CloudMovieDatabase\
dotnet add package Microsoft.EntityFrameworkCore.Core -v 3.1.0
dotnet add package Microsoft.EntityFrameworkCore.InMemory -v 3.1.0
dotnet add package Microsoft.EntityFrameworkCore.Design -v 3.1.0
dotnet watch run
#open http://localhost:5000/weatherforecast
#VS Code
code -r ..\CloudMovieDatabase\

#other useful commands

#install dotnet-ef tool
dotnet tool install --global dotnet-ef

#database creation migration
dotnet ef migrations add InitialCreate -v

#create database
dotnet ef database update

#install aspnet code generator tool
dotnet tool install -g dotnet-aspnet-codegenerator --version 3.1.0


dotnet aspnet-codegenerator controller -outDir Controllers -name ActorsController -async -api -m CloudMovieDatabase.Models.Actor -dc CloudMovieDatabase.Data.DataContext
dotnet aspnet-codegenerator controller -outDir Controllers -name MoviesController -async -api -m CloudMovieDatabase.Models.Movie -dc CloudMovieDatabase.Data.DataContext


#switch to SqlServer as sqlite doesn't support some migration actions
dotnet add package Microsoft.EntityFrameworkCore.SqlServer -v 3.1.0
dotnet add package Microsoft.EntityFrameworkCore.Tools -v 3.1.0
dotnet add package Microsoft.VisualStudio.Web.CodeGeneration.Design -v 3.1.0
dotnet add package Microsoft.AspNetCore.Mvc.NewtonsoftJson -v 3.1.0

#move main project into webapi dir
#create solution
dotnet new sln
#add project
dotnet sln add .\webapi\CloudMovieDatabase.csproj

dotnet add CloudMovieDatabase.csproj package Swashbuckle.AspNetCore -v 5.0.0-rc4
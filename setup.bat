#run in root development dir
dotnet new webapi -o CloudMovieDatabase
cd CloudMovieDatabase\
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.InMemory
#VS Code
code -r ..\CloudMovieDatabase\


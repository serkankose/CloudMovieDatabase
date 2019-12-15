Cloud Movie Database

See API at Swagger page: http://localhost:5000/swagger/index.html 
Postman collection for integration testing: MovieDatabaseAPI.postman_collection.json

The cloud movie database is a simple backend project which stores information about movies and actors and provides REST Api. Please assume at least the following information is kept for each entity:

 

Movie: Title, Year, Genre, Starring actors

Actor: First name, Last name, Birthday, Filmography

 Functional requirements

    Add new movie to the system
    Add new actor
    Link existing actor to existing movie
    Update information about existing movie
    Delete existing movie
    List movies (all and by year)
    List actors starring in a movie
    List movies with given actor
    Cannot add new movie without actors
    Movie’s year cannot be a future year
    Actor’s first and last name cannot be empty

Additional requirements not specified above can be assumed by you.

Technical requirements

    Single ASP.NET Core Web Api project
    Unit Test project which tests the controller(s)
    Data needs to be persisted in a cloud storage of your choice (Azure)
    Ensure concurrent access based on optimistic approach to storing the movie and actor to the data base
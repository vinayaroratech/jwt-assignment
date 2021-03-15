# JWT Demo App (ASP.NET Core / ASP.NET 5+)

This repository demos an ASP.NET Core web API application using JWT auth, and an integration testing project for a set of actions including login, logout, refresh token, impersonation, authentication, and authorization.


## Solution Structure

This repository includes one application: an ASP.NET Core web API app in the `src/JwtDemoApp` folder. An API BaseURL is `https://localhost:5001/swagger`. 

- `webapi`
  The ASP.NET Core web API app is served by Kestrel on Docker. This app has implemented HTTPS support.

## Usage

The demo is configured to run by Docker Compose. The services are listed in the `docker-compose.yml` file. You can launch the demo by the following command.

```bash
docker-compose up --build --remove-orphans -d
```

Then visit [https://localhost:5001/swagger/index.html](https://localhost:5001/swagger/index.html) for Swagger document for the web API project.
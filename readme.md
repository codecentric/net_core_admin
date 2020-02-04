# NetCoreAdmin

This package adds the abillity to use Spring Boot Admin with a Asp.Net Core Server.

## Usage

1. Install Package
1. Add in ConfigureServices:

```C#
services.AddNetCoreAdmin(Configuration);
```

1. Run Spring Boot Admin, e.g. in a Docker Container

```bash
docker run -d -p 1111:1111 -e SPRING_BOOT_ADMIN_TITLE='SB Admin' -e SPRING_BOOT_ADMIN_SECURITY_ENABLED=false --name spring-boot-admin slydeveloper/spring-boot-admin:latest
```

1. Configure your application so that in can talk to SBA:

Configure in appsettings.json OR Code

```json
 "NetCoreAdmin": {
    "springBootServerUrl": "http://localhost:1111/instances",
    "retryTimeout": "0:00:10",
    "Application": {
      "BaseUrl": "http://host.docker.internal:5000"
    }
  }
```

Note that host.docker.internal is required on Windows hosts, otherwise it should be localhost:1111

1. Launch your application
1. Navigate to SBA, you should be able to see your server

## Issues

Note that a lot of endpoints are exposed at /actuator/* - always secure them otherwise sensitive data WILL leak!.

## Health

Per default, we respond with an "OK" Health result.
You can customize this using Health Checks: https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/health-checks?view=aspnetcore-3.1

Please note that we use an internal Health check url, not the one configured by HealthCheck due to the need to conforms to Actuators Data format.

## Beans

This maps to the Services registered via ASP.NET Cores DI.
The displayed NAME is the concrete type, e.g. a DataProvider
The Alias contains the Type we registered for, e.g. IDataProvider.

In case of Lists of objects, a " | 1" is appended, since Actuator does not know of this concept.
The Type might not be accurate, especially for Factories.
Due to the DI limitations, it is not possible to show Dependencies.

## LogFile

Allows you to view the log of your app.
Due to the limitations of Spring Boot Admins existing contract, it is needed to write an actual file with the log file somewhere.
The location of this file can be set through the configuration

```json logFilePath": "C:\\dev\\nactuator\\NactuatorSample\\NactuatorSample\\log20200204.txt"```

OR implement a `ILogFileLocationResolver` and register it in the DI. 
The latter is usually more appropiate for Serilog, because Serilogs FileSink regularly changes the name of the file. See this Sample for an Implementation.

## Todo

1. Add more endpoints

    - Logging
    - Info
    - Components( and move them off Environment?)
    - Threads
    - Metrics
    - Web - Mappings (and the other one?)
    - Audit Log?
    - Caches?

1. Doku
1. Auth
1. Deregister
1. Get Config Validation working
1. Single-Property and EDIT for Env
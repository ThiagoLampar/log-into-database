# AutoLog Database

This project was developed using .NET Core technology and the idea of is to allow you to save the state of all your models before and after being updated, in a generic and transparent way in your database.

## How it works

The idea of the **AutoLog Database** service is to record all changes made to your database values.
The service occurs through the transformations on the columns and values of any entity it receives, this collection works through reflections and at the end the service returns a string set that forms a log and records it in the database.

When the *LogService* service is invoked, you will pass the log type will be parameterized, the entity that has been changed or registered, and then the service will do all the work of creating the values to save the log.

- In query/registration types the service will only save values of type **after** which indicates that your project has been inserted.
- In the type of update, the service will use the **KeyModel** key that indicates its primary key and then query what are the current values of your model already saved in the database. This will make the before and after recorded.
- In the removal log type, the service will log the values as **before**.

## How to use

- You will need to use Visual Studio 2017 or later with .NET Core SDKs installed.
- To use the database of this project, you must have Microsoft SQL Server version 2017 or higher or you may use with some modifications any other database.
- Inside the **appsettings.json** file you will need to set up your database connection or enjoy the connection your project already uses.
- If you use your own **appsettings.json** file, simply add the **KeyModel** key and enter the value of your default primary key.

You can also use this project in Visual Studio Code (Windows, Linux, and MacOS).
To learn more about project migration and setup, visit the official Microsoft documentation [Microsoft .NET Download Guide](https://www.microsoft.com/net/download)

## Technologies implemented

- ASP.NET Core 2.2
- Entity Framework Core 2.2.6
- Reflections
- Enums
- Microsoft Extensions

## Database

In the project there is a directory called *Sql*, it finds a simple file for creating tables for logging.
You can make the changes according to your need, only an adjustment to the logging service is required.

# A cross platform, modularized CMS system built on .NET Core

## Online demo (Azure Website)
http://lumle.azurewebsites.net/

Username: demo@ekbana.com
Password: Admin@123

## Visual Studio 2017 and SQL Server

#### Prerequisites

- SQL Server
- [Visual Studio 2017 version 15.3 with .NET Core SDK 2.0](https://www.microsoft.com/net/core/)

#### Steps to run

- Create a database in SQL Server
- Update the connection string in appsettings.json in Lumle.Web
- Build whole solution.
- In the Task Runner Explorer, right click on the "copy-modules" task and Run.
- Open Package Manager Console Window and type "Update-Database" then press "Enter". This action will create database schema.
- In Visual Studio, press "Control + F5".
- There is default user with superadmin role by default [Username: demo@ekbana.com, Password: Admin@123]

## Mac/Linux with PostgreSQL

#### Prerequisite

- PostgreSQL
- NodeJS
- [.NET Core SDK 2.0](https://www.microsoft.com/net/core/)

## Technologies and frameworks used:
- ASP.NET MVC Core 2.0.0 on .NET Core 2.0.0 
- Entity Framework Core 2.0.0
- ASP.NET Identity Core 2.0.0
- Autofac 4.2.0
- UI(Monarch : https://agileui.com/demo/monarch/demo/admin-template/index.html)
- Nlog for logging (https://github.com/NLog/NLog)
- Web Optimizer to minify CSS and JS files on runtime (https://github.com/ligershark/WebOptimizer)
- WebMarkupMin to minify HTML (https://github.com/Taritsyn/WebMarkupMin) 
- Hangfire to schedule jobs
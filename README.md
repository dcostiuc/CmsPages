# CmsPages

## Description

This is a layered startup solution based on Domain Driven Design (DDD).

### Prerequisites

- [.NET9.0+ SDK](https://dotnet.microsoft.com/download/dotnet)
- [Node v18 or 20](https://nodejs.org/en)

### Configurations

The solution comes with a default configuration that works out of the box. However, you may consider to change the following configuration before running your solution:

- Check the `ConnectionStrings` in `appsettings.json` files under the `CmsPages.Blazor` and `CmsPages.DbMigrator` projects and change it if you need.

### Before running the application

- Run `abp install-libs` command on your solution folder to install client-side package dependencies. This step is automatically done when you create a new solution, if you didn't especially disabled it. However, you should run it yourself if you have first cloned this solution from your source control, or added a new client-side package dependency to your solution.
- Run `CmsPages.DbMigrator` to create the initial database. This step is also automatically done when you create a new solution, if you didn't especially disabled it. This should be done in the first run. It is also needed if a new database migration is added to the solution later.

### Solution structure

This is a layered monolith application that consists of the following applications:

- `CmsPages.DbMigrator`: A console application which applies the migrations and also seeds the initial data. It is useful on development as well as on production environment.
- `CmsPages.Blazor`: ASP.NET Core Blazor Server application that is the essential web application of the solution.

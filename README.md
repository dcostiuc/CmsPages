# “CMSPages”, a sample CMS Full-Stack Web App

Demo:

## Description


This is a Content Management System (CMS) application written with the **ABP Framework**, using **ASP.NET Blazor Server** (with Blazorise) and **Entity Framework Core** (EF Core), as well as **Microsoft SQL Server**.

This project is a follow up after working on [this](https://github.com/dcostiuc/cms-app/) one.
The focus of this project was to try implementing my own custom version of CMS Kit's Pages, by creating my own Domain Entity, DTO, Application Service, Permission, etc.
However, instead of completely re-implementing everything that CMS Kit Pages already does, this project further focuses on adding in additional features that I previously was not able to implement on top of CMS Kit.

The project was built on top of a new layered startup solution based on Domain Driven Design (DDD), created initially through ABP Studio.

### Prerequisites

- [.NET9.0+ SDK](https://dotnet.microsoft.com/download/dotnet)
- [Node v18 or 20](https://nodejs.org/en)

### Configurations

The solution comes with a default configuration that works out of the box. However, you may consider to change the following configuration before running your solution:

- Check the `ConnectionStrings` in `appsettings.json` files under the `CmsPages.Blazor` and `CmsPages.DbMigrator` projects and change it if you need.

### Before running the application

- Run `abp install-libs` command on your solution folder to install client-side package dependencies. This step is automatically done when you create a new solution, if you didn't especially disabled it. However, you should run it yourself if you have first cloned this solution from your source control, or added a new client-side package dependency to your solution.
- Run `CmsPages.DbMigrator` to create the initial database. This step is also automatically done when you create a new solution, if you didn't especially disabled it. This should be done in the first run. It is also needed if a new database migration is added to the solution later.

If there is a DB-related error (e.g. when running the app), try resetting the DB and/or deleting the existing migrations.

### Solution structure

This is a layered monolith application that consists of the following applications:

- `CmsPages.DbMigrator`: A console application which applies the migrations and also seeds the initial data. It is useful on development as well as on production environment.
- `CmsPages.Blazor`: ASP.NET Core Blazor Server application that is the essential web application of the solution.

## Technical decisions and tradeoffs 

The big decision here was to **not** use an existing application module like CMS Kit, and instead try making things from scratch.
The alternative would have been to use CMS Kit, which I did in [this](https://github.com/dcostiuc/cms-app/) project.
  
The tradeoff is that we have very fine-tuned control over how the page functionality looks and works. This also makes it easier to expand on the existing features and to add in new ones.
However, this also means that you kind of "re-invent the wheel" since a lot of the functionality for Pages in a CMS is already provided in CMS Kit. Which also means it would take longer to complet the overall project.

Another decision was to focus more on implementing the things that I didn't or couldn't implement when I tried using [CMS Kit](https://github.com/dcostiuc/cms-app/).
This includes:
- Using Blazor UI components
- Going through the DDD approach for making new features (creating Domain Entities, DTOs, AppService, etc)
- Permissions for Creating/Editing/Deleting pages
- Unit tests

Note that this project uses the **Multi-Layer Template**, which is intended for more longer-term/serious/production-level applications.

## Tools used

The following tools were used:
- Visual Studio
- ABP Framework, ABP CLI, ABP Studio
- ABP Framework documentation + YouTube videos
- ChatGPT

## "Next Steps" section with: 

### What you would improve, refactor, or add 

Ideally, I would add any other features that I didn’t get around to implementing/couldn't figure out how to implement on top of CMS Kit, such as:
- Versioning/entity history
- Having the menu items auto-fill based on the existing pages that are already created
- Fine-tuned permissions for creating/editing/deleting CMS Kit pages
- etc


In terms of what I would improve:
- Not much, as CMS Kit already handles most of the functionality that I wanted and it does so well.
- Perhaps one small thing I would improve is better understanding the relationship between its Pages and Images functionality.  
   - Initially I only wanted to enable Pages and Menus from CMS Kit, but I found that then I wouldn't be able to successfully upload an image as part of a Page's content.


And in terms of refactoring:
- After realizing that not much is exposed for you to extend on top of CMS Kit, I decided to try implementing a simple CMS app without using CMS Kit with the time I had left.
- Essentially, I would consider refactoring the whole project to be more custom made (e.g. having my own Domain Entity, DTO, Application Service, UI etc).

### Thoughts about scalability, caching, modularity, or architectural improvements

#### Scalability
The current project is not that scalable, as even though it supports multi-tenancy and has support for using a distributed cache (which would help with periods of increased usage and usage from multiple tenants), it currently uses in-memory caching as distributed caching was not set up for it (although we could do so with Redis for example).  
  
Also, instead of solely relying on ABP Framework to help with creating and maintaining the database and data, we could also improve scalability by indexing, sharding and replicating the database. Although, we would also need to change the DB as currently it uses SQLite which doesn't support the aforementioned concepts.

#### Caching
Currently, the caching strategy for this project is simple, i.e. in-memory caching.
Ideally, especially for a multi-tenant content-hosting application like a CMS, it would be better to use a distributed cache, using a tool such as Redis. 
This would also help with scalability.

#### Modularity
Since ABP Framework was designed with modularity in mind, there are ways in which this project is already modular.  
For example, we are using the CMS Kit module to power the core functionality of the project.  
One way the project could be even more modular is by separating out the custom project logic from the framework boilerplate into its own plugin/module, and then using that similar to how we used CMS Kit.  

##### Architectural improvements
At the moment, the application is a monolith.   
Transitioning to more of a microservice architecture could be an improvement for all of the above (scalability, caching, modularity). 
Also, containerization of the current project (or microservices), for example, using Docker, could help with stability, maintainability, and scalability as well.



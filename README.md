# “CMSPages”, a WIP CMS Full-Stack Web App

Brief Application Demo: 
https://github.com/user-attachments/assets/c0b247c5-6b2f-4eb9-b2c3-19dd3b4de4ae

Brief Code Demo:
https://github.com/user-attachments/assets/ffbae90a-f4cf-4daf-84fa-b0b8df1acedf

## Description


This is a WIP Content Management System (CMS) application written with the **ABP Framework**, using **ASP.NET Blazor Server** (with Blazorise) and **Entity Framework Core** (EF Core), as well as **Microsoft SQL Server**.

This project is a follow up after working on [this](https://github.com/dcostiuc/cms-app/) one. The focus of this project was to try implementing my own custom version of CMS Kit's Pages, by creating my own Domain Entity, DTO, Application Service, Permission, etc.    

However, instead of completely re-implementing everything that CMS Kit Pages already does, this project further focuses on adding in additional features that I previously was not able to implement on top of CMS Kit.  
  
The project was built on top of a layered startup solution based on Domain Driven Design (DDD), created initially through ABP Studio.

### Prerequisites

- [.NET9.0+ SDK](https://dotnet.microsoft.com/download/dotnet)
- [Node v18 or 20](https://nodejs.org/en)

### Before running the application

- Run `abp install-libs` command on your solution folder to install client-side package dependencies.

If there is a DB-related error (e.g. when running the app), try resetting the DB and/or deleting the existing migrations and re-running the DbMigrator.

### Solution structure

This is a layered monolith application that consists of the following applications:

- `CmsPages.DbMigrator`: A console application which applies the migrations and also seeds the initial data.
- `CmsPages.Blazor`: ASP.NET Core Blazor Server application that is the essential web application of the solution.

## Technical decisions and tradeoffs 

The big decision here was to **not** use an existing application module like CMS Kit, and instead try making things from scratch.
The alternative would have been to use CMS Kit, which I did in [this](https://github.com/dcostiuc/cms-app/) project.
  
The tradeoff is that we have very fine-tuned control over how the page functionality looks and works. This also makes it easier to expand on the existing features and to add in new ones.
However, this also means that you kind of "re-invent the wheel" since a lot of the functionality for Pages in a CMS is already provided in CMS Kit. Which also means it would take longer to complete the overall project.

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

## Next Steps

### What you would improve, refactor, or add 

Ideally, I would add any other features that I didn’t get around to implementing, which would be most of the expected CMS features (aka pretty much re-implementing CMS Kit Pages).

I would fix any bugs that exist and add more unit tests so that the test suite is more thorough.

### Thoughts about scalability, caching, modularity, or architectural improvements

#### Scalability
The current project is more scalable than the one with CMS Kit, as it also supports multi-tenancy and supports using a distributed cache, though it currently uses in-memory caching as distributed caching was not set up for it yet (although we could do so with Redis for example).    
    
Also, instead of solely relying on ABP Framework to help with creating and maintaining the database and data, we could also improve scalability by indexing, sharding and replicating the database. Although, it would be better to change the DB as it currently uses SQL Server.  
  
Related to the above, we could use MongoDB instead of SQL Server, as it would be more suitable for the needs of a larger CMS application.   
  
For example, Mongo is naturally suited for horizontal scaling (aka sharding), which is more cost/performance efficient if the CMS were to become more of a distributed system.    
On the other hand, SQL Server is harder to shard, and instead would require vertical scaling, which can cost a lot more and has a limit (e.g. there’s only so many resources 1 machine can handle).    
Mongo also has flexibility in it’s document-based NoSQL design, where it would be easier to store data of varying shape and format (e.g. blog posts, comments, tags), without needing to define rigid schemas for each possible kind of data that needs to be stored or needing migrations every time there is a change in the data model somewhere (e.g. a new property added).   

#### Caching
Currently, the caching strategy for this project is simple, i.e. in-memory caching.
Ideally, especially for a multi-tenant content-hosting application like a CMS, it would be better to use a distributed cache, using a tool such as Redis. 
This would also help with scalability.

#### Modularity
Since ABP Framework was designed with modularity in mind, there are ways in which this project is already modular.  
For example, the project is split into multiple "layers", such as:
- Application
- Application.Contracts
- Blazor
- DbMigrator
- Domain
- Domain.Shared
- EntityFrameworkCore
- HttpApi
- HttpApi.Client
along with having a whole other `test/` section that mirrors the above structure but is focused on tests and test infrastructure/harnesses.
  
One way the project could be even more modular is by separating out all the custom project logic from the framework boilerplate into its own plugin/module, and then injecting it as a dependency.

##### Architectural improvements
At the moment, the application is a layered monolith.   
Transitioning to more of a microservice architecture could be an improvement for all of the above (scalability, caching, modularity). 
Also, containerization of the current project (or microservices), for example, using Docker, could help with stability, maintainability, and scalability as well.



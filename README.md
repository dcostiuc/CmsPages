# CMS Pages: a CMS Full-Stack Web App

## Description
This project is a Content Management System (CMS) application written with the **ABP Framework**, using **ASP.NET Blazor Server** (with **Blazorise**) and **Entity Framework Core** (EF Core), as well as **Microsoft SQL Server** (and **xUnit** for unit tests).

The project was built on top of a layered startup solution based on Domain Driven Design (DDD), created initially through ABP Studio.

### Features
- creating, editing, deleting, and viewing custom-made pages
  - this includes visiting the page’s url and seeing it’s title and content rendered
- an admin panel for doing the above actions, as well as pagination, sorting and filtering
- changing permissions for the above for pages
- automatically creating/updating/deleting menu items for each page
- being able to assign one of the pages (at a time) as the homepage (and a default homepage if none are set)
- a WYSIWYG content editor that supports HTML and Markdown (Markdown doesn’t come with a preview in the editor though)
   - input is sanitized for security
- supports soft delete and auditing
- handles invalid url and invalid page url by showing a “not found” page
- user feedback/validation for both positive and negative actions (e.g. successfully creating a page, or failing to update a page, etc)
- multi-tenancy support
- max-length validations on the data model and user input
- other kinds of exception handling, including relevant user error pop-ups shown when needed
- Swagger UI (in dev environment) to test API endpoints
- unit tests for the various functionality and logic for the above features (the tests try to be thorough but do not necessarily have 100% coverage of all features)

### Demos
New Application Demo: https://drive.google.com/file/d/1VLdhQWTnfqgPml6YROZi7UgQ7OjfYR5C/view?usp=drive_link  
Part 2: https://drive.google.com/file/d/1uLb2D9vqgIxCm5oREtU3YHhzZ4ZQNCv_/view?usp=sharing  
(I realized afterwards that I went to a previously-made page that also had markdown rendered as html, but the functionality demoed is the same)

New Brief Code Demo: https://drive.google.com/file/d/1jR3HVEaRubC4x62-FpUliKeoX1yZfKUt/view?usp=drive_link  
Also all unit tests pass:
![image](https://github.com/user-attachments/assets/84acbb9f-b6a2-49fc-8741-b98330b12b09)



### Prerequisites

- [.NET9.0+ SDK](https://dotnet.microsoft.com/download/dotnet)
- [Node v18 or 20](https://nodejs.org/en)

### Before running the application

- Run `abp install-libs` command on your solution folder to install client-side package dependencies.

### Solution structure

This is a layered monolith application that consists of several applications.   

Of particular note:
- `CmsPages.Blazor`: ASP.NET Core Blazor Server application that is the essential web application of the solution.
- `CmsPages.DbMigrator`: A console application which applies the migrations and also seeds the initial data.


## Technical decisions and tradeoffs 

To start, one major decision was to use ABP framework with Blazor Server and EF Core. On the one hand, we get strong modularity, domain-driven design, and built-in features (auditing, multi-tenancy, authorization, etc). On the other hand, this tight coupling to ABP means we have less flexibility if we ever want to move off of it.  

Also, we used SQL Server for the database. This was mainly because it is well-supported by the framework and integrates well with EF Core, though we do also get some benefits like helping enforce data integrity. However, this causes us to use SQL and store relational data, and the RDBMS makes it harder to scale horizontally or with storing embedded media (if we wanted to do that).

Another big decision here was to **not** use an existing application module like CMS Kit, and instead try making things from scratch.
The alternative would have been to use CMS Kit, which I did in [this](https://github.com/dcostiuc/cms-app/) project.
  
The tradeoff is that we have very fine-tuned control over how the page functionality looks and works. This also makes it easier to expand on the existing features and to add in new ones.
However, this also means that you kind of "re-invent the wheel" since a lot of the functionality for Pages in a CMS is already provided in CMS Kit.

We are also storing the entire page content directly inside the DB. While this makes it easy to manage such data due to the existing models and entities we have and keeps the rest of the page metadata together, it has complications for scaling (e.g. if we start embedding videos or pages get very long), caching (e.g. if we wanted to use a CDN), and we currently can’t partially load content (e.g. lazy-load more of the page as the user scrolls).  

Note that this project uses the Multi-Layer Template, which is intended for more longer-term/serious/production-level applications.

## Tools used

The following tools were used:
- Visual Studio and VS Code
- ABP Framework, ABP CLI, ABP Studio
- ABP Framework documentation + YouTube videos
- ChatGPT
- GitHub Copilot

## Next Steps

### What you would improve, refactor, or add 

#### Refactor
I would consider refactoring the current implementation by moving my custom logic into its own module and integrating it using ABP's Plugin system. This approach would improve modularity and separation of concerns. Additionally, I would consider making smaller improvements, such as changing the Content property to be non-nullable, and renaming RouteName (along with any related parts of the code) to a more accurate name like UrlSlug.

#### Add 
- The features that I didn’t implement. This includes:
  - uploading images
  - using a distributed cache


### Thoughts about scalability, caching, modularity, or architectural improvements

#### Scalability
Since this project is built on top of the ABP Framework, it benefits from a strong architectural foundation, including abstractions like Unit of Work, repository pattern, DTOs, and a layered architecture (Domain, Application, Presentation). This design promotes separation of concerns and makes the system easier to maintain and extend.

Built-in features like auditing and soft-delete also help with traceability and data control.

While the current implementation works well for 10s of pages, scaling to 100s/1000s could become problematic, for example, with search performance. For bigger sets of data, a dedicated search engine such as Elasticsearch would be more scalable than simple querying within the DB. Though for now, especially since we don't search content but rather only title or route, it is still manageable.

If the size of page content were to grow significantly (for example, we add support for directly uploading videos - not just embedding to a youtube video), it would then make sense to store the content in an external store like Azure Blob Storage, especially when combined with CDN caching.

For database scalability, indexing is very important and should be done based on the most common query patterns. This is because indexes help a lot with query performance by allowing the database to locate and find rows without scanning the whole table. For example, we could consider creating an index on RouteName if it is often queried. We could also consider creating an index on Title if we saw based on user telemetry that a lot of searching/filtering is done on the title.

Related to scalability, we could consider using MongoDB instead of SQL Server, depending on the future requirements of the CMS.

MongoDB naturally supports horizontal scaling through sharding, which can be more cost-effective and scalable for large, distributed systems. In contrast, SQL Server is more suitable for vertical scaling, which often requires more expensive hardware and has physical limitations.

Additionally, MongoDB’s document-based NoSQL model provides more flexibility for storing data of varying structure (e.g. if we were to have blog posts, comments, tags) without needing schema updates and migrations if data models end up changing.  

However, Mongo lacks some relational and consistency features, as it focuses more on schema flexibility, horizontal scaling, and high availability in distributed systems.


#### Caching
The project currently uses in-memory caching, which would be enough for smaller scale apps. However, for more scalable architecture, especially for a multi-tenant CMS app, a it would be better if we used a distributed cache through a tool like Redis.

Using caching effectively can help a lot with performance, responsiveness, and scalability. It also makes a lot of sense to use in this kind of project since Page content is unlikely to change often while at the same time is likely to be accessed (viewed) often. So, for example, we could cache each page by guid and cache its decoded and sanitized page content and have it expire when that page actually gets updated. We could also cache the menu items as well as the home page in particular, for the same reasons (unlikely to change often, likely to be accessed often).

We could also consider using CDN caching with services like Cloudflare. By caching static assets on public-facing pages such as videos, images, fonts, even javascript and css (if we allowed custom scripting and styling per page), we would improve the speed of loading a page and its content while also reducing load on the server.

#### Modularity
Since the ABP Framework is built with modularity in mind, the current project already benefits from a modular structure to some extent.
For example, it follows a layered architecture with clearly separated projects:
- Application
- Application.Contracts
- Blazor
- DbMigrator
- Domain
- Domain.Shared
- EntityFrameworkCore
- HttpApi
- HttpApi.Client
  
Additionally, there's a corresponding `test/` folder that mirrors this structure, allowing each project to have its own dedicated tests.

While the aove provides good separation of concerns, we can still make things more modular. One way would be to group related functionality (e.g. page management) into a dedicated domain module (e.g. CmsPages.PagesModule). This could eventually be turned into a reusable ABP module, in the best case allowing us to even re-use its functionality in other ABP apps (similar to how we already can with CMS Kit).

This would allow us to encapsulate related domain logic, application services, entities, and migrations together. Doing so also improves testability, scalability, and long-term maintainability.

#### Architectural improvements
At the moment, the application is a layered modular monolith. At the current scale of this project, it is faster and easier to maintain this way.

However, in the future, we could consider splitting the project up into separate microservices (such as one for Pages, another for Blogging if we decide to add that in, etc). This gives us more flexibility in things like scaling one part of the app more than others (e.g. in case Pages get a lot of traffic but Blogs don't). It would help with modularity as there are more clear boundaries between Page-related data and logic versus other parts of the app (e.g. Blogging if it were added). It helps with resiliency as the current monolith architecture means that resources are shared between modules/services, so if we, for example, had a background job related to indexing pages for search, that job would currently be at risk of starving resources (e.g. using the database heavily) and if that happened, that would affect other parts of the app (e.g. simply trying to view pages, or even editing your account). If we split such a job into its own microservice, we isolate it more to avoid bottlenecks/problems for the rest of the app.

Also, containerizing the project (and its future microservices), for example, using Docker, could help with stability (consistency across dev/staging/prod environments), maintainability (more separation of concerns), and scalability (easier deployment of different parts of app, easier horizontal scaling) as well.

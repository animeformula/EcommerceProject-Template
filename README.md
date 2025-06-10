# EcommerceProject

A modern ASP.NET Core MVC e-commerce web application.

## Features
- User authentication and registration
- Product catalog with categories
- Product details with star ratings and text reviews
- Shopping cart and wishlist (AJAX-enabled)
- Order management for users and admins
- Admin dashboard for managing orders
- Export orders to CSV
- Responsive, modern UI with Bootstrap

## Getting Started (Visual Studio)

### 1. Prerequisites
- Visual Studio 2022 or later (with ASP.NET and web development workload)
- .NET 6 or later (check with `dotnet --version`)
- SQL Server (LocalDB is fine for development)

### 2. Clone the Repository
```
git clone <your-repo-url>
cd EcommerceProject
```

### 3. Open the Solution
- Open `EcommerceProject.sln` in Visual Studio.

### 4. Check Entity Framework Core Tools
- In Visual Studio, open the **Package Manager Console** (Tools > NuGet Package Manager > Package Manager Console).
- Run:
  ```
  dotnet ef --version
  ```
- If you see a version, EF Core CLI is installed. If not, install it:
  ```
  dotnet tool install --global dotnet-ef
  ```

### 5. Set Up the Database
- **If this is your first time:**
  - In the Package Manager Console, run:
    ```
    dotnet ef database update
    ```
  - This will create the database and all tables.
- **If the database already exists:**
  - Run the same command to apply any new migrations:
    ```
    dotnet ef database update
    ```

### 6. Run the Project
- Press **F5** or click **Start Debugging** in Visual Studio.
- The site will open in your browser (usually at `https://localhost:xxxx`).

### 7. Default Admin Account
- If seeded, the default admin is:
  - **Email:** `admin@example.com`
  - **Password:** `Admin123!`

---

## Troubleshooting
- If you get a database error, make sure SQL Server (LocalDB) is running and you have the right connection string in `appsettings.json`.
- If you add new models, run:
  ```
  dotnet ef migrations add <MigrationName>
  dotnet ef database update
  ```

---

## Useful Commands
- List migrations:
  ```
  dotnet ef migrations list
  ```
- Remove last migration:
  ```
  dotnet ef migrations remove
  ```
- Create a new migration:
  ```
  dotnet ef migrations add <MigrationName>
  ```

## Usage
- Register a new user or log in.
- Browse products, add to cart or wishlist.
- Rate and review products.
- Place orders and view order history.
- Admins can manage orders and export data.

## Folder Structure
- `Controllers/` - MVC controllers
- `Models/` - Entity models
- `Views/` - Razor views
- `Data/` - Database context and migrations
- `ViewModels/` - View models for forms
- `wwwroot/` - Static files (CSS, JS, images)

## License
MIT 
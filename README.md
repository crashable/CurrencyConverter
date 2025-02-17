# Currency Converter

This project consists of:
- **CurrencyConverter.Console**: A console application for converting currency using the Fixer.io API.
- **CurrencyConverter.WorkerService**: A background worker service that fetches and stores exchange rates daily.
- **CurrencyConverter.Api**: A REST API providing currency conversion.
- **CurrencyConverter.webapp**: A React front-end for interacting with the API.

## Prerequisites

Before setting up the project, ensure you have the following installed:

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Node.js (for React UI)](https://nodejs.org/)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (or a Docker container with SQL Server)
- A **Fixer.io API key** (sign up at [Fixer.io](https://fixer.io/))

---

## Setup Instructions

### 1. Clone the Repository
```bash
git clone https://github.com/YOUR_USERNAME/CurrencyConverter.git
cd CurrencyConverter
```

### 2. Set Up Environment Variables
Create a `.env` file in the root folder or set environment variables manually.

```
FIXERIO_API_KEY=your_fixer_io_api_key
```

Alternatively, add this key to your system environment variables.

---

## Backend Setup

### 3. Configure the Database
#### **Option 1: Local SQL Server**
Update the connection string in **`appsettings.json`** of `CurrencyConverter.Api` and `CurrencyConverter.WorkerService`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=ExchangeRates;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

#### **Option 2: Using Docker**
Run SQL Server in a Docker container:
```bash
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=YourStrong@Passw0rd" -p 1433:1433 --name sqlserver -d mcr.microsoft.com/mssql/server:2019-latest
```
Then update `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=ExchangeRates;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True;"
  }
}
```

### 4. Apply Migrations
Navigate to the WorkerService or API project folder and run:

```bash
dotnet ef database update
```

---

## Running the Applications

### **Run the Console App**
```bash
cd src/CurrencyConverter.Console
dotnet run USD NOK 100
```

### **Run the Worker Service**
```bash
cd src/CurrencyConverter.WorkerService
dotnet run
```

### **Run the API**
```bash
cd src/CurrencyConverter.Api
dotnet run
```
API will be available at: `https://localhost:[port]/swagger` (Swagger UI)

---

## Frontend Setup (React)

### 5. Install Dependencies
```bash
cd src/CurrencyConverter.webapp
npm install
```

### 6. Start the React App
```bash
npm start
```
React UI will be available at `http://localhost:58431`

---

## Additional Notes

- Ensure the API and Worker Service are running for full functionality.
- If you experience **CORS issues**, update `Program.cs` in `CurrencyConverter.Api` to allow the React app’s origin:
  ```csharp
  builder.Services.AddCors(options =>
  {
      options.AddPolicy("AllowReactApp", policy =>
      {
          policy.WithOrigins("http://localhost:3000")
                .AllowAnyHeader()
                .AllowAnyMethod();
      });
  });
  ```
- For debugging, logs are available in the terminal when running each service.

---

## License
This project is licensed under the MIT License.

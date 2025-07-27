# 🔗 LinkShorterAPI

A powerful and scalable **.NET 9 Web API** for creating, managing, and tracking short links — built with clean architecture, authentication, analytics, and email support.

---

## 🚀 Getting Started

### ✅ Prerequisites
- [.NET 9 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)
- SQL Server (local or remote)
- [SendGrid account](https://sendgrid.com/) (for sending emails)

### 📦 Clone & Setup
```bash
git clone <your-repo-url>
cd LinkShorterAPI
````

### 🔧 Configuration

Update `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "LinkShortnerConnectionString": "YOUR-CONNECTION-STRING-HERE"
  },
  "Jwt": {
    "Key": "YOUR-SECRET-KEY",
    "Issuer": "YOUR-ISSUER",
    "Audience": "YOUR-AUDIENCE"
  },
  "SendGrid": {
    "ApiKey": "YOUR-SENDGRID-API-KEY"
  }
}
```

* `ConnectionStrings` → Your SQL Server string
* `Jwt` → Used for user authentication
* `SendGrid.ApiKey` → For email notification features

### 🗺️ GeoIP Database (Optional)

Download `GeoLite2-City.mmdb` from [MaxMind](https://dev.maxmind.com/geoip/geolite2-free-geolocation-data) and place it in the `GeoData/` folder to enable IP-based location tracking.

---

## 🛠️ Build & Run

```bash
dotnet restore
dotnet build
dotnet run
```

Default URL: `https://localhost:5001`

---

## 📚 API Documentation

* Swagger UI: [https://localhost:5001/swagger](https://localhost:5001/swagger)

---

## 🔐 Environment Variables & Secrets

* Never commit secrets to source control.
* Use `dotnet user-secrets` or system environment variables in production.
* Example:

  ```bash
  dotnet user-secrets set "Jwt:Key" "your-secret"
  ```

---

## ✉️ Email Notifications (SendGrid)

1. Register at [SendGrid](https://sendgrid.com/) and get an API key.
2. Place it in `appsettings.json` or your secrets.
3. Update sender email in `SendGridEmailService.cs`:

   ```csharp
   var from = new EmailAddress("your-verified-email@example.com", "SHORT LINK");
   ```

---

## 🧪 Testing

* Use Swagger or Postman for API testing.
* A test file `LinkServiceTest.cs` is included for local/demo use.

---

## ⚙️ Tech Stack & Dependencies

* **.NET 9.0** / **ASP.NET Core**
* **Entity Framework Core** (SQL Server)
* **JWT Bearer Authentication**
* **FluentValidation** / **AutoMapper**
* **BCrypt.Net-Next** for password hashing
* **SendGrid** email integration
* **MaxMind.GeoIP2** for IP geo-location
* **UAParser** for device/browser parsing
* **Swagger (Swashbuckle)** for API documentation
* **Scrutor** for DI decoration
* **Ngrok** (optional, for local tunneling)

---

## 🛡️ Security Best Practices

* Use long, random JWT keys (32+ chars)
* Keep secrets outside version control
* Use HTTPS in production
* Hash passwords with BCrypt (already integrated)

---

## ⚠️ Important Setup Notes

Make sure you update the following parts of the project:

### 1. 🔐 Sender Email

**File:** `Services/EmailServices/SendGridEmailService.cs`

```csharp
var from = new EmailAddress("PUT-YOUR-COMPANY-WORK-EMAIL-HERE", "SHORT LINK");
```

### 2. 🌐 Default Domain

**File:** `Services/LinkService/LinkService.cs`

```csharp
var defaultDomain = await _linkRepository.GetDefualtDomain("//PUT-YOUR-NGROK-OR-LIVE-DOMAIN");
```

### 3. 🔑 Secrets & Keys

**File:** `appsettings.json`

```json
"LinkShortnerConnectionString": "YOUR-CONNECTION-STRING-HERE",
"SendGrid": { "ApiKey": "YOUR-SENDGRID-KEY" }
```

### 4. 🌐 Test URLs

**File:** `LinkServiceTest.cs`
Replace sample domains like:

```csharp
"https://www.example.com" → your own domain
```

---

## 🧰 Directory Structure (Brief Overview)

```
LinkShorterAPI/
│
├── Controllers/            → API endpoints
├── DTOs/                   → Request/response models
├── Services/               → Business logic
├── Repositories/           → Data access layer
├── Models/                 → EF Core entities
├── Authentication/         → JWT, login, register
├── Background Service/     → Queued jobs or scheduled tasks
├── ClassValidators/        → FluentValidation validators
├── MapperProfiles/         → AutoMapper config
├── GeoData/                → IP location DB (GeoLite2)
└── README.md               → You're here
```

---

## 📄 License

This project is currently **private/internal**. Add a license if you plan to open source.

---

## 🤝 Contributing

Contributions, ideas, and suggestions are welcome. Please open issues or discussions before submitting pull requests.

---

## ❓ Need Help?

* Refer to comments in the code
* Check [`README_LinkService.md`](README_LinkService.md)
* Or open a GitHub issue

---

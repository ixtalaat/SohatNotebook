# SohatNotebook - Health Tracking Web API

SohatNotebook is a Health Tracking API built on ASP.NET Core. This project aims to provide a comprehensive platform for users to track and manage their health-related data.

## Table of Contents

- [Key Features](#key-features)
- [Setup](#setup)
- [Usage](#usage)
- [API Documentation](#api-documentation)
- [Technologies Used](#technologies-used)
- [Contributing](#contributing)
- [License](#license)

## Key Features

- **Personalized Health Insights:** Enable users to securely manage and maintain detailed health records
- **Customizable User Profiles:** Empower users to personalize and manage their profiles, tailoring experiences and data presentation to individual preferences.
- **Robust Security Measures:** Implement secure account creation, login, and JWT-based authentication to ensure data privacy.

## Setup

### Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/download) installed
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) installed (or other relevant database)

### Installation

1. **Clone the repository:**
   ```bash
   git clone https://github.com/ixtalaat/SohatNotebook.git
   ```
2. **Install dependencies:**
   ```bash
   cd SohatNotebook
   dotnet restore
   ```
3. **Database Setup:**
  - **Configure Connection String**: Locate the **"appsettings.json"** file and update the database connection string under **"ConnectionStrings"** with your database details.
  - **Run Migrations**: Execute the following commands in the terminal:
   ```bash
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```
4. **Run the application:**
   ```bash
   dotnet run
   ```

### Usage
1. Register an account: Use the /api/accounts/register endpoint to create a new account.
2. Login: Obtain an authentication token by logging in using the /api/accounts/login endpoint.
3. Manage Profiles: Access profile-related functionalities through the /api/profiles endpoints.
4. User Tracking: Utilize the /api/users endpoints to track and manage users.

### API Documentation
The API endpoints and their functionalities are documented within the codebase. For detailed information, refer to the API documentation or the code comments.

### Technologies Used
- ASP.NET Core
- C#
- Entity Framework Core
- SQL Server 

### Contributing
Contributions are welcome! Feel free to open issues or submit pull requests.

### License
This software is licensed under the [MIT License](https://github.com/nhn/tui.editor/blob/master/LICENSE).

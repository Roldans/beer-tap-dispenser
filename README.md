# beer-tap-dispenser
Description of the project.

## Prerequisites

Before you can run the project locally, ensure you have the following prerequisites installed on your machine:

- [.NET 6.0 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)
- [Git](https://git-scm.com/downloads)
- [Your preferred code editor (e.g., Visual Studio, Visual Studio Code, etc.)](https://visualstudio.microsoft.com/)

## Getting Started

Follow the steps below to set up and run the project locally:

### 1. Clone the repository

Open your terminal or command prompt, navigate to your desired project directory, and clone the repository using Git:

´´´
git clone https://github.com/yourusername/your-repo.git
´´´

### 2. Open the project
Open your code editor and navigate to the project directory.

### 3. Install dependencies
In the terminal or command prompt, navigate to the project directory and restore the NuGet packages:

´´´
dotnet restore
´´´

### 4. Build the project
Build the project to ensure that everything is correctly set up:


dotnet build

### 5. Configure the database 
There is a MongoDB instance for the DAL in case that you want to persist the data, by default a "in memory DB" will be used. 

### 6. Run the project
To run the project locally, use the following command:

´´´
dotnet run
´´´
The application will start, and you should see a message indicating that the server is running on a specific port (e.g., https://localhost:7012).

### 7. Access the application
Open your web browser and navigate to the URL where the application is running with the swagger path https://localhost:7012/swagger/index.html. You should see swagger menue of the app
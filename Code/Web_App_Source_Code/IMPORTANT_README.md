# Deployment Status

## Frontend (Angular)
The Angular frontend has **not been successfully deployed** to Azure yet. 

For the time being, you will need to run the frontend individually and locally, the zip file **does not contain** all the required dependencies because unzipping such a file might take too long on your PC, please follow the instructions in the [Running Frontend Locally](#running-frontend-locally) section below for installing and running the frontend.



## Backend (.NET)
The .NET backend has been successfully deployed to Azure and does **NOT** have to be run locally, it is currently accessible at:

**[https://web-app-t5-dev-aca2dahff0bkb5g9.westeurope-01.azurewebsites.net/api/user](https://web-app-t5-dev-aca2dahff0bkb5g9.westeurope-01.azurewebsites.net/api/user)**


The backend currently has CORS permissions configured to allow requests from:

- **http://localhost:4200** (for local frontend development)
- **https://localhost:4200** (for secure local frontend development)

## Important:
Once the frontend is deployed to Azure, the CORS configuration must be updated to allow requests from the deployed domain instead of localhost.

---

## Running Frontend Locally

To run the Angular frontend locally, follow these steps:

1. **Open `SMARTHOMEWEB.CLIENT` folder and Install Node.js and Angular CLI:**
   - Ensure you have Node.js installed. Download it from [Node.js Official Site](https://nodejs.org).
   - Install the Angular CLI globally by running:
     ```bash
     npm install -g @angular/cli
     ```
      If you're on a Mac or Linux, use the **sudo** command
      ```bash
      sudo npm install -g @angular/cli
      ```

2. **Clone the Repository:**
   - Clone the frontend repository to your local machine.

3. **Install Dependencies:**
   - Navigate to the project directory and run:
     ```bash
     npm install
     ```
   - If you're on a Mac or Linux, use this command beforehand:
      ``` bash
      sudo chown -R $(whoami) $(npm config get cache)
      ```

4. **Start the Development Server:**
   - Run the Angular development server:
     ```bash
     ng serve
     ```

      If you encounter more problems, execute these commands to clean the modules and re-install them:
      ``` bash
      rm -rf node_modules package-lock.json
      npm cache clean --force
      npm install
      ```

   - By default, the server runs on **http://localhost:4200** or **https://localhost:4200**.


5. **Test the Application:**
   - Ensure the frontend is functional by accessing the local server in your browser.
   - The frontend will communicate with the backend hosted at:
     **https://web-app-t5-dev-aca2dahff0bkb5g9.westeurope-01.azurewebsites.net**

   **YOU ONLY NEED TO RUN THE FRONTEND**

---

## Future Changes to CORS

- **Current Setting:** Backend allows requests from `http://localhost:4200` and `https://localhost:4200`.
- **After Frontend Deployment:** Go to Azure Portal and update the CORS configuration to allow requests from the frontend's Azure new domain.

To modify the CORS settings, update the backend's CORS configuration in the Azure Function App or in the application code (in the `appsettings.json` and `program.cs` files).

---

# Login credentials

Once the Application is running and you're on the Login page use these credentials to log in and **please not not delete the content** because the ID's of the registered homes, rooms and devices are **directly tied** with the IoT Hub!

**email :**       `helb`
**password :**    `helb`

You can however create a new account to add homes, rooms and devices, toggle them, remove them, etc.. But they will not be tied with the IoT Hub unless your register them, and toggling them will return a server error, because their **ID's do not match** with the ones that are registered on the IoT Hub.

---

If you encounter any issues or have questions, feel free to reach us, we'd be happy to help!

# SmartHome Technical Documentation


---

## Table of Contents
1. [Executive Summary](#1-executive-summary)
2. [System Overview](#2-system-overview)
3. [Architecture Overview](#3-architecture-overview)
4. [Detailed Component Design](#4-detailed-component-design)
5. [Data Flow](#5-data-flow)
6. [Application Security](#6-application-security)
7. [Scalability and Performance](#7-scalability-and-performance)
8. [Deployment Architecture](#8-deployment-architecture)
9. [Monitoring and Maintenance](#9-monitoring-and-maintenance)
10. [Appendix](#10-appendix)

---

> [!TODO]
>
> Retirer de la doc:

COULEURS MERMAID:

``````
classDef azure fill:#1B65E5,stroke:#FFDE59,stroke-width:2px,color:#FFDE59
classDef database fill:#13AA52,stroke:#FFF,stroke-width:2px,color:#FFF
``````


## 1. Executive Summary

### Purpose

The *SmartHome* project aims to provide a cloud-based IoT solution that enables users to remotely control and monitor their home devices in real-time through a web application. The system offers a secure, scalable, and responsive platform for smart home automation.

### Project Scope

- 🔐 User Authentication & Security
- ⚡ Real-Time Device Control 
- 🏠 Home Management System
- 🔄 IoT Device Integration

> [!IMPORTANT]
>
> Not In Scope
> ❌ Mobile Applications  
> ❌ Hardware Manufacturing  
> ❌ Third-party IoT Platforms Integration  
> ❌ Voice Control Features  
> ❌ AI/ML Implementation

---

## 2. System Overview

### Problem Statement

Modern homes require smart automation solutions that allow users to:

- Control devices remotely
- Monitor device states in real-time
- Manage multiple homes and rooms
- Ensure secure access to their devices

### Solution Highlights

The solution leverages several Azure services and modern web technologies:

``````mermaid
graph LR
    De[IoT Devices] <-->|via IoT Hub| Af[Azure Functions]
    Af -->|via SignalR| Wa[Web Application]
    Wa -->|Device Commands| Af
    Wa <-->|Collections Management| Md[(MongoDB)]

	classDef azure fill:#1B65E5,stroke:#FFDE59,stroke-width:2px,color:#FFDE59
	classDef database fill:#13AA52,stroke:#FFF,stroke-width:2px,color:#FFF
	
	class Af azure
	class Md database
``````

| Azure IoT Hub                                                | Azure Functions                                              | SignalR                                                      |
| ------------------------------------------------------------ | ------------------------------------------------------------ | ------------------------------------------------------------ |
| Manages bi-directional communication between devices and cloud | Handles device state changes and manages communication between IoT Hub and SignalR | Enables real-time communication between the web application and cloud services |

---

## 3. Architecture Overview

### Logic Architecture Diagram
The system follows a microservices architecture pattern, integrating IoT devices, cloud services, and web applications.

```mermaid
flowchart LR
    subgraph Devices["IoT Devices"]
        iot[("IoT\nDevices")]
    end
    subgraph Azure["Azure Cloud Services"]
        iothub["IoT Hub\n(Gateway)"]
        functions["Azure Functions\n(Event Processing)"]
        signalr["SignalR\n(Real-time)"]
    end
    subgraph Storage["Data Layer"]
        mongodb[("MongoDB\n(Device Data)")]
    end
    subgraph Frontend["Application Layer"]
        webapp["Web App\n(Front/Back)"]
    end
    %% Device to Cloud
    iot <-- MQTT protocol --> iothub
    iothub -- "Eventgrid Event Stream" --> functions
    functions -- "HTTP protocol" --> iothub
    functions --> signalr
    signalr -- "Real-time Updates" --> webapp
    %% Data Storage
    webapp <--> mongodb
    %% Styling
    classDef azure fill:#1B65E5,stroke:#FFDE59,stroke-width:2px,color:#FFDE59
    classDef database fill:#13AA52,stroke:#FFF,stroke-width:2px,color:#FFF
    class iothub,functions,signalr azure
    class mongodb database
```

### Container Description

#### Azure IoT Hub
- **Purpose**: Manages IoT device communication
- **Features**:
  - Device registration
  - Message routing
  - Device twins
  - Security management
- **Configuration**: Configured in West Europe region with standard tier

#### Azure Functions
- **Implementation**: .NET 8.0
- **Triggers**: 
  - HTTP triggers for API endpoints
  - Event Grid triggers for IoT events
- **Key Functions**:
  - ReceiveFromDevice
  - SendToDevice
  - Negotiate (SignalR)

#### SignalR
- **Purpose**: Real-time communication
- **Features**:
  - WebSocket support
  - Fallback to long polling
  - Client groups
  - Connection management
- **Integration**: Azure SignalR Service

#### WebApp
- **Framework**: Angular
- **Features**:
  - Device control interface
  - Real-time updates while coding
  - Responsive design
  - Frontend integrates well with .NET's Backend

#### MongoDB
- **Purpose**: Data persistence
- **Collections**:
    - Users
        - User
        - Home
        - Room
        - Device
- **Schema**: Document-based with nested objects

---

## 4. Detailed Component Design

### Components Description
- **IoT Devices**: Edge devices sending telemetry data.
- **Azure IoT Hub**: Manages device communication.
- **Azure Functions**: Processes data and triggers real-time updates.
- **SignalR Service**: Enables real-time updates to the web app.
- **Web Application**: User-facing dashboard for monitoring and control.

### IoT Devices
- Protocols: MQTT, AMQP, or HTTP.
- Data format: JSON.

### Azure IoT Hub
Azure IoT Hub serves as the central communication hub between IoT devices and the cloud. It enables device provisioning, authentication, and seamless message routing.

- **Device Provisioning and Authentication**: IoT Hub supports secure device connections using shared access signatures, X.509 certificates, or device identity tokens. This ensures devices are authenticated and authorized before exchanging data.
- **Message Routing and Telemetry Ingestion**: Incoming telemetry data from devices is routed based on predefined rules. IoT Hub integrates seamlessly with Azure Event Grid, Azure Functions, and other services for processing and analytics.

### Azure Functions

This section provides a technical breakdown of the Azure Functions implemented in the `EventGridTriggerT5` class. These functions are critical to enabling device communication, real-time updates, and client connectivity.

#### **ReceiveFromDevice**

- **Trigger**: This function is triggered by an Event Grid event containing telemetry data sent by IoT devices.
- **Parameters**:
  - `EventGridEvent eventGridEvent`: The payload received from Azure Event Grid, containing data from the IoT Hub.
- **Functionality**:
  1. Extracts the `Data` property from the `EventGridEvent` payload and deserializes it into a JSON object.
  2. Checks for the presence of a `body` property in the deserialized JSON. If absent or empty, logs an informational message and terminates execution.
  3. If valid data is present in the `body`, sends the telemetry data to all connected clients through SignalR using the `ReceiveTelemetry` event.
- **Error Handling**:
  - Logs errors encountered during JSON deserialization or data processing.
- **Integration**:
  - Bridges IoT devices and the SignalR clients (e.g., Angular web app) by transmitting telemetry data in real-time.

#### **SendToDevice**

- **Trigger**: Triggered via an HTTP POST request to send commands to a specific IoT device.
- **Parameters**:
  - `HttpRequestData req`: The HTTP request containing the JSON payload with command details.
- **Functionality**:
  1. Reads the request body and deserializes it into an `IotDeviceMessage` object. If the body is empty or deserialization fails, returns a `400 Bad Request` response.
  2. Constructs an Azure IoT Hub message using the deserialized payload. The message is encoded as JSON with UTF-8 encoding.
  3. Sends the constructed message to the IoT Hub for delivery to the specified device using the `DeviceId` property.
  4. Logs a success message upon successful delivery and responds with `200 OK`. In case of failure, logs the error and returns a `500 Internal Server Error` response.
- **Error Handling**:
  - Validates the request body and provides detailed error responses for missing or invalid payloads.
  - Logs and returns detailed errors for any failures during message sending.
- **Integration**:
  - Facilitates two-way communication by allowing the Angular web app to send commands to IoT devices via the IoT Hub.

#### **Negotiate**

- **Trigger**: Triggered via an HTTP POST request to provide SignalR connection details to clients.
- **Parameters**:
  - `HttpRequestData req`: The HTTP request triggering the function.
- **Functionality**:
  1. Retrieves the SignalR endpoint URL for the `deviceHub` hub using the `IServiceManager` API.
  2. Generates a client access token for the `deviceHub` hub using the `GenerateClientAccessToken` method.
  3. Constructs a JSON response containing the SignalR endpoint URL and the access token.
  4. Returns the response with a `200 OK` status.
- **Error Handling**:
  - Catches exceptions during SignalR endpoint retrieval or token generation, logs the error, and returns a `500 Internal Server Error` response.
- **Integration**:
  - Ensures that the Angular web app can establish secure connections to SignalR for real-time updates.


### SignalR Service

The SignalR service is tightly integrated with the `ReceiveFromDevice` and `Negotiate` functions to enable real-time communication between the web application and the backend.

- **Broadcasting**: The `ReceiveFromDevice` function sends telemetry updates to all connected clients using the `ReceiveTelemetry` event.
- **Connection Management**: The `Negotiate` function generates access tokens, ensuring secure and authenticated client connections to SignalR.
- **Scalability**: SignalR dynamically handles large numbers of simultaneous client connections and maintains high performance under load.


> [!TODO]
### Web Application
- Frontend: React or Angular.
- Integration: REST APIs and SignalR.
> [!TODO]

---

## 5. Data Flow

### 
```mermaid
sequenceDiagram
    participant D as Device
    participant I as IoT Hub
    participant F as Azure Functions
    participant S as SignalR
    participant W as WebApp
    participant M as MongoDB

    D->>I: Device State Update
    I->>F: Event Grid Trigger
    F->>M: Update State
    F->>S: Broadcast Update
    S->>W: Real-time Update
    W->>F: Device Command
    F->>I: Command Message
    I->>D: Execute Command
```

### JSON Message Structure
Device State Message:
```json
{
  "deviceId": "device123",
  "homeId": "home456",
  "roomId": "room789",
  "state": true,
  "timestamp": "2024-12-15T10:00:00Z"
}
```

### Database Schema
```json
{
  "user": {
    "userId": "string",
    "email": "string",
    "password": "string",
    "homes": [
      {
        "homeId": "string",
        "nickname": "string",
        "rooms": [
          {
            "roomId": "string",
            "name": "string",
            "devices": [
              {
                "deviceId": "string",
                "description": "string",
                "state": "boolean"
              }
            ]
          }
        ]
      }
    ]
  }
}
```
---

## 6. Application Security

The application implements several security measures:
- Session-based authentication
- HTTPS communication
- Azure IoT Hub device authentication
- Input validation and sanitization

---

## 7. Monitoring and Maintenance

> [!TODO]

--- 
## 8. Deployment Architecture

> [!TODO]
>
> Github h3
>
> - smarthome web app
>   - subdirectoy Client --> angular frontend project
>   - subdirectory Server --> angular backend project
> - azure function http trigger (keep it because could be necessary if problems with eventgrid)
> - azure function eventgrid trigger

### Environment Setup

1. Azure Resources:
   - IoT Hub
   - Function App
   - SignalR Service
   - MongoDB Atlas

2. Development Environment:
   - Visual Studio 2022
   - .NET 8.0 SDK
   - Node.js and Angular CLI
   - Azure Functions Core Tools

---

## 9. Monitoring and Maintenance

> [!TODO]

---



## 10. Appendix

### References
- [Azure IoT Hub Documentation](https://docs.microsoft.com/en-us/azure/iot-hub/)
- [Azure Functions Documentation](https://docs.microsoft.com/en-us/azure/azure-functions/)
- [SignalR Documentation](https://docs.microsoft.com/en-us/aspnet/core/signalr/)
- [MongoDB Documentation](https://docs.mongodb.com/)

### Attachments 

> [!Important]
>
> -- Attach the code of the azure function
> ---- Complete code or part of it, as file with reference link or in code block ?

> [!IMPORTANT]
>
> Code Samples necessary if already explained before in documentation

---

### Team Members
- Alain M. Nitunga
- Elie Kheirallah
- Farouk Ait Oujkal
- Jamal Assou
- Léo Calvo Castaño

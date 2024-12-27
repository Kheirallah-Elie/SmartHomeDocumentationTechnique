# IoT Web Application Documentation

## Table of Contents
1. [Executive Summary](#executive-summary)
2. [System Overview](#system-overview)
3. [Architecture Overview](#architecture-overview)
4. [Detailed Component Design](#detailed-component-design)
5. [Data Flow](#data-flow)
6. [Security Architecture](#security-architecture)
7. [Scalability and Performance](#scalability-and-performance)
8. [Deployment Architecture](#deployment-architecture)
9. [Monitoring and Maintenance](#monitoring-and-maintenance)
10. [Appendix](#appendix)

---

## Executive Summary
### Purpose
Describe the purpose of the IoT web application and its key objectives.

### Scope
Define the scope of the solution, including primary features and target audience.

### Audience
Specify who this document is for (e.g., developers, architects, stakeholders).

## System Overview
### Problem Statement
What challenges does the application solve?

### Solution Highlights
Provide a high-level description of how the solution addresses these challenges:
- Real-time updates with **SignalR**.
- Device integration via **Azure IoT Hub**.
- Serverless backend using **Azure Functions**.

## Architecture Overview
### Logical Architecture
Describe the high-level architecture of the application.

![Logical Architecture](./images/logical-architecture.png)

### Components Description
- **IoT Devices**: Edge devices sending telemetry data.
- **Azure IoT Hub**: Manages device communication.
- **Azure Functions**: Processes data and triggers real-time updates.
- **SignalR Service**: Enables real-time updates to the web app.
- **Web Application**: User-facing dashboard for monitoring and control.

## Detailed Component Design
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

---

### SignalR Service

The SignalR service is tightly integrated with the `ReceiveFromDevice` and `Negotiate` functions to enable real-time communication between the web application and the backend.

- **Broadcasting**: The `ReceiveFromDevice` function sends telemetry updates to all connected clients using the `ReceiveTelemetry` event.
- **Connection Management**: The `Negotiate` function generates access tokens, ensuring secure and authenticated client connections to SignalR.
- **Scalability**: SignalR dynamically handles large numbers of simultaneous client connections and maintains high performance under load.

### Web Application
- Frontend: React or Angular.
- Integration: REST APIs and SignalR.

## Data Flow
### End-to-End Data Flow
- From IOT devices -> IoT Hub -> Azure Functions -> SignalR -> Web Application -> MongoDB
- JSON Message structure
- Database Data Modeling

## Application Security
- Autentication and authorization
- Token-based authentication
- Data encryption in transit/at rest

## Deployment Architecture
- CI/CD Pipeline:
    - Integration with tools GitHub/Git Lab
    - Steps for building, testing, and deploying the application.
- Environment Setup: Azure environment configuration, resource 
names, endpoints

## Monitoring and Maintenance

## Appendix
- **References**: Links to Azure documentation for IoT Hub, SignalR, and Azure Functions.
- **Glossay**: Define technical terms for non-technical readers.
- **Code Samples**: Example configurations for IoT Hub routing, SignalR hubs, and Azure Functions.

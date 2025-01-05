# SmartHome - Technical Documentation Structure

## 1. Executive Summary

### Purpose
Define the primary objectives of the application: provide a real-time, serverless web interface for managing IoT devices at home.

### Scope
- Key features based on epics and user stories from the JIRA backlog.
- Include a brief overview or an export in the appendix to avoid overloading this section.

### Audience
- **Stakeholders**:
  - **Primary Client**: End users of the solution.
  - **Professors**: For academic evaluation.
  - **Technical Team**: Developers maintaining or enhancing the system.

---

## 2. System Overview

### Problem Statement
Reformulate the addressed problem: enabling smooth and centralized IoT device management, integrated with the cloud ecosystem (Azure).

### Solution Highlights
High-level summary of the solution:
- **Azure IoT Hub**: Data collection and centralization for IoT devices.
- **Azure Functions**: Message processing and orchestration.
- **SignalR**: Real-time communication management.
- **Web App**: Responsive and intuitive user interface.

### Diagrams
- **Context Diagram (C4)**: Overview of the system as a black box.
  - External actors: users, connected systems.
  - Use Structurizr to create this diagram.

---

## 3. Architecture Overview

### Logical Architecture Diagram
High-level representation of major components:
- Interactions between IoT Hub, Azure Functions, SignalR, Web App, and MongoDB.

### Containers Description
Details for each Structurizr container:
- **Azure IoT Hub**: Management of connected devices.
- **Azure Functions**: Trigger points, input/output bindings.
- **SignalR**: Real-time connection management.
- **Web App**: Angular-based application integrated with SignalR.
- **MongoDB**: Persistent storage for necessary data.

---

## 4. Detailed Component Design

### Components
Detailed look into key components using **Component Diagrams (C4)**:
1. **Azure IoT Hub**: Processing incoming messages.
2. **Azure Functions**: Trigger mechanisms, exchanged JSON structures.
3. **SignalR**: Coordination of real-time data between backend and frontend.
4. **Web App**: Angular architecture and SignalR integration.

### Explanations
Describe how each component interacts:
- Dependencies.
- Integration points.

---

## 5. Data Flow

### Flow Diagram
Represent data flow across layers:
1. **IoT Devices → IoT Hub**: Incoming messages.
2. **IoT Hub → Azure Functions**: Processing and transformation.
3. **Azure Functions → SignalR → Web App**: Real-time updates.
4. **SignalR → MongoDB**: Persistent data storage.

### JSON Message Structure
Include JSON examples to illustrate used formats.

### Database
Describe MongoDB data model with diagrams or examples.

---

## 6. Application Security

### Authentication and Authorization
- Use of secure tokens (Token-Based Authentication).
- Role-based authorization management.

### Data Encryption
- Explain encryption mechanisms for data in transit and at rest.

---

## 7. Deployment Architecture

### GitHub
- Structure and management of the GitHub repository.

### Environment Setup
- Azure resource configurations (names, endpoints).
- Steps to redeploy the application.

### Structurizr
- Use Structurizr to maintain diagrams synchronized with the code.
- Document setup steps for Structurizr (e.g., Docker version, Git integration).

---

## 8. Appendix

### References
- Official documentation on Azure IoT Hub, SignalR, Azure Functions.
- Useful links for Structurizr and C4 model:
  - [Structurizr DSL](https://structurizr.com/dsl)
  - [Structurizr DSL Cookbook](https://structurizr.com/help/cookbook).

### Attachments
- Azure Functions code for concrete examples.

### Code Samples
- Relevant snippets for quick understanding.

----


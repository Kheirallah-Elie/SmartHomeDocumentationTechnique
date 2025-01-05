workspace "SmartHome System Detailed" "C4 Diagrams with Component Details" {
  model {
    user = person "User" {
        description "Interacts directly with IoT devices or remotely via the web application."
    }

    iotDevices = softwareSystem "IoT Devices" {
        description "Physical IoT devices"
    }

    iotSystem = softwareSystem "IoT System" {
        description "Components of the 'SmartHome' solution."

        iotHub = container "IoT Hub" {
            description "IoT gateway"
            technology "Azure IoT Hub"
        }

        azureFunctions = container "Azure Functions" {
            description "Azure serverless functions"
            technology "Azure Functions"

            receiveFromDevice = component "ReceiveFromDevice" {
                description "Handles telemetry from devices"
                technology "Azure Function"
            }

            sendToDevice = component "SendToDevice" {
                description "Sends commands to devices"
                technology "Azure Function"
            }

            negotiate = component "Negotiate" {
                description "Manages SignalR client connections"
                technology "Azure Function"
            }
        }

        signalR = container "SignalR" {
            description "Real-time notifications"
            technology "Azure SignalR"
        }

        appServices = container "AppServices" {
            description "Application services"
            technology "Azure App Services"

            frontend = component "Frontend" {
                description "User interface for interacting with IoT devices"
                technology "Angular"
            }

            backend = component "Backend" {
                description "Provides REST endpoints for CRUD operations on MongoDb"
                technology "REST API"
            }
        }
        
        mongoDB = container "MongoDB" {
            description "Database for storing data"
            technology "MongoDB"

            usersCollection = component "UsersCollection" {
                description "Stores users' data, homes and devices"
                technology "MongoDB Collection"
            }
        }
    }
    
    // Context level
    user -> iotDevices "Controls"
    
    // Container level
    iotDevices -> iotHub "Send telemetry"
    iotHub -> iotDevices "Delivers commands"
    
    // Component level
        // First frontend connection
    negotiate -> signalR "[On frontend initialization]\nEstablishes SignalR connection"
    
        // From User to IoT
    user -> frontend "Interacts with"
    frontend -> sendToDevice "Sends commands through HTTP POST request"
    backend -> usersCollection "Stores users’ and their devices’ data"
    sendToDevice -> iotHub "Broadcasts commands"

        // From IoT to User
    iotHub -> receiveFromDevice "Triggers an EventGrid event"
    receiveFromDevice -> signalR "Broadcasts the event"
    signalR -> frontend "Delivers the real-time update"
    frontend -> backend "Calls API endpoint to update database devices' information"
  }

  views {
    systemContext iotSystem "ContextDiagram" {
        include *
        autolayout lr
    }

    container iotSystem "ContainerDiagram" {
        include *
        autolayout lr
    }

    component azureFunctions "AzureFunctionsComponentDiagram" {
        include *
        autolayout lr
    }

    component appServices "AppServicesComponentDiagram" {
        include *
        autolayout lr
    }

    component mongoDB "MongoDBComponentDiagram" {
        include backend
        include usersCollection
        autolayout lr
    }

    theme default
  }

  configuration {
    scope softwareSystem
  }
}

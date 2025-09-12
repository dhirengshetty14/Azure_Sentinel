// This file defines the Azure SignalR Service for real-time communication.
@description('The name of the SignalR service.')
param signalRName string

@description('The location where the resources will be deployed.')
param location string = resourceGroup().location

// Define the SignalR resource
resource signalR 'Microsoft.SignalRService/signalR@2023-02-01' = {
  name: signalRName
  location: location
  sku: {
    name: 'Free_F1' // Use the free tier
    tier: 'Free'
    capacity: 1
  }
  kind: 'SignalR'
  properties: {
    features: [
      {
        flag: 'ServiceMode'
        value: 'Default' // Allows both client-server and serverless connections
      }
    ]
  }
}

// Output the connection string so our API can connect to SignalR
@description('The connection string for the SignalR service.')
output connectionString string = listKeys(signalR.id, '2023-02-01').primaryConnectionString
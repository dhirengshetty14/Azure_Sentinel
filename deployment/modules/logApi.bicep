// This file defines the Azure App Service to host our .NET Log Ingestor API.
@description('The name of the API web app.')
param apiAppName string

@description('The ID of the App Service Plan to host this app.')
param appServicePlanId string

@description('The connection string for Cosmos DB.')
@secure() // Marks the parameter as secure so it's not logged
param cosmosConnectionString string

@description('The connection string for SignalR.')
@secure()
param signalRConnectionString string

@description('The location where the resources will be deployed.')
param location string = resourceGroup().location

// Define the Web App resource
resource apiApp 'Microsoft.Web/sites@2022-09-01' = {
  name: apiAppName
  location: location
  kind: 'app,linux'
  properties: {
    serverFarmId: appServicePlanId
    siteConfig: {
      linuxFxVersion: 'DOTNETCORE|8.0' // Specify the .NET 8 runtime
      appSettings: [
        // These settings are injected as environment variables into our application
        {
          name: 'ConnectionStrings__CosmosDb'
          value: cosmosConnectionString
        }
        {
          name: 'ConnectionStrings__SignalR'
          value: signalRConnectionString
        }
        {
          name: 'ASPNETCORE_ENVIRONMENT'
          value: 'Production'
        }
      ]
    }
  }
}

// Output the public URL of our API
@description('The hostname of the deployed API app.')
output apiHostName string = apiApp.properties.defaultHostName
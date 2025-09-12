// This file defines our NoSQL database using Azure Cosmos DB.
// It's perfect for storing unstructured log data.
@description('The name for the Cosmos DB account. Must be globally unique.')
param cosmosAccountName string

@description('The location where the resources will be deployed.')
param location string = resourceGroup().location

// Define the top-level Cosmos DB account
resource cosmosAccount 'Microsoft.DocumentDB/databaseAccounts@2023-04-15' = {
  name: cosmosAccountName
  location: location
  kind: 'GlobalDocumentDB'
  properties: {
    databaseAccountOfferType: 'Standard'
    locations: [
      {
        locationName: location
        failoverPriority: 0
      }
    ]
  }
}

// Define the database within the account
resource cosmosDatabase 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases@2023-04-15' = {
  parent: cosmosAccount
  name: 'LogDatabase'
  properties: {
    resource: {
      id: 'LogDatabase'
    }
  }
}

resource cosmosContainer 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers@2023-04-15' = {
  parent: cosmosDatabase
  name: 'LogEntries'
  properties: {
    resource: {
      id: 'LogEntries'
      partitionKey: {
        paths: [
          '/sourceApplication' // Partitioning by source app is a good strategy
        ]
        kind: 'Hash'
      }
    }
  }
}

// Output the connection string so our API can connect to the database
@description('The connection string for the Cosmos DB account.')
output connectionString string = listConnectionStrings(cosmosAccount.id, '2023-04-15').connectionStrings[0].connectionString
// We're using a Linux plan on the Free (F1) tier to keep costs down.
@description('The name of the App Service Plan.')
param appServicePlanName string

@description('The location where the resources will be deployed.')
param location string = resourceGroup().location

// Define the App Service Plan resource
resource appServicePlan 'Microsoft.Web/serverfarms@2022-09-01' = {
  name: appServicePlanName
  location: location
  sku: {
    name: 'F1' // F1 is the Free tier
  }
  kind: 'linux'
  properties: {
    reserved: true // Required for Linux plans
  }
}

@description('The resource ID of the created App Service Plan.')
output appServicePlanId string = appServicePlan.id
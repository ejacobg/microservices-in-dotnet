#!/bin/bash

# Create a resource group.
az group create --name ShoppingCart --location westus

# Create a container registry. The registry name must be unique within Azure, and contain 5-50 lowercase alphanumeric characters.
# https://learn.microsoft.com/en-us/azure/container-registry/container-registry-get-started-azure-cli#create-a-container-registry
# Check if a name is available: https://learn.microsoft.com/en-us/rest/api/containerregistry/registries/check-name-availability?tabs=HTTP
az acr create --resource-group ShoppingCart --name ejacobgshoppingcartregistry --sku Basic

# Create an AKS cluster.
# More on creating a cluster: https://learn.microsoft.com/en-us/azure/aks/learn/quick-kubernetes-deploy-cli
# I got a "subscription is not registered to use namespace 'microsoft.insights'" error. Fix: https://aidanfinn.com/?p=21192#:~:text=Or%20you%20can%20just%20use%20the%20Azure%20Portal.%20Browse%20to%20Subscriptions%20%3E%20select%20your%20subscription%20%3E%20Resource%20Providers%20(under%20Settings).%20Here%20you%20can%20see%20the%20registration%20status%20of%20the%20provider%2C%20and%20you%20can%20register%20the%20provider%20in%20the%20GUI%3A
az aks create --resource-group ShoppingCart --name ShoppingCartAKSCluster --node-count 1 --enable-addons monitoring --generate-ssh-keys --attach-acr ejacobgshoppingcartregistry

# Log kubectl into the cluster.
az aks get-credentials --resource-group ShoppingCart --name ShoppingCartAKSCluster

kubectl get nodes

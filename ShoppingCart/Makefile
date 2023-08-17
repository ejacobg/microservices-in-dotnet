# ==================================================================================== #
# DOCKER
# ==================================================================================== #

.PHONY: docker/build
docker/build:
	docker build . -t shopping-cart # This may show a "SECURITY WARNING" error on Windows. This is expected.

.PHONY: docker/run
docker/run:
	docker run --name shopping-cart --rm -p 5000:80 shopping-cart
	# --name runs the named container.
	# --rm removes the container upon exiting, useful to prevent polluting with old containers.
	# --p 5000:80 exposes the container on port 5000 and listens for requests. These requests get forwarded to port 80 inside the container.

.PHONY: docker/build-and-run
docker/build-and-run: docker/build docker/run

.PHONY: docker/stop
docker/stop:
	docker stop shopping-cart

# Apply this tag before pushing your container to your container registry.
.PHONY: docker/tag
docker/tag:
	docker tag shopping-cart ejacobgshoppingcartregistry.azurecr.io/shopping-cart:1.0.0

.PHONY: docker/push-registry
docker/push-registry:
	docker push ejacobgshoppingcartregistry.azurecr.io/shopping-cart:1.0.0

# ==================================================================================== #
# KUBERNETES
# ==================================================================================== #

# Make sure "Enable Kubernetes" is enabled in your Docker Desktop settings.
# Make sure that you're using the "image: shopping-cart" line in shopping-cart.yaml.
# Confirm that you're in the correct context (ie. AKS or localhost) before using these targets. 

.PHONY: k8s/deploy
k8s/deploy:
	kubectl apply -f shopping-cart.yaml

# Check that your deployments have succeeded.
.PHONY: k8s/check
k8s/check:
	kubectl get all

.PHONY: k8s/delete
k8s/delete:
	kubectl delete -f shopping-cart.yaml

# Use this target to show all your active contexts before switching between them.
.PHONY: k8s/contexts
k8s/contexts:
	kubectl config get-contexts

# If you're running both AKS and localhost clusters at the same time, you have to make sure that kubectl is pointing to the correct cluster before running your commands.
# Use this target to switch between your contexts.
.PHONY: k8s/switch
k8s/switch:
	kubectl config use-context ${context}

# ==================================================================================== #
# AZURE KUBERNETES SERVICE
# ==================================================================================== #

# Once your cluster is created, you can stop and start it from the Azure portal (portal.azure.com).
# Make sure that you're using the "image: ejacobgshoppingcartregistry.azurecr.io/shopping-cart:1.0.0" line in shopping-cart.yaml.

.PHONY: aks/create
aks/create:
	./create-aks.sh
	# Give the kube-system:kubernetes-dashboard account the cluster-admin role. This line may be omitted if you don't plan on using the dashboard.
	kubectl create clusterrolebinding kubernetes-dashboard \
		--clusterrole=cluster-admin \
		--serviceaccount=kube-system:kubernetes-dashboard # The Kubernetes dashboard is bound to this account, so it now has admin rights.

# View the dashboard.
.PHONY: aks/dashboard
aks/dashboard:
	az aks browse --resource-group ShoppingCart --name ShoppingCartAKSCluster

# Note: deletion may take some time. Wait a bit before trying to recreate your cluster.
.PHONY: aks/delete
aks/delete:
	az group delete --name ShoppingCart --yes --no-wait

# Authenticate your Docker client with your container registry.
.PHONY: aks/acr-login
aks/acr-login:
	az acr login --name ejacobgshoppingcartregistry
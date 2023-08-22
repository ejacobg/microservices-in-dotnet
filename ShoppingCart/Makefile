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

.PHONY: docker/pull-sql-server
docker/pull-sql-server:
	docker pull mcr.microsoft.com/mssql/server

# After starting the database, make sure to apply the migrations in the database-setup.sql file.
.PHONY: docker/run-sql-server
docker/run-sql-server:
	# If you wish to change the password, you have to update your connection strings (see Models/SqlEventStore.cs) to match.
	docker run -e 'ACCEPT_EULA=Y' -e 'SA_PASSWORD=yourStrong(!)Password' -p 1433:1433 -d mcr.microsoft.com/mssql/server
	# SA_PASSWORD is deprecated? https://learn.microsoft.com/en-us/sql/linux/quickstart-install-connect-docker?view=sql-server-ver16&pivots=cs1-bash#pullandrun2022

# If you want to stop running the SQL Server or EventStore containers, use `docker stop <name>` using the name from this command.
.PHONY: docker/check
docker/check:
	docker ps

.PHONY: docker/pull-eventstore
docker/pull-eventstore:
	docker pull eventstore/eventstore

# Runs EventStore on http://localhost:2113. If running correctly, a login page should be displayed. Enter username "admin" and password "changeit".
.PHONY: docker/run-eventstore
docker/run-eventstore:
	# If you're getting TLS errors, use the --insecure flag. Do not do this in production.
	docker run --name eventstore-node -it -p 2113:2113 -p 1113:1113 --rm eventstore/eventstore:latest --run-projections All --enable-external-tcp --enable-atom-pub-over-http --insecure

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
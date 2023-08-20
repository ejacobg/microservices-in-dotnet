# ==================================================================================== #
# DOCKER
# ==================================================================================== #

.PHONY: docker/build-loyalty-program
docker/build-loyalty-program:
	docker build . -t loyalty-program -t ejacobgshoppingcartregistry.azurecr.io/loyalty-program:1.0.2

.PHONY: docker/push-loyalty-program
docker/push-loyalty-program:
	docker push ejacobgshoppingcartregistry.azurecr.io/loyalty-program:1.0.2

.PHONY: docker/build-special-offers
docker/build-special-offers:
	docker build ./SpecialOffers -t special-offers -t ejacobgshoppingcartregistry.azurecr.io/special-offers:1.0.0

.PHONY: docker/push-special-offers
docker/push-special-offers:
	docker push ejacobgshoppingcartregistry.azurecr.io/special-offers:1.0.0

.PHONY: docker/build-all
docker/build-all: docker/build-loyalty-program docker/build-special-offers

.PHONY: docker/push-all
docker/push-all: docker/push-loyalty-program docker/push-special-offers

# ==================================================================================== #
# KUBERNETES
# ==================================================================================== #

# Make sure "Enable Kubernetes" is enabled in your Docker Desktop settings.
# Make sure that your container images are correct in your manifest files.
# Confirm that you're in the correct context (ie. AKS or localhost) before using these targets. 

.PHONY: k8s/deploy-loyalty-program
k8s/deploy-loyalty-program:
	kubectl apply -f loyalty-program.yaml

.PHONY: k8s/delete-loyalty-program
k8s/delete-loyalty-program:
	kubectl delete -f loyalty-program.yaml

.PHONY: k8s/deploy-special-offers
k8s/deploy-special-offers:
	kubectl apply -f ./SpecialOffers/special-offers.yaml

.PHONY: k8s/delete-special-offers
k8s/delete-special-offers:
	kubectl delete -f ./SpecialOffers/special-offers.yaml

.PHONY: k8s/deploy-all
k8s/deploy-all: k8s/deploy-loyalty-program k8s/deploy-special-offers

.PHONY: k8s/check
k8s/check:
	kubectl get pods

.PHONY: k8s/delete-all
k8s/delete-all: k8s/delete-loyalty-program k8s/delete-special-offers

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

# Authenticate your Docker client with your container registry.
.PHONY: aks/acr-login
aks/acr-login:
	az acr login --name ejacobgshoppingcartregistry

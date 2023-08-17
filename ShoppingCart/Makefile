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

.PHONY: k8s/deploy
k8s/deploy:
	kubectl apply -f shopping-cart.yaml

.PHONY: k8s/check
k8s/check:
	kubectl get all

.PHONY: k8s/delete
k8s/delete:
	kubectl delete -f shopping-cart.yaml

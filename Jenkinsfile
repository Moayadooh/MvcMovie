pipeline {
  agent any

  environment {
    REGISTRY = "localhost:5006"
    WEB_APP_IMAGE_NAME = "web-app"
    API_APP_IMAGE_NAME = "api-app"

    DEPLOYMENT_FILE = "deployment.yml"
    SERVICE_FILE = "service.yml"
    NAMESPACE = "default"

    // Use mounted kubeconfig in Jenkins container
    KUBECONFIG = "/root/.kube/config"
  }

  stages {
    stage('Checkout') {
      steps {
        checkout scm
      }
    }

    stage('Build Docker Images') {
      steps {
        sh '''
          echo "Building Docker Images..."

          # Set Docker to use Minikube daemon
          eval $(minikube -p minikube docker-env)

          # Build API App image
          docker build -t $API_APP_IMAGE_NAME:latest ./crud-api

          # Build Web App image
          docker build -t $WEB_APP_IMAGE_NAME:latest .
        '''
      }
    }

    stage('Push to Local Registry') {
      steps {
        sh '''
          echo "Tagging and pushing Docker images to local registry..."

          # Tag images for local registry
          docker tag $API_APP_IMAGE_NAME:latest $REGISTRY/$API_APP_IMAGE_NAME:latest
          docker tag $WEB_APP_IMAGE_NAME:latest $REGISTRY/$WEB_APP_IMAGE_NAME:latest

          # Push to registry
          docker push $REGISTRY/$API_APP_IMAGE_NAME:latest
          docker push $REGISTRY/$WEB_APP_IMAGE_NAME:latest
        '''
      }
    }

    stage('Deploy to Kubernetes') {
      steps {
        sh '''
          echo "Deploying to Kubernetes..."

          # Update deployment file to use latest images
          sed -i "s|image: .*/api-app:.*|image: $REGISTRY/$API_APP_IMAGE_NAME:latest|g" $DEPLOYMENT_FILE
          sed -i "s|image: .*/web-app:.*|image: $REGISTRY/$WEB_APP_IMAGE_NAME:latest|g" $DEPLOYMENT_FILE

          # Apply Kubernetes manifests
          kubectl apply -f $DEPLOYMENT_FILE --namespace=$NAMESPACE
          kubectl apply -f $SERVICE_FILE --namespace=$NAMESPACE

          # Wait for rollout to complete
          kubectl rollout status deployment/api-app --namespace=$NAMESPACE
          kubectl rollout status deployment/web-app --namespace=$NAMESPACE
        '''
      }
    }
  }
}

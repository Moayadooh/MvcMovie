pipeline {
  agent any

  environment {
    REGISTRY = "10.111.254.158:30000"
    WEB_APP_IMAGE_NAME = "web-app"
    API_APP_IMAGE_NAME = "api-app"

    DEPLOYMENT_FILE = "deployment.yml"
    SERVICE_FILE = "service.yml"
    NAMESPACE = "default"

    // Path to kubeconfig mounted in Jenkins container
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
          set -e
          echo "Building Docker Images..."

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
          set -e
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
          set -e
          echo "Deploying to Kubernetes..."

          # Ensure kubeconfig is set
          export KUBECONFIG=$KUBECONFIG

          # Update deployment file with latest images
          sed -i "s|image: .*/api-app:.*|image: $REGISTRY/$API_APP_IMAGE_NAME:latest|g" $DEPLOYMENT_FILE
          sed -i "s|image: .*/web-app:.*|image: $REGISTRY/$WEB_APP_IMAGE_NAME:latest|g" $DEPLOYMENT_FILE

          # Apply manifests
          kubectl apply -f $DEPLOYMENT_FILE --namespace=$NAMESPACE --validate=false
          kubectl apply -f $SERVICE_FILE --namespace=$NAMESPACE

          # Wait for rollout to complete
          kubectl rollout status deployment/api-app --namespace=$NAMESPACE --timeout=2m
          kubectl rollout status deployment/web-app --namespace=$NAMESPACE --timeout=2m
        '''
      }
    }
  }
}

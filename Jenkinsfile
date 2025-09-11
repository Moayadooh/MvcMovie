pipeline {
  agent any

  environment {
    REGISTRY = "localhost:5006"

    // Images names
    WEB_APP_IMAGE_NAME = "web-app"
    API_APP_IMAGE_NAME = "api-app"

    // Container names
    WEB_APP_CONTAINER_NAME = "web-app-container"
    API_APP_CONTAINER_NAME = "api-app-container"
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
          docker build -f Dockerfile.ApiApp -t $API_APP_IMAGE_NAME:latest ./crud-api

          # Build Web App image
          docker build -f Dockerfile.WebApp -t $WEB_APP_IMAGE_NAME:latest .
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

    stage('Deploy Containers') {
        steps {
            sh '''
                # Get or create internal network
                NETWORK_NAME=$(docker network ls --format "{{.Name}}" | grep internal-network || echo "$REGISTRY"_internal-network)
                docker network inspect $NETWORK_NAME >/dev/null 2>&1 || docker network create $NETWORK_NAME
                echo "Using network: $NETWORK_NAME"

                # Remove old containers
                docker rm -f $API_APP_CONTAINER_NAME || true
                docker rm -f $WEB_APP_CONTAINER_NAME || true

                # Run new containers
                docker run -d --name $API_APP_CONTAINER_NAME --network $NETWORK_NAME -p 3000:3000 $REGISTRY/$API_APP_IMAGE_NAME:latest
                docker run -d --name $WEB_APP_CONTAINER_NAME --network $NETWORK_NAME -p 5005:5005 $REGISTRY/$WEB_APP_IMAGE_NAME:latest
            '''
        }
    }
  }
}

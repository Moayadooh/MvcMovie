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
          docker build -t $API_APP_IMAGE_NAME:latest -f ./crud-api/Dockerfile.ApiApp ./crud-api

          # Build Web App image
          docker build -t $WEB_APP_IMAGE_NAME:latest -f Dockerfile.WebApp .
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

    stage('Deploy Containers with Docker Compose') {
        steps {
            sh '''
                echo "Bringing down old containers..."
                docker compose -f docker-compose.apps.yml down

                echo "Deploying new containers..."
                docker compose -f docker-compose.apps.yml up -d --build
            '''
        }
    }
  }
}

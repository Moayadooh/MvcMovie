pipeline {
  agent any

  environment {
    REGISTRY = "muayadoh"
    WEB_APP_IMAGE_NAME = "web-app"
    API_APP_IMAGE_NAME = "api-app"

    DEPLOYMENT_FILE = "deployment.yml"
    SERVICE_FILE = "service.yml"
    NAMESPACE = "default"
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

          # Build API App image
          docker build -t $REGISTRY/$API_APP_IMAGE_NAME:latest ./crud-api

          # Build Web App image
          docker build -t $REGISTRY/$WEB_APP_IMAGE_NAME:latest .
        '''
      }
    }

    stage('Push to Registry') {
      steps {
        sh '''
          echo "Pushing Docker Images to Registry..."

          docker push $REGISTRY/$API_APP_IMAGE_NAME:latest
          docker push $REGISTRY/$WEB_APP_IMAGE_NAME:latest
        '''
      }
    }

    stage('Deploy to Kubernetes') {
      steps {
        sh '''
          echo "Deploying to Kubernetes..."

          # Update Kubernetes deployment file to use the latest images
          sed -i "s|image: .*/api-app:.*|image: $REGISTRY/$API_APP_IMAGE_NAME:latest|g" $DEPLOYMENT_FILE
          sed -i "s|image: .*/web-app:.*|image: $REGISTRY/$WEB_APP_IMAGE_NAME:latest|g" $DEPLOYMENT_FILE

          # Apply deployments and services
          kubectl apply -f $DEPLOYMENT_FILE --namespace=$NAMESPACE
          kubectl apply -f $SERVICE_FILE --namespace=$NAMESPACE

          # Verify rollout status
          kubectl rollout status deployment/api-app --namespace=$NAMESPACE
          kubectl rollout status deployment/web-app --namespace=$NAMESPACE
        '''
      }
    }
  }
}

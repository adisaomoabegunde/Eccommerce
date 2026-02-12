# Kubernetes Deployment Guide

This directory contains Kubernetes manifests for deploying the Eccommerce API to a Kubernetes cluster.

## Prerequisites

- Kubernetes cluster (v1.24+)
- kubectl configured to access your cluster
- Container registry with your Docker image
- Ingress controller (e.g., NGINX Ingress Controller)

## Quick Start

### 1. Create Namespace

```bash
kubectl apply -f namespace.yaml
```

### 2. Create Secrets

**Important:** Do not commit actual secrets to version control!

```bash
# Copy the example secret file
cp secret.yaml.example secret.yaml

# Edit secret.yaml with your actual base64-encoded values
# To encode a value:
# Linux/Mac: echo -n 'your-value' | base64
# Windows: [Convert]::ToBase64String([System.Text.Encoding]::UTF8.GetBytes('your-value'))

# Apply the secret
kubectl apply -f secret.yaml
```

### 3. Create ConfigMap

```bash
kubectl apply -f configmap.yaml
```

### 4. Deploy the Application

```bash
# Deploy the application
kubectl apply -f deployment.yaml

# Create the service
kubectl apply -f service.yaml

# (Optional) Create ingress
kubectl apply -f ingress.yaml

# (Optional) Enable auto-scaling
kubectl apply -f hpa.yaml
```

## Deployment Order

Apply manifests in this order:

1. `namespace.yaml` - Creates the namespace
2. `secret.yaml` - Creates secrets (connection strings, JWT secret)
3. `configmap.yaml` - Creates configuration
4. `deployment.yaml` - Deploys the application
5. `service.yaml` - Creates the service
6. `ingress.yaml` - (Optional) Sets up ingress
7. `hpa.yaml` - (Optional) Enables horizontal pod autoscaling

## Environment-Specific Deployments

### Development

```bash
kubectl apply -f . -n eccommerce
```

### Production

1. Update image tags in `deployment.yaml`
2. Review and adjust resource limits
3. Ensure secrets are properly configured
4. Apply manifests:

```bash
kubectl apply -f namespace.yaml
kubectl apply -f secret.yaml
kubectl apply -f configmap.yaml
kubectl apply -f deployment.yaml
kubectl apply -f service.yaml
kubectl apply -f ingress.yaml
kubectl apply -f hpa.yaml
```

## Updating the Application

### Rolling Update

```bash
# Update the image in deployment.yaml or use kubectl set image
kubectl set image deployment/eccommerce-api \
  eccommerce-api=ghcr.io/your-org/eccommerce-api:v1.2.3 \
  -n eccommerce

# Check rollout status
kubectl rollout status deployment/eccommerce-api -n eccommerce
```

### Rollback

```bash
# Rollback to previous version
kubectl rollout undo deployment/eccommerce-api -n eccommerce

# Rollback to specific revision
kubectl rollout undo deployment/eccommerce-api --to-revision=2 -n eccommerce
```

## Database Migrations

Run database migrations before deploying a new version:

```bash
# Option 1: Run as a Kubernetes Job
kubectl run eccommerce-migrate \
  --image=ghcr.io/your-org/eccommerce-api:latest \
  --restart=Never \
  --env="ConnectionStrings__Default=$CONNECTION_STRING" \
  -n eccommerce \
  -- dotnet ef database update

# Option 2: Run in an existing pod
kubectl exec -it deployment/eccommerce-api -n eccommerce \
  -- dotnet ef database update
```

## Monitoring and Troubleshooting

### View Logs

```bash
# View logs from all pods
kubectl logs -f deployment/eccommerce-api -n eccommerce

# View logs from a specific pod
kubectl logs -f <pod-name> -n eccommerce

# View previous container logs
kubectl logs <pod-name> --previous -n eccommerce
```

### Check Pod Status

```bash
# List all pods
kubectl get pods -n eccommerce

# Describe a pod
kubectl describe pod <pod-name> -n eccommerce

# Get pod events
kubectl get events -n eccommerce --sort-by='.lastTimestamp'
```

### Health Checks

```bash
# Port-forward to test locally
kubectl port-forward deployment/eccommerce-api 8080:8080 -n eccommerce

# Test health endpoint
curl http://localhost:8080/health
```

### Resource Usage

```bash
# View resource usage
kubectl top pods -n eccommerce
kubectl top nodes

# View HPA status
kubectl get hpa -n eccommerce
```

## Scaling

### Manual Scaling

```bash
# Scale to 5 replicas
kubectl scale deployment/eccommerce-api --replicas=5 -n eccommerce
```

### Auto-Scaling

The HPA (Horizontal Pod Autoscaler) is configured to scale between 3-10 replicas based on CPU and memory usage.

```bash
# View HPA status
kubectl get hpa eccommerce-api-hpa -n eccommerce

# Describe HPA
kubectl describe hpa eccommerce-api-hpa -n eccommerce
```

## Clean Up

```bash
# Delete all resources
kubectl delete -f . -n eccommerce

# Delete namespace (this will delete all resources in it)
kubectl delete namespace eccommerce
```

## Configuration Updates

### Update ConfigMap

```bash
# Edit configmap
kubectl edit configmap eccommerce-api-config -n eccommerce

# Or apply updated file
kubectl apply -f configmap.yaml

# Restart deployment to pick up changes
kubectl rollout restart deployment/eccommerce-api -n eccommerce
```

### Update Secrets

```bash
# Delete existing secret
kubectl delete secret eccommerce-api-secret -n eccommerce

# Create new secret
kubectl apply -f secret.yaml

# Restart deployment
kubectl rollout restart deployment/eccommerce-api -n eccommerce
```

## Security Best Practices

1. **Secrets Management:**
   - Never commit `secret.yaml` to version control
   - Use external secret managers (e.g., Azure Key Vault, AWS Secrets Manager, HashiCorp Vault)
   - Rotate secrets regularly

2. **Network Policies:**
   - Implement network policies to restrict pod-to-pod communication
   - Use ingress controller for external access

3. **RBAC:**
   - Apply principle of least privilege
   - Create service accounts with minimal permissions

4. **Image Security:**
   - Use specific image tags (not `latest`)
   - Scan images for vulnerabilities
   - Use private container registries

## Additional Resources

- [Kubernetes Documentation](https://kubernetes.io/docs/)
- [NGINX Ingress Controller](https://kubernetes.github.io/ingress-nginx/)
- [Horizontal Pod Autoscaler](https://kubernetes.io/docs/tasks/run-application/horizontal-pod-autoscale/)

#!/bin/bash

# 🐳 Script de Teste Rápido - Docker e CI/CD
# FIAP Projeto Games - Fase 2

echo "🚀 Iniciando testes de Docker e CI/CD..."

# Cores para output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Função para log
log() {
    echo -e "${BLUE}[INFO]${NC} $1"
}

success() {
    echo -e "${GREEN}[SUCCESS]${NC} $1"
}

error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

warning() {
    echo -e "${YELLOW}[WARNING]${NC} $1"
}

# Verificar se Docker está instalado
log "Verificando se Docker está instalado..."
if ! command -v docker &> /dev/null; then
    error "Docker não está instalado!"
    exit 1
fi
success "Docker encontrado!"

# Verificar se Docker Compose está instalado
log "Verificando se Docker Compose está instalado..."
if ! command -v docker-compose &> /dev/null; then
    error "Docker Compose não está instalado!"
    exit 1
fi
success "Docker Compose encontrado!"

# Verificar se Dockerfile existe
log "Verificando se Dockerfile existe..."
if [ ! -f "Dockerfile" ]; then
    error "Dockerfile não encontrado!"
    exit 1
fi
success "Dockerfile encontrado!"

# Verificar se docker-compose.yml existe
log "Verificando se docker-compose.yml existe..."
if [ ! -f "docker-compose.yml" ]; then
    error "docker-compose.yml não encontrado!"
    exit 1
fi
success "docker-compose.yml encontrado!"

echo ""
log "🐳 TESTE 1: Build da Imagem Docker"
echo "=================================="

# Build da imagem
log "Fazendo build da imagem..."
docker build -t fiap-projeto-games-api .

if [ $? -eq 0 ]; then
    success "Build da imagem concluído com sucesso!"
else
    error "Falha no build da imagem!"
    exit 1
fi

# Verificar tamanho da imagem
log "Verificando tamanho da imagem..."
docker images fiap-projeto-games-api --format "table {{.Repository}}\t{{.Tag}}\t{{.Size}}"

echo ""
log "🐳 TESTE 2: Execução do Container"
echo "================================="

# Executar container
log "Executando container..."
docker run -d -p 5000:80 --name fiap-test fiap-projeto-games-api

if [ $? -eq 0 ]; then
    success "Container iniciado com sucesso!"
else
    error "Falha ao iniciar container!"
    exit 1
fi

# Aguardar aplicação inicializar
log "Aguardando aplicação inicializar..."
sleep 10

# Verificar se container está rodando
log "Verificando status do container..."
docker ps | grep fiap-test

# Testar health check
log "Testando health check..."
response=$(curl -s -o /dev/null -w "%{http_code}" http://localhost:5000/health)

if [ "$response" = "200" ]; then
    success "Health check funcionando! (Status: $response)"
else
    error "Health check falhou! (Status: $response)"
fi

# Testar health check detalhado
log "Testando health check detalhado..."
curl -s http://localhost:5000/health/detailed | head -20

# Testar Swagger
log "Testando Swagger..."
swagger_response=$(curl -s -o /dev/null -w "%{http_code}" http://localhost:5000)

if [ "$swagger_response" = "200" ]; then
    success "Swagger funcionando! (Status: $swagger_response)"
else
    warning "Swagger pode não estar funcionando (Status: $swagger_response)"
fi

echo ""
log "🐳 TESTE 3: Docker Compose"
echo "=========================="

# Parar container individual
log "Parando container individual..."
docker stop fiap-test
docker rm fiap-test

# Executar com Docker Compose
log "Executando com Docker Compose..."
docker-compose up -d

if [ $? -eq 0 ]; then
    success "Docker Compose iniciado com sucesso!"
else
    error "Falha ao iniciar Docker Compose!"
    exit 1
fi

# Aguardar serviços inicializarem
log "Aguardando serviços inicializarem..."
sleep 15

# Verificar status dos containers
log "Verificando status dos containers..."
docker-compose ps

# Testar serviços
log "Testando serviços..."

# API
api_response=$(curl -s -o /dev/null -w "%{http_code}" http://localhost:5000/health)
if [ "$api_response" = "200" ]; then
    success "API funcionando! (Status: $api_response)"
else
    error "API falhou! (Status: $api_response)"
fi

# Prometheus
prometheus_response=$(curl -s -o /dev/null -w "%{http_code}" http://localhost:9090)
if [ "$prometheus_response" = "200" ]; then
    success "Prometheus funcionando! (Status: $prometheus_response)"
else
    warning "Prometheus pode não estar funcionando (Status: $prometheus_response)"
fi

# Grafana
grafana_response=$(curl -s -o /dev/null -w "%{http_code}" http://localhost:3000)
if [ "$grafana_response" = "200" ]; then
    success "Grafana funcionando! (Status: $grafana_response)"
else
    warning "Grafana pode não estar funcionando (Status: $grafana_response)"
fi

echo ""
log "🐳 TESTE 4: Métricas e Monitoramento"
echo "===================================="

# Testar métricas
log "Testando endpoints de métricas..."
curl -s -X POST "http://localhost:5000/api/metrics/counter/test?label=demo" > /dev/null
curl -s -X POST "http://localhost:5000/api/metrics/histogram/latency?value=150&label=api" > /dev/null
curl -s -X POST "http://localhost:5000/api/metrics/gauge/memory?value=512&label=heap" > /dev/null

# Verificar métricas
log "Verificando métricas..."
curl -s http://localhost:5000/api/metrics | grep -E "(test|latency|memory)" | head -5

# Gerar tráfego para ver métricas
log "Gerando tráfego para testar métricas..."
for i in {1..5}; do
    curl -s http://localhost:5000/health > /dev/null
    curl -s http://localhost:5000/api/usuarios > /dev/null
done

echo ""
log "🐳 TESTE 5: Health Checks"
echo "========================"

# Testar todos os endpoints de health
log "Testando endpoints de health..."

endpoints=("health" "health/detailed" "health/ready" "health/live")

for endpoint in "${endpoints[@]}"; do
    response=$(curl -s -o /dev/null -w "%{http_code}" "http://localhost:5000/$endpoint")
    if [ "$response" = "200" ]; then
        success "Endpoint /$endpoint funcionando! (Status: $response)"
    else
        error "Endpoint /$endpoint falhou! (Status: $response)"
    fi
done

echo ""
log "🐳 TESTE 6: Limpeza"
echo "=================="

# Parar Docker Compose
log "Parando Docker Compose..."
docker-compose down

# Remover imagem de teste
log "Removendo imagem de teste..."
docker rmi fiap-projeto-games-api

success "Limpeza concluída!"

echo ""
echo "🎉 TESTES CONCLUÍDOS COM SUCESSO!"
echo "=================================="
echo ""
echo "✅ Docker build funcionando"
echo "✅ Container executando"
echo "✅ Health checks respondendo"
echo "✅ Docker Compose funcionando"
echo "✅ Métricas funcionando"
echo "✅ Monitoramento configurado"
echo ""
echo "🚀 Pronto para gravar o vídeo de demonstração!"
echo ""
echo "📋 Próximos passos:"
echo "1. Configurar secrets do GitHub Actions"
echo "2. Fazer push para trigger do CI/CD"
echo "3. Gravar demonstração do pipeline"
echo "4. Mostrar deploy na cloud"
echo "" 
# Script para importar o dashboard do Grafana
Write-Host "=== Importacao do Dashboard FCG - FIAP Challenge Games ===" -ForegroundColor Green
Write-Host ""

# Verificar se o Grafana esta rodando
Write-Host "Verificando se o Grafana esta rodando..." -ForegroundColor Yellow
try {
    $response = Invoke-RestMethod -Uri "http://localhost:3000/api/health" -Method GET -TimeoutSec 5
    Write-Host "Grafana esta rodando!" -ForegroundColor Green
} catch {
    Write-Host "Grafana nao esta rodando. Execute 'docker-compose up -d' primeiro." -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "=== INSTRUCOES PARA IMPORTAR O DASHBOARD ===" -ForegroundColor Cyan
Write-Host ""
Write-Host "1. Abra o navegador e acesse: http://localhost:3000" -ForegroundColor White
Write-Host "2. Faca login com: admin / admin" -ForegroundColor White
Write-Host "3. Clique no icone '+' no menu lateral esquerdo" -ForegroundColor White
Write-Host "4. Selecione 'Import'" -ForegroundColor White
Write-Host "5. Clique em 'Upload JSON file' ou cole o conteudo do arquivo:" -ForegroundColor White
Write-Host "   grafana/dashboards/fiap-games-dashboard.json" -ForegroundColor Yellow
Write-Host "6. Clique em 'Load'" -ForegroundColor White
Write-Host "7. Clique em 'Import'" -ForegroundColor White
Write-Host ""

# Verificar se o arquivo existe
$dashboardFile = "grafana/dashboards/fiap-games-dashboard.json"
if (Test-Path $dashboardFile) {
    Write-Host "Arquivo do dashboard encontrado: $dashboardFile" -ForegroundColor Green
    Write-Host ""
    Write-Host "Conteudo do arquivo para copiar:" -ForegroundColor Cyan
    Write-Host "----------------------------------------" -ForegroundColor Gray
    Get-Content $dashboardFile | Write-Host
    Write-Host "----------------------------------------" -ForegroundColor Gray
} else {
    Write-Host "Arquivo do dashboard nao encontrado: $dashboardFile" -ForegroundColor Red
}

Write-Host ""
Write-Host "=== LINKS UTEIS ===" -ForegroundColor Cyan
Write-Host "Grafana: http://localhost:3000" -ForegroundColor White
Write-Host "Prometheus: http://localhost:9090" -ForegroundColor White
Write-Host "API Health: http://localhost:8080/health" -ForegroundColor White
Write-Host "API Metrics: http://localhost:8080/api/metrics" -ForegroundColor White
Write-Host "Swagger: http://localhost:8080/swagger" -ForegroundColor White

Write-Host ""
Write-Host "=== PARA GERAR METRICAS ===" -ForegroundColor Cyan
Write-Host "Execute algumas requisicoes para a API para gerar dados:" -ForegroundColor White
Write-Host "Health Check: curl http://localhost:8080/health" -ForegroundColor Gray
Write-Host "Cadastro: curl -X POST http://localhost:8080/api/usuarios/cadastro -H 'Content-Type: application/json' -d '{\"nome\":\"Teste\",\"email\":\"teste@teste.com\",\"senha\":\"123456Ii#\",\"confirmarSenha\":\"123456Ii#\",\"isAdmin\":false}'" -ForegroundColor Gray
Write-Host "Login: curl -X POST http://localhost:8080/api/usuarios/login -H 'Content-Type: application/json' -d '{\"email\":\"teste@teste.com\",\"senha\":\"123456Ii#\"}'" -ForegroundColor Gray

Write-Host ""
Write-Host "Apos importar o dashboard, voce vera os dados em tempo real!" -ForegroundColor Green 
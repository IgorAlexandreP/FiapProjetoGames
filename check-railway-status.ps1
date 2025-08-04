# Script para verificar status completo da aplicação no Railway

$RAILWAY_URL = "https://fiap-projeto-games.railway.app"

Write-Host "Verificando status completo da aplicação no Railway..." -ForegroundColor Green
Write-Host "URL: $RAILWAY_URL" -ForegroundColor Cyan
Write-Host ""

# Função para testar endpoint
function Test-Endpoint {
    param(
        [string]$Name,
        [string]$Url,
        [string]$Method = "GET"
    )
    
    Write-Host "$Name..." -ForegroundColor Yellow
    try {
        $response = Invoke-WebRequest -Uri $Url -Method $Method -TimeoutSec 10
        Write-Host "Status: $($response.StatusCode)" -ForegroundColor Green
        Write-Host "Content: $($response.Content)" -ForegroundColor Cyan
        Write-Host "Headers:" -ForegroundColor Yellow
        $response.Headers | Format-Table -AutoSize
        return $response
    }
    catch {
        Write-Host "Erro: $($_.Exception.Message)" -ForegroundColor Red
        return $null
    }
    Write-Host ""
}

# Testando diferentes endpoints
$endpoints = @(
    @{Name="1. Raiz /"; Url="$RAILWAY_URL/"},
    @{Name="2. Health /health"; Url="$RAILWAY_URL/health"},
    @{Name="3. Debug /debug"; Url="$RAILWAY_URL/debug"},
    @{Name="4. API Test /api/test"; Url="$RAILWAY_URL/api/test"},
    @{Name="5. Swagger /swagger"; Url="$RAILWAY_URL/swagger"},
    @{Name="6. API Docs /api-docs"; Url="$RAILWAY_URL/api-docs"},
    @{Name="7. Health Ping /health/ping"; Url="$RAILWAY_URL/health/ping"},
    @{Name="8. Health Test /health/test"; Url="$RAILWAY_URL/health/test"},
    @{Name="9. Root Ping /ping"; Url="$RAILWAY_URL/ping"},
    @{Name="10. Root Status /status"; Url="$RAILWAY_URL/status"}
)

foreach ($endpoint in $endpoints) {
    Test-Endpoint -Name $endpoint.Name -Url $endpoint.Url
}

Write-Host "Análise dos resultados:" -ForegroundColor Green
Write-Host "- Se apenas /health retorna 'OK', o Railway está interceptando as requisições" -ForegroundColor Yellow
Write-Host "- Se todos retornam 404, a aplicação não está iniciando" -ForegroundColor Yellow
Write-Host "- Se alguns funcionam, há problema de roteamento" -ForegroundColor Yellow
Write-Host ""
Write-Host "Próximos passos:" -ForegroundColor Green
Write-Host "1. Verificar logs do Railway" -ForegroundColor Cyan
Write-Host "2. Verificar configuração do Railway" -ForegroundColor Cyan
Write-Host "3. Verificar se o Dockerfile está correto" -ForegroundColor Cyan 
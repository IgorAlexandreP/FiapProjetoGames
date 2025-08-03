# Script PowerShell para testar a API no Railway

$RAILWAY_URL = "https://fiap-projeto-games.railway.app"

Write-Host "Testando API no Railway..." -ForegroundColor Green
Write-Host "URL: $RAILWAY_URL" -ForegroundColor Cyan
Write-Host ""

# Função para testar endpoint
function Test-Endpoint {
    param(
        [string]$Name,
        [string]$Url,
        [string]$Method = "GET",
        [string]$Body = $null,
        [hashtable]$Headers = @{}
    )
    
    Write-Host "$Name..." -ForegroundColor Yellow
    try {
        $params = @{
            Uri = $Url
            Method = $Method
        }
        
        if ($Body) {
            $params.Body = $Body
            $params.ContentType = "application/json"
        }
        
        if ($Headers.Count -gt 0) {
            $params.Headers = $Headers
        }
        
        $response = Invoke-WebRequest @params
        Write-Host "Status: $($response.StatusCode)" -ForegroundColor Green
        Write-Host "Content: $($response.Content)" -ForegroundColor Cyan
        return $response
    }
    catch {
        Write-Host "Erro: $($_.Exception.Message)" -ForegroundColor Red
        return $null
    }
    Write-Host ""
}

# 1. Health Check
Test-Endpoint -Name "1. Health Check" -Url "$RAILWAY_URL/health"

# 2. Root Endpoint
Test-Endpoint -Name "2. Root Endpoint" -Url "$RAILWAY_URL/"

# 3. API Test Endpoint
Test-Endpoint -Name "3. API Test Endpoint" -Url "$RAILWAY_URL/api/test"

# 4. API Jogos Endpoint
Test-Endpoint -Name "4. API Jogos Endpoint" -Url "$RAILWAY_URL/api/jogos"

# 5. Health Ping
Test-Endpoint -Name "5. Health Ping" -Url "$RAILWAY_URL/health/ping"

# 6. Health Test
Test-Endpoint -Name "6. Health Test" -Url "$RAILWAY_URL/health/test"

# 7. Cadastrar Usuário
$cadastroBody = @{
    nome = "João Silva"
    email = "joao@teste.com"
    senha = "Teste123!"
    confirmarSenha = "Teste123!"
} | ConvertTo-Json

Test-Endpoint -Name "7. Cadastrar Usuário" -Url "$RAILWAY_URL/api/usuarios/cadastro" -Method "POST" -Body $cadastroBody

# 8. Login
$loginBody = @{
    email = "joao@teste.com"
    senha = "Teste123!"
} | ConvertTo-Json

$loginResponse = Test-Endpoint -Name "8. Login" -Url "$RAILWAY_URL/api/usuarios/login" -Method "POST" -Body $loginBody

# 9. Testar Biblioteca com Token (se login funcionou)
if ($loginResponse -and $loginResponse.StatusCode -eq 200) {
    try {
        $loginData = $loginResponse.Content | ConvertFrom-Json
        if ($loginData.token) {
            $headers = @{
                "Authorization" = "Bearer $($loginData.token)"
            }
            
            Test-Endpoint -Name "9. Biblioteca (com token)" -Url "$RAILWAY_URL/api/biblioteca" -Headers $headers
        }
        else {
            Write-Host "Token não encontrado na resposta do login" -ForegroundColor Red
        }
    }
    catch {
        Write-Host "Erro ao processar resposta do login: $($_.Exception.Message)" -ForegroundColor Red
    }
}

# 10. Swagger Documentation
Test-Endpoint -Name "10. Swagger Documentation" -Url "$RAILWAY_URL/api-docs"

Write-Host "Testes concluidos!" -ForegroundColor Green 
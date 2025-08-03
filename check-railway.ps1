# Script para verificar configurações do Railway

Write-Host "Verificando configuracoes do Railway..." -ForegroundColor Green
Write-Host ""

# Verificar se a aplicação está respondendo
Write-Host "1. Testando conectividade básica..." -ForegroundColor Yellow
try {
    $response = Invoke-WebRequest -Uri "https://fiap-projeto-games.railway.app/health" -Method GET
    Write-Host "Status: $($response.StatusCode)" -ForegroundColor Green
    Write-Host "Content: $($response.Content)" -ForegroundColor Cyan
    Write-Host "Headers:" -ForegroundColor Yellow
    $response.Headers | Format-Table -AutoSize
}
catch {
    Write-Host "Erro: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host ""

# Verificar se há algum problema com o roteamento
Write-Host "2. Testando diferentes endpoints..." -ForegroundColor Yellow

$endpoints = @(
    "/health",
    "/",
    "/api/test",
    "/api/jogos",
    "/api/usuarios/cadastro",
    "/api-docs"
)

foreach ($endpoint in $endpoints) {
    try {
        $response = Invoke-WebRequest -Uri "https://fiap-projeto-games.railway.app$endpoint" -Method GET
        Write-Host "$endpoint - Status: $($response.StatusCode)" -ForegroundColor Green
    }
    catch {
        Write-Host "$endpoint - Erro: $($_.Exception.Message)" -ForegroundColor Red
    }
}

Write-Host ""
Write-Host "3. Verificando se há problemas de configuração..." -ForegroundColor Yellow
Write-Host "O problema pode ser:" -ForegroundColor Red
Write-Host "- Variáveis de ambiente não configuradas no Railway" -ForegroundColor Yellow
Write-Host "- Aplicação não iniciando corretamente" -ForegroundColor Yellow
Write-Host "- Problema com o roteamento do Railway" -ForegroundColor Yellow
Write-Host ""
Write-Host "Próximos passos:" -ForegroundColor Green
Write-Host "1. Verificar se as variáveis estão configuradas no Railway" -ForegroundColor Cyan
Write-Host "2. Verificar os logs do deployment no Railway" -ForegroundColor Cyan
Write-Host "3. Verificar se a aplicação está iniciando corretamente" -ForegroundColor Cyan 
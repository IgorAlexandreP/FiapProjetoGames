#!/bin/bash

echo "🚀 Iniciando testes da API FIAP Projeto Games..."

# URL base da API
BASE_URL="http://localhost:5000"

echo "📋 Testando endpoints básicos..."

# Teste de health check
echo "1. Testando health check..."
curl -s "$BASE_URL/health" | jq '.'

# Teste de root
echo "2. Testando endpoint raiz..."
curl -s "$BASE_URL/" | jq '.'

# Teste de Swagger
echo "3. Testando Swagger..."
curl -s "$BASE_URL/swagger" -I | head -1

# Teste de listagem de jogos
echo "4. Testando listagem de jogos..."
curl -s "$BASE_URL/api/jogos" | jq '.'

# Teste de cadastro de usuário
echo "5. Testando cadastro de usuário..."
curl -s -X POST "$BASE_URL/api/usuarios/cadastro" \
  -H "Content-Type: application/json" \
  -d '{
    "nome": "Teste Usuario",
    "email": "teste@fiap.com.br",
    "senha": "Teste@123"
  }' | jq '.'

# Teste de login
echo "6. Testando login..."
TOKEN=$(curl -s -X POST "$BASE_URL/api/usuarios/login" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "teste@fiap.com.br",
    "senha": "Teste@123"
  }' | jq -r '.token')

echo "Token obtido: $TOKEN"

# Teste de biblioteca com autenticação
echo "7. Testando biblioteca com autenticação..."
curl -s -H "Authorization: Bearer $TOKEN" "$BASE_URL/api/biblioteca" | jq '.'

echo "✅ Testes concluídos!" 
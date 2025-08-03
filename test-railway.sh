#!/bin/bash

# Script para testar a API no Railway
# Substitua YOUR_RAILWAY_URL pela URL real da sua aplicação

RAILWAY_URL="https://YOUR_RAILWAY_URL"

echo "🧪 Testando API no Railway..."
echo "URL: $RAILWAY_URL"
echo ""

# 1. Health Check
echo "1️⃣ Testando Health Check..."
curl -s "$RAILWAY_URL/health" | jq .
echo ""

# 2. Root Endpoint
echo "2️⃣ Testando Root Endpoint..."
curl -s "$RAILWAY_URL/" | jq .
echo ""

# 3. Listar Jogos
echo "3️⃣ Testando Listagem de Jogos..."
curl -s "$RAILWAY_URL/api/jogos" | jq .
echo ""

# 4. Cadastrar Usuário
echo "4️⃣ Testando Cadastro de Usuário..."
CADASTRO_RESPONSE=$(curl -s -X POST "$RAILWAY_URL/api/usuarios/cadastro" \
  -H "Content-Type: application/json" \
  -d '{
    "nome": "João Silva",
    "email": "joao@teste.com",
    "senha": "Teste123!",
    "confirmarSenha": "Teste123!"
  }')

echo "$CADASTRO_RESPONSE" | jq .
echo ""

# 5. Login
echo "5️⃣ Testando Login..."
LOGIN_RESPONSE=$(curl -s -X POST "$RAILWAY_URL/api/usuarios/login" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "joao@teste.com",
    "senha": "Teste123!"
  }')

echo "$LOGIN_RESPONSE" | jq .

# Extrair token se o login foi bem-sucedido
TOKEN=$(echo "$LOGIN_RESPONSE" | jq -r '.token // empty')
echo ""

if [ ! -z "$TOKEN" ] && [ "$TOKEN" != "null" ]; then
    echo "✅ Login bem-sucedido! Token obtido."
    
    # 6. Listar Biblioteca
    echo "6️⃣ Testando Listagem da Biblioteca..."
    curl -s -H "Authorization: Bearer $TOKEN" "$RAILWAY_URL/api/biblioteca" | jq .
    echo ""
    
    # 7. Comprar Jogo (se houver jogos disponíveis)
    echo "7️⃣ Testando Compra de Jogo..."
    curl -s -X POST "$RAILWAY_URL/api/biblioteca/comprar" \
      -H "Authorization: Bearer $TOKEN" \
      -H "Content-Type: application/json" \
      -d '{
        "jogoId": "ID_DO_PRIMEIRO_JOGO"
      }' | jq .
    echo ""
else
    echo "❌ Login falhou ou token não foi retornado."
fi

# 8. Health Check Detalhado
echo "8️⃣ Testando Health Check Detalhado..."
curl -s "$RAILWAY_URL/health/detailed" | jq .
echo ""

# 9. Teste de Conectividade
echo "9️⃣ Testando Conectividade..."
curl -s "$RAILWAY_URL/health/ping"
echo ""
echo ""

echo "🎉 Testes concluídos!" 
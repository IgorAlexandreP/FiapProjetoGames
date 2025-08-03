#!/bin/bash

echo "🚀 Preparando deploy para Railway..."

# Verifica se há mudanças para commitar
if [ -z "$(git status --porcelain)" ]; then
    echo "❌ Não há mudanças para commitar"
    exit 1
fi

# Adiciona todas as mudanças
echo "📦 Adicionando arquivos..."
git add .

# Faz o commit
echo "💾 Fazendo commit..."
git commit -m "feat: Configuração completa para Railway

- Adicionado Program.cs robusto com logging estruturado
- Configurado health checks e middlewares
- Adicionado suporte a variáveis de ambiente
- Configurado banco de dados para Railway
- Adicionado controllers de teste
- Configurado CORS para produção
- Adicionado inicialização automática do banco
- Configurado JWT com variáveis de ambiente
- Adicionado arquivos de configuração Railway"

# Push para a branch main
echo "🚀 Fazendo push para main..."
git push origin main

echo "✅ Deploy iniciado! O Railway irá detectar as mudanças e fazer o build automaticamente."
echo ""
echo "📋 Próximos passos:"
echo "1. Acesse o Railway e configure as variáveis de ambiente"
echo "2. Crie um banco SQL Server no Railway"
echo "3. Configure as variáveis DB_HOST, DB_NAME, DB_USER, DB_PASSWORD"
echo "4. Configure JWT_SECRET"
echo "5. Aguarde o deploy completar"
echo "6. Teste os endpoints: /health, /, /api/jogos" 
#!/bin/bash

echo "Iniciando configuração do banco de dados..."

# Aguarda o banco de dados estar disponível
echo "Aguardando banco de dados..."
sleep 10

# Executa as migrações
echo "Executando migrações..."
dotnet ef database update --project src/FiapProjetoGames.Infrastructure --startup-project src/FiapProjetoGames.API

# Verifica se as migrações foram aplicadas com sucesso
if [ $? -eq 0 ]; then
    echo "Migrações aplicadas com sucesso!"
else
    echo "Erro ao aplicar migrações. Tentando criar banco..."
    dotnet ef database ensure-created --project src/FiapProjetoGames.Infrastructure --startup-project src/FiapProjetoGames.API
fi

echo "Configuração do banco de dados concluída!" 
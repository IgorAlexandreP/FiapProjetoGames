<<<<<<< HEAD
# FIAP Challenge Games (FCG)

## 💡 Sobre o Projeto

Bem-vindo ao FCG - uma iniciativa inovadora da FIAP para revolucionar a educação em tecnologia através dos games! Este é o MVP da primeira fase do projeto, onde desenvolvemos uma API REST em .NET 8 que servirá como base para uma plataforma educacional de jogos.

Nossa missão? Criar uma base sólida que não só atenda às necessidades atuais dos alunos FIAP, Alura e PM3, mas que também esteja preparada para evoluir com recursos incríveis como matchmaking e gerenciamento de servidores nas próximas fases.

## 🚀 Principais Funcionalidades

### 👤 Para os Usuários
- **Cadastro Simplificado e Seguro**
  - Nome, e-mail e senha
  - Validação rigorosa de e-mail
  - Senha forte obrigatória (mínimo 8 caracteres, com números, letras e caracteres especiais)
  - Sua biblioteca pessoal de jogos

### 👑 Para Administradores
- Gestão completa de jogos
- Administração de usuários
- Controle total da plataforma

### 🎮 Gestão de Jogos
- Cadastro e manutenção do catálogo
- Biblioteca pessoal para cada usuário
- Sistema de propriedade de jogos

## 🛠️ Tecnologias e Práticas

### Base Tecnológica
- **.NET 8**: Última versão do framework, garantindo performance e recursos modernos
- **Entity Framework Core**: ORM robusto para persistência de dados
- **SQL Server**: Banco de dados confiável e escalável
- **JWT**: Autenticação segura e stateless
- **Swagger**: Documentação clara e interativa da API

### Arquitetura e Qualidade
- **Arquitetura Monolítica**: Escolhida estrategicamente para o MVP, facilitando o desenvolvimento ágil
- **Domain-Driven Design (DDD)**: Organização do código em camadas bem definidas
  - Domain: Coração do negócio
  - Application: Orquestração dos casos de uso
  - Infrastructure: Persistência e serviços externos
  - API: Interface com o mundo exterior
- **Testes Unitários**: Garantia de qualidade do código
- **Middleware de Tratamento de Erros**: Respostas padronizadas e logs estruturados

## 🚦 Como Começar

### Pré-requisitos
- .NET 8 SDK
- SQL Server (LocalDB ou instância completa)
- Sua IDE favorita (recomendamos Visual Studio 2022 ou VS Code)

### Configuração em 4 Passos

1. **Clone o Repositório**
```bash
git clone [url-do-repositorio]
cd [nome-do-diretorio]
```

2. **Restaure os Pacotes**
```bash
dotnet restore
```

3. **Configure o Banco de Dados**
Ajuste o `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=FiapProjetoGames;Trusted_Connection=True"
  }
}
```

4. **Prepare o Banco de Dados**
```bash
cd src/FiapProjetoGames.API
dotnet ef database update
```

### 🎯 Endpoints Principais

#### Autenticação
```
POST /api/usuarios/cadastro - Crie sua conta
POST /api/usuarios/login - Acesse a plataforma
```

#### Gestão de Jogos
```
GET /api/jogos - Explore o catálogo
POST /api/jogos - Adicione novos jogos (Admin)
GET /api/biblioteca - Sua coleção pessoal
```

## 🔒 Segurança

Levamos a segurança a sério! Implementamos:
- Autenticação via JWT
- Hash seguro de senhas com BCrypt
- Autorização baseada em roles
- Validação de propriedade de jogos

## 🤝 Contribuindo

Quer fazer parte dessa revolução na educação? Aqui está como:

1. Faça um fork
2. Crie sua branch (`git checkout -b feature/SuaFeature`)
3. Commit suas mudanças (`git commit -m 'Adiciona feature incrível'`)
4. Push para a branch (`git push origin feature/SuaFeature`)
5. Abra um Pull Request

## 📝 Licença

Este projeto está sob a licença MIT.

## 📬 Contato

Dúvidas? Sugestões? Entre em contato!
[Seu Nome] - [seu-email@exemplo.com]

---
Desenvolvido com 💙 pela equipe FCG 
=======
# FiapProjetoGames
>>>>>>> 147cfe0f6852a2a78552d4ca644a0175ade0f4c3

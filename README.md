# FIAP Challenge Games (FCG)

## Sobre o Projeto

Bem-vindo ao FCG - uma iniciativa inovadora da FIAP para revolucionar a educação em tecnologia através dos games! Este é o MVP da primeira fase do projeto, onde desenvolvemos uma API REST em .NET 8 que servirá como base para uma plataforma educacional de jogos.

Nossa missão? Criar uma base sólida que não só atenda às necessidades atuais dos alunos FIAP, Alura e PM3, mas que também esteja preparada para evoluir com recursos incríveis como matchmaking e gerenciamento de servidores nas próximas fases.

## Principais Funcionalidades

### Para os Usuários
- **Cadastro Simplificado e Seguro**
  - Nome, e-mail e senha
  - Validação rigorosa de e-mail
  - Senha forte obrigatória (mínimo 8 caracteres, com números, letras e caracteres especiais)
  - Sua biblioteca pessoal de jogos

### Para Administradores
- Gestão completa de jogos
- Administração de usuários
- Controle total da plataforma

### Gestão de Jogos
- Cadastro e manutenção do catálogo
- Biblioteca pessoal para cada usuário
- Sistema de propriedade de jogos

## Tecnologias e Práticas

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

## Como Começar

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

### Deploy no Railway

Para fazer o deploy no Railway, siga estes passos:

1. **Conecte seu repositório ao Railway**
   - Acesse [railway.app](https://railway.app)
   - Conecte seu repositório GitHub
   - Selecione este projeto

2. **Configure as Variáveis de Ambiente**
   No Railway, adicione as seguintes variáveis de ambiente:
   ```
   ASPNETCORE_ENVIRONMENT=Production
   ASPNETCORE_URLS=http://+:8080
   PORT=8080
   DB_HOST=sua-host-do-banco
   DB_NAME=sua-database-name
   DB_USER=seu-usuario
   DB_PASSWORD=sua-senha
   JWT_SECRET=sua-chave-secreta-jwt-muito-longa-e-segura-2024
   ```

3. **Deploy Automático**
   - O Railway detectará automaticamente o Dockerfile
   - O deploy será feito automaticamente a cada push para a branch main

4. **Verificação do Deploy**
   - Acesse o endpoint `/health` para verificar se a API está funcionando
   - Acesse `/swagger` para ver a documentação da API

### Endpoints Principais

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

#### Health Checks
```
GET /health - Status geral da aplicação
GET /health/ready - Verificação de prontidão
GET /health/live - Verificação de vida
```

## Segurança

Levamos a segurança a sério! Implementamos:
- Autenticação via JWT
- Hash seguro de senhas com BCrypt
- Autorização baseada em roles
- Validação de propriedade de jogos

## Contribuindo

Quer fazer parte dessa revolução na educação? Aqui está como:

1. Faça um fork
2. Crie sua branch (`git checkout -b feature/SuaFeature`)
3. Commit suas mudanças (`git commit -m 'Adiciona feature incrível'`)
4. Push para a branch (`git push origin feature/SuaFeature`)
5. Abra um Pull Request

## Contato

E-mail: irgopk13@gmail.com

---

Desenvolvido com ❤️ por Igor Alexandre 
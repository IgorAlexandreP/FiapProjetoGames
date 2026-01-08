# FIAP Challenge Games (FCG)

Bem-vindo ao **FCG**! Esta é uma iniciativa inovadora da FIAP para revolucionar a educação em tecnologia através dos games. O projeto consiste em uma plataforma completa para gerenciamento e consumo de jogos, desenvolvida com uma arquitetura moderna, distribuída e resiliente.

Nossa missão é criar uma base sólida que não só atenda às necessidades educacionais atuais, mas que sirva como referência técnica para implementações de Microsserviços, DevOps e Observabilidade.

---

## 🚀 Funcionalidades e Arquitetura

O sistema evoluiu de um MVP monolítico para uma **Arquitetura de Microsserviços** orientada a eventos, garantindo alta escalabilidade e desacoplamento.

### Para os Usuários
- **Experiência Fluida**: Navegação rápida e responsiva.
- **Segurança**: Cadastro com validação rigorosa e autenticação via JWT.
- **Pagamentos Ágeis**: Sistema de checkout assíncrono que não trava a navegação.
- **Biblioteca Pessoal**: Gerencie sua coleção de jogos favoritos.

### Destaques Técnicos (O que roda por trás dos panos)
- **Microsserviços**: Separação clara de responsabilidades.
  - **API Principal**: Gerencia usuários, catálogo e recebe requisições.
  - **Worker Service**: Processa tarefas pesadas (como pagamentos) em segundo plano.
- **Comunicação Assíncrona**: Uso de **RabbitMQ** e **MassTransit** para garantir que o sistema continue funcionando mesmo sob alta carga.
- **Resiliência**: Se um serviço cair, as mensagens ficam salvas na fila para processamento posterior.
- **Performance**: Imagens Docker otimizadas com **Alpine Linux** (leves e seguras).

---

## 🛠️ Tecnologias Utilizadas

- **.NET 9**: Framework de alta performance.
- **RabbitMQ**: Message Broker para comunicação entre serviços.
- **SQL Server**: Banco de dados relacional robusto.
- **Kubernetes (K8s)**: Orquestração de containers com auto-scaling (HPA).
- **Docker**: Containerização completa da aplicação.
- **OpenTelemetry & Prometheus**: Monitoramento de métricas em tempo real.
- **Swagger**: Documentação interativa da API.

---

## 📦 Como Rodar o Projeto

A maneira mais simples de ver tudo funcionando é utilizando o Docker. Preparamos um ambiente que sobe todos os serviços (Banco, RabbitMQ, API e Worker) automaticamente.

### Pré-requisitos
- Docker e Docker Compose instalados.

### Passo a Passo

1. **Clone o repositório**
   ```bash
   git clone https://github.com/IgorAlexandreP/FiapProjetoGames.git
   cd FiapProjetoGames
   ```

2. **Suba o ambiente**
   ```bash
   docker-compose up -d --build
   ```
   *Isso irá compilar o código, criar as imagens e iniciar os containers.*

3. **Acesse a Aplicação**
   - **Documentação (Swagger)**: [http://localhost:8080/swagger](http://localhost:8080/swagger)
   - **Métricas**: [http://localhost:8080/metrics](http://localhost:8080/metrics)
   - **RabbitMQ**: [http://localhost:15672](http://localhost:15672) (Login: guest / guest)

---

## ☁️ Infraestrutura e Kubernetes

O projeto está pronto para rodar em nuvem. Na pasta `k8s/`, você encontra todos os manifestos necessários para deploy em um cluster Kubernetes:

- **Escalabilidade Automática (HPA)**: O sistema monitora o uso de CPU e sobe novas réplicas da API automaticamente quando a demanda aumenta.
- **Segurança**: Credenciais sensíveis gerenciadas via *Secrets*.
- **Configuração**: Variáveis de ambiente injetadas via *ConfigMaps*.
- **Health Checks**: Sondas de `Liveness` e `Readiness` para garantir que apenas containers saudáveis recebam tráfego.

---

## 📂 Estrutura do Código

```
ProjetoFiap/
├── ProjetoFiap.API/          # API REST (Porta de entrada)
├── ProjetoFiap.Worker/       # Serviço de Background (Consumidor de filas)
├── ProjetoFiap.Domain/       # Regras de Negócio e Entidades
├── ProjetoFiap.Infrastructure/# Acesso a Dados e Repositórios
├── ProjetoFiap.Tests/        # Testes Unitários
├── k8s/                      # Arquivos de configuração Kubernetes
└── docker-compose.yml        # Orquestração para desenvolvimento local
```

---

## 📞 Contato

Desenvolvido com ❤️ por **Igor Alexandre**.
E-mail: irgopk13@gmail.com

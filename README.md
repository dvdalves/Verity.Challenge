# Verity Challenge - Gerenciamento de Transações e Resumo Diário

## 📌 Visão Geral
Este projeto é um sistema baseado em microsserviços para gerenciar transações financeiras e gerar um resumo diário consolidado. O sistema foi projetado para garantir escalabilidade e resiliência, utilizando Clean Architecure, CQRS, MediatR, MassTransit (RabbitMQ), Entity Framework Core e NUnit para testes.

---

## 🚀 Tecnologias Utilizadas
- **C# (.NET 8)**
- **ASP.NET Core Web API**
- **Entity Framework Core (PostgreSQL)**
- **MediatR (Padrão CQRS)**
- **MassTransit (RabbitMQ)**
- **AutoMapper**
- **NUnit & Moq (Testes Unitários)**
- **Docker & Docker Compose**

---

## 📂 Estrutura do Projeto

```
Verity.Challenge
│── Verity.Challenge.Transactions/         # Microsserviço de Transações Financeiras
│   ├── API/                               # Camada de Controllers e Middlewares e configurações principais
│   ├── Application/                       # Camada de Aplicação (CQRS, Handlers, DTOs)
│   ├── Domain/                            # Camada de Domínio (Entidades e Regras de Negócio)
│   ├── Infrastructure/                    # Infraestrutura (Banco, Mensageria, Configurações)
│
│── Verity.Challenge.DailySummary/         # Microsserviço de Resumo Diário
│   ├── API/
│   ├── Application/
│   ├── Domain/
│   ├── Infrastructure/
│
│── DailySummary.Tests/                    # Testes unitários gerais
│── Transactions.Tests/                    # Testes unitários gerais
│
│── docker-compose.yml                     # Configuração do Docker para dependências
```

---

## ⚙️ **Configuração e Execução**

### **1️⃣ Pré-requisitos**
Antes de rodar o projeto, certifique-se de ter instalado:
- [.NET SDK 8.0](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- [Docker](https://www.docker.com/get-started)

### **2️⃣ Configuração do Banco de Dados**
Por padrão, o projeto utiliza **PostgreSQL**. Se estiver rodando localmente sem Docker, configure a conexão no arquivo **`appsettings.Development.json`**:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=verity_transactions;Username=admin;Password=admin"
  },
  "RabbitMQ": {
    "Host": "localhost",
    "Username": "guest",
    "Password": "guest"
  }
}
```

Caso esteja utilizando **Docker**, as dependências podem ser iniciadas com:

```sh
docker-compose up -d
```

### **3️⃣ Executando a Aplicação**
Para rodar os microsserviços, pode serguir esse exemplo usando o Visual Studio.
Botão direito na solution > Propriedades:

![image](https://github.com/user-attachments/assets/b2fe36a7-0f6b-4f21-ad99-08355b3d9846)

Rode as APIs em múltiplos projetos:
![image](https://github.com/user-attachments/assets/bd004da3-0762-481d-bece-6350ef77b250)

Inicie a aplicação:
![image](https://github.com/user-attachments/assets/6ea579eb-ec85-492c-9c2c-689a715ec1de)

Swagger Transaction:
![image](https://github.com/user-attachments/assets/964933fa-9f88-41d2-8923-da928ab11922)

Swagger Daily Summary
![image](https://github.com/user-attachments/assets/716006d1-5464-4708-b237-a3a54ea2ad47)
---

## 🔄 **Arquitetura e Fluxo**
O sistema utiliza o **padrão CQRS** para separar **operações de leitura e escrita**, garantindo maior **desempenho e escalabilidade**.

1. O **microsserviço de Transações** recebe uma requisição para criar/editar/deletar uma transação.
2. Após a persistência no banco, um **evento é publicado no RabbitMQ**.
3. O **microsserviço de Resumo Diário** consome essa mensagem para atualizar o saldo diário.

---

## ✅ **Testes**
O projeto contém **testes unitários** utilizando **NUnit e Moq** utilizando banco em memória.

Para rodar os testes, execute:

```sh
dotnet test Verity.Challenge.Transactions.Tests/Verity.Challenge.Transactions.Tests.csproj
dotnet test Verity.Challenge.DailySummary.Tests/Verity.Challenge.DailySummary.Tests.csproj
```

---

## 📌 **Futuras Melhorias**
🔹 Implementação de **autenticação e autorização** usando **JWT + Keycloak**  
🔹 Otimização do **cache** com **Redis** para melhorar a performance  
🔹 Monitoramento e logging
🔹 Frontend para consumir as APIs

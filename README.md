# Controle de Estoque — SEMSRJ/Almoxarifado

Sistema desktop para controle de estoque do **Ministério da Saúde / SEMSRJ / Almoxarifado**, desenvolvido em **C# (.NET 8)**, **WinForms** e **PostgreSQL**.

## Funcionalidades

- Login de administrador
- Cadastro e gestão de setores (adicionar, editar, excluir)
- Cadastro de materiais com quantidade, preço, localização e validade opcional
- Saída de material com registro de retirante, CPF/matrícula e setor
- Histórico de saídas com edição e exclusão (ajuste automático do estoque)
- Consulta de estoque com filtro e últimas saídas por material
- Relatórios mensal, anual e de inventário com visualização, impressão e exportação PDF

## Credenciais padrão

| Campo | Valor |
|-------|-------|
| Login | `almoxarifado12` |
| Senha | `estoque123` |

## Requisitos (Windows)

- Windows 10 ou superior
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (recomendado) **ou** [PostgreSQL 15+](https://www.postgresql.org/download/windows/) instalado localmente

## Instalação com Docker (recomendado)

A forma mais simples de subir o banco de dados é via **Docker Compose**. A aplicação WinForms continua rodando no Windows e se conecta ao PostgreSQL exposto em `localhost:5432`.

### 1. Instalar Docker Desktop

Instale o [Docker Desktop para Windows](https://www.docker.com/products/docker-desktop/) e certifique-se de que está em execução.

### 2. Configurar variáveis de ambiente

Na raiz do projeto, copie o arquivo de exemplo:

```powershell
copy .env.example .env
```

Edite `.env` se quiser alterar usuário, senha ou porta do banco.

### 3. Subir o ambiente

**PowerShell:**

```powershell
.\scripts\docker-up.ps1
```

**Ou manualmente:**

```powershell
docker compose up -d --build
```

Isso irá:
- Subir o **PostgreSQL 16** em um container
- Aplicar automaticamente as **migrations** do Entity Framework
- Persistir os dados no volume `postgres_data`

### 4. Verificar se está rodando

```powershell
docker compose ps
```

### 5. Executar a aplicação WinForms

```powershell
dotnet run --project ControleEstoque
```

O `appsettings.json` já está configurado para `localhost:5432` com as credenciais padrão do `.env.example`. O usuário administrador (`almoxarifado12` / `estoque123`) é criado automaticamente na primeira execução.

### Comandos úteis do Docker

```powershell
# Parar os containers
docker compose down

# Parar e remover os dados do banco
docker compose down -v

# Ver logs do PostgreSQL
docker compose logs -f postgres

# Ver logs das migrations
docker compose logs migrate
```

> **Nota:** A interface gráfica WinForms não roda dentro do Docker (requer Windows desktop). O Docker sobe apenas o **PostgreSQL** e aplica as migrations. A aplicação é executada diretamente no Windows.

## Instalação manual (sem Docker)

### 1. Instalar PostgreSQL

Instale o PostgreSQL e anote usuário e senha do banco (padrão: `postgres`).

### 2. Criar o banco de dados

Abra o **pgAdmin** ou o **psql** e execute:

```sql
CREATE DATABASE controle_estoque;
```

### 3. Configurar a connection string

Edite o arquivo `ControleEstoque/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "Default": "Host=localhost;Port=5432;Database=controle_estoque;Username=postgres;Password=SUA_SENHA"
  }
}
```

### 4. Restaurar pacotes e aplicar migrations

No PowerShell ou Prompt de Comando, na pasta do projeto:

```powershell
cd ControleEstoque
dotnet restore
dotnet ef database update
```

> As migrations já estão incluídas no projeto (incluindo a coluna de preço histórico nas saídas). O usuário administrador é criado automaticamente na primeira execução.

### 5. Executar o sistema

```powershell
dotnet run --project ControleEstoque
```

Ou abra `ControleEstoque.sln` no **Visual Studio 2022** e pressione **F5**.

## Gerar executável para Windows (.exe)

Para instalar ou executar em outro computador **sem precisar do Visual Studio**, gere um executável com o script de publicação.

### Pré-requisito

No computador onde você vai **gerar** o `.exe`, instale o [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) (Windows 64 bits).

### Opção 1 — Script automático (recomendado)

Na raiz do projeto, abra o **PowerShell** e execute:

```powershell
.\scripts\publish-windows.ps1
```

Isso gera a pasta `publish\win-x64\` com:

| Arquivo | Descrição |
|---------|-----------|
| `ControleEstoque.exe` | Programa principal |
| `appsettings.json` | Configuração do banco (editável) |
| `Iniciar Controle de Estoque.bat` | Atalho para abrir o sistema |
| `LEIA-ME.txt` | Instruções de instalação |

**Modos disponíveis:**

```powershell
# Pasta com arquivos (recomendado — mais estável)
.\scripts\publish-windows.ps1 -Mode folder

# Um único arquivo .exe (mais compacto para distribuir)
.\scripts\publish-windows.ps1 -Mode singlefile

# Menor, mas exige .NET 8 Runtime instalado no PC de destino
.\scripts\publish-windows.ps1 -Mode framework
```

### Opção 2 — Visual Studio

1. Abra `ControleEstoque.sln` no Visual Studio 2022
2. Clique com o botão direito no projeto **ControleEstoque** → **Publicar**
3. Escolha **Pasta** como destino
4. Em **Configurações**, selecione:
   - **Modo de implantação:** Autossuficiente
   - **Destino de runtime:** `win-x64`
5. Clique em **Publicar**

### Opção 3 — Linha de comando manual

```powershell
dotnet publish ControleEstoque\ControleEstoque.csproj `
  -c Release `
  -r win-x64 `
  --self-contained true `
  -o publish\win-x64
```

### Como instalar em outro computador

1. **Copie** a pasta `publish\win-x64` inteira para o PC de destino (ex.: `C:\Programas\ControleEstoque`)
2. **Configure o banco de dados** no PC de destino:
   - Com Docker: copie também `docker-compose.yml` e `.env`, execute `docker compose up -d`
   - Sem Docker: instale PostgreSQL e crie o banco `controle_estoque`
3. **Edite** `appsettings.json` na pasta copiada com a senha correta do PostgreSQL
4. **Execute** `ControleEstoque.exe` ou o atalho `.bat`
5. Faça login com `almoxarifado12` / `estoque123`

> Na primeira execução, o sistema cria as tabelas e o usuário administrador automaticamente (se o banco estiver acessível).

### Diferença entre os modos

| Modo | Tamanho | Precisa instalar .NET no destino? |
|------|---------|-----------------------------------|
| `folder` (autossuficiente) | ~80–120 MB | Não |
| `singlefile` (autossuficiente) | ~80–120 MB em 1 arquivo | Não |
| `framework` | ~5–10 MB | Sim — [.NET 8 Desktop Runtime](https://dotnet.microsoft.com/download/dotnet/8.0) |

### Criar instalador (.msi / setup.exe)

Para um instalador profissional com atalho no menu Iniciar, use ferramentas como:

- **[Inno Setup](https://jrsoftware.org/isinfo.php)** (gratuito) — gera `setup.exe`
- **WiX Toolset** — gera `.msi`
- **Visual Studio Installer Projects** — extensão da Microsoft

O modo mais simples para uso interno é **copiar a pasta `publish\win-x64`** para o computador de destino.

## Estrutura do projeto

```
controle-estoque/
├── ControleEstoque.sln
├── docker-compose.yml          # PostgreSQL + migrations
├── .env.example                # Variáveis de ambiente do Docker
├── docker/
│   ├── Dockerfile.migrate      # Container que aplica migrations
│   └── migrate-entrypoint.sh
├── ControleEstoque.Migrator/   # Console app net8.0 para migrations no Docker
├── scripts/
│   ├── docker-up.ps1           # Script de subida (Windows)
│   ├── docker-up.sh            # Script de subida (Linux/macOS)
│   └── publish-windows.ps1     # Gera executável .exe para Windows
└── ControleEstoque/
    ├── Program.cs              # Ponto de entrada
    ├── appsettings.json        # Connection string
    ├── Forms/                  # Telas WinForms
    ├── Models/                 # Entidades
    ├── Data/                   # DbContext e migrations
    ├── Services/               # Regras de negócio
    ├── Reports/                # PDF e impressão
    └── UI/                     # Tema visual
```

## Telas do sistema

| Tela | Descrição |
|------|-----------|
| Login | Autenticação do administrador |
| Menu Principal | Navegação entre módulos |
| Adicionar Setor | CRUD de setores |
| Adicionar Material | Entrada de materiais no estoque |
| Saída de Material | Registro de retiradas |
| Histórico de Saídas | Consulta, edição e exclusão de saídas |
| Estoque | Consulta de materiais e últimas saídas |
| Gerar Relatórios | Mensal, anual e inventário (PDF/impressão) |

## Relatórios

- **Mensal:** materiais retirados por setor e mês, com preço total gasto (considerando o preço no momento da retirada)
- **Anual:** materiais retirados por setor e ano (2026–2100), também com preço histórico da saída
- **Inventário:** todos os materiais em estoque com localização e valores

Todos os relatórios podem ser **visualizados**, **impressos** ou **salvos em PDF**.

## Tecnologias

- .NET 8 WinForms
- Entity Framework Core 8
- PostgreSQL (Npgsql)
- BCrypt.Net (hash de senha)
- QuestPDF (geração de PDF)

## Solução de problemas

**Erro de conexão com o banco:** verifique se o Docker está rodando (`docker compose ps`) ou se o PostgreSQL local está em execução. Confira a connection string em `appsettings.json`.

**Porta 5432 em uso:** altere `POSTGRES_PORT` no arquivo `.env` (ex.: `5433`) e atualize o `appsettings.json` com a mesma porta.

**Migrations não aplicadas:** execute `docker compose up migrate --build` ou `dotnet ef database update` na pasta `ControleEstoque`.

**Erro `failed to solve: ... dotnet restore ControleEstoque.Migrator/ControleEstoque.Migrator.csproj`:** normalmente ocorre quando o restore roda no Linux e tenta resolver também o alvo Windows do projeto principal. Faça rebuild sem cache da imagem de migration para garantir o `Dockerfile.migrate` atualizado:

```powershell
docker compose down
docker compose build --no-cache migrate
docker compose up -d
```

**Erro `exec /migrate-entrypoint.sh: no such file or directory`:** faça rebuild sem cache do serviço de migration:

```powershell
docker compose down
docker compose build --no-cache migrate
docker compose up -d
```

**Erro `ControleEstoque.deps.json does not exist` ou `Microsoft.WindowsDesktop.App was not found`:** imagem antiga de migrate. A versão atual usa o `ControleEstoque.Migrator` (console `net8.0`), compatível com Linux. Atualize para a versão atual do projeto e rode rebuild sem cache:

```powershell
docker compose down
docker compose build --no-cache migrate
docker compose up -d
```

**Conflito de concorrência no estoque:** em cenários com múltiplos usuários retirando/editando ao mesmo tempo, o sistema pode retornar "Conflito de concorrência no estoque. Tente novamente.". Basta repetir a operação; o sistema usa transações serializáveis com retentativa automática para proteger o saldo.

**Login inválido:** use `almoxarifado12` / `estoque123`. O usuário é criado automaticamente na primeira execução com banco vazio.

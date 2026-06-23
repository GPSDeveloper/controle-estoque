param(
    [ValidateSet("folder", "singlefile", "framework")]
    [string]$Mode = "folder"
)

$ErrorActionPreference = "Stop"
$Root = Split-Path -Parent (Split-Path -Parent $MyInvocation.MyCommand.Path)
Set-Location $Root

$Project = "ControleEstoque\ControleEstoque.csproj"

Write-Host "=== Publicando Controle de Estoque para Windows ===" -ForegroundColor Cyan
Write-Host "Modo: $Mode" -ForegroundColor Yellow
Write-Host ""

switch ($Mode) {
    "folder" {
        $Output = "publish\win-x64"
        dotnet publish $Project `
            -c Release `
            -r win-x64 `
            --self-contained true `
            -p:PublishReadyToRun=true `
            -o $Output
    }
    "singlefile" {
        $Output = "publish\win-x64-singlefile"
        dotnet publish $Project `
            -c Release `
            -r win-x64 `
            --self-contained true `
            -p:PublishSingleFile=true `
            -p:IncludeNativeLibrariesForSelfExtract=true `
            -p:EnableCompressionInSingleFile=true `
            -p:PublishReadyToRun=true `
            -o $Output
    }
    "framework" {
        $Output = "publish\win-x64-framework"
        dotnet publish $Project `
            -c Release `
            -r win-x64 `
            --self-contained false `
            -o $Output
    }
}

if ($LASTEXITCODE -ne 0) {
    Write-Host "Erro ao publicar o projeto." -ForegroundColor Red
    exit 1
}

# Garante appsettings.json na pasta de publicacao
Copy-Item "ControleEstoque\appsettings.json" "$Output\appsettings.json" -Force

# Cria arquivo de exemplo para o usuario configurar em outro PC
@"
{
  ""ConnectionStrings"": {
    ""Default"": ""Host=localhost;Port=5432;Database=controle_estoque;Username=postgres;Password=SUA_SENHA""
  }
}
"@ | Set-Content "$Output\appsettings.exemplo.json" -Encoding UTF8

# Atalho em lote para abrir o sistema
@"
@echo off
cd /d ""%~dp0""
start """" ControleEstoque.exe
"@ | Set-Content "$Output\Iniciar Controle de Estoque.bat" -Encoding ASCII

# Instrucoes para distribuicao
@"
CONTROLE DE ESTOQUE - SEMSRJ/Almoxarifado
=========================================

COMO INSTALAR EM OUTRO COMPUTADOR
---------------------------------

1. Copie esta pasta inteira para o computador de destino
   (ex.: C:\Programas\ControleEstoque)

2. Instale o PostgreSQL OU use Docker para subir o banco
   (veja README.md do projeto)

3. Edite o arquivo appsettings.json e configure a senha do banco:
   Host=localhost;Port=5432;Database=controle_estoque;Username=postgres;Password=SUA_SENHA

4. Execute ControleEstoque.exe ou o atalho .bat

LOGIN PADRAO
------------
Usuario: almoxarifado12
Senha:   estoque123

REQUISITOS
----------
- Windows 10 ou superior (64 bits)
- PostgreSQL acessivel (local ou via Docker)
- Nao e necessario instalar o .NET (versao self-contained)
"@ | Set-Content "$Output\LEIA-ME.txt" -Encoding UTF8

Write-Host ""
Write-Host "Publicacao concluida!" -ForegroundColor Green
Write-Host "Pasta gerada: $Output" -ForegroundColor Green
Write-Host ""
Write-Host "Arquivo principal: $Output\ControleEstoque.exe" -ForegroundColor White
Write-Host ""
Write-Host "Para distribuir, copie a pasta inteira para o computador de destino." -ForegroundColor Yellow
Write-Host "Edite appsettings.json com os dados do PostgreSQL antes de executar." -ForegroundColor Yellow

@echo off
setlocal

if not exist .env (
    echo Arquivo .env nao encontrado. Copiando de .env.example...
    copy .env.example .env
)

echo Subindo PostgreSQL e aplicando migrations...
docker compose up -d --build

echo.
echo Ambiente pronto!
echo   PostgreSQL: localhost:5432
echo   Banco: controle_estoque
echo   Usuario: postgres
echo.
echo Execute a aplicacao WinForms com:
echo   dotnet run --project ControleEstoque
echo.

endlocal

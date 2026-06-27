#!/bin/sh
set -e

CONN="${CONNECTION_STRING:?CONNECTION_STRING não definida}"

echo "Aplicando migrations no banco de dados..."
dotnet ef database update \
  --project ControleEstoque/ControleEstoque.csproj \
  --startup-project ControleEstoque/ControleEstoque.csproj \
  --configuration Release \
  --framework net8.0-windows \
  --no-build \
  --connection "$CONN"

echo "Migrations aplicadas com sucesso."

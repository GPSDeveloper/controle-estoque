#!/bin/sh
set -e

CONN="${CONNECTION_STRING:?CONNECTION_STRING não definida}"

echo "Aplicando migrations no banco de dados..."
dotnet ef database update \
  --project ControleEstoque/ControleEstoque.csproj \
  --connection "$CONN"

echo "Migrations aplicadas com sucesso."

#!/bin/sh
set -e

CONN="${CONNECTION_STRING:?CONNECTION_STRING não definida}"
export CONNECTION_STRING="$CONN"

echo "Aplicando migrations no banco de dados..."
dotnet /src/ControleEstoque.Migrator/bin/Release/net8.0/ControleEstoque.Migrator.dll

echo "Migrations aplicadas com sucesso."

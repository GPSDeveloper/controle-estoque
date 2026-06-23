#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "$0")/.." && pwd)"
cd "$ROOT"

if [ ! -f .env ]; then
  echo "Arquivo .env não encontrado. Copiando de .env.example..."
  cp .env.example .env
fi

echo "Subindo PostgreSQL e aplicando migrations..."
docker compose up -d --build

echo ""
echo "Ambiente pronto!"
echo "  PostgreSQL: localhost:5432"
echo "  Banco: controle_estoque"
echo "  Usuário: postgres"
echo ""
echo "Execute a aplicação WinForms no Windows com:"
echo "  dotnet run --project ControleEstoque"
echo ""

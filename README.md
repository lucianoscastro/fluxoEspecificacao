# fluxoEspecificacao — Pet Registration API

![.NET](https://img.shields.io/badge/.NET-10.0-512BD4)
![License](https://img.shields.io/badge/License-MIT-green)

> Exemplo prático que acompanha o artigo do [`LinkedIn`](https://www.linkedin.com/pulse/ilus%C3%A3o-do-prompt-perfeito-por-que-o-seu-fluxo-de-ia-precisa-castro-abo8f/) sobre **desenvolvimento guiado por especificação** (_spec-driven_) com agentes de IA.
>
> O código aqui é proposital­mente simples. O valor está em **como** ele foi produzido: de uma ideia em linguagem natural até a implementação, passando por PRD, especificação técnica, decisões de arquitetura e tarefas — tudo versionado em [`.compozy/`](.compozy/tasks/pet-registration/).

---

## O fluxo de especificação

O coração deste repositório não é a API — é a trilha de artefatos que a originou. Cada etapa refina a anterior antes de uma linha de código ser escrita:

| Etapa | Artefato | O que define |
|---|---|---|
| 1. Produto | [`_prd.md`](.compozy/tasks/pet-registration/_prd.md) | Problema, personas, user stories, escopo e _não-escopo_ |
| 2. Decisão de produto | [`adr-001.md`](.compozy/tasks/pet-registration/adrs/adr-001.md) | Cadastro em etapa única, tutor como texto livre |
| 3. Especificação técnica | [`_techspec.md`](.compozy/tasks/pet-registration/_techspec.md) | Arquitetura, modelos, contrato HTTP, testes |
| 4. Decisão técnica | [`adr-002.md`](.compozy/tasks/pet-registration/adrs/adr-002.md) | Storage in-memory, arquivo único, RFC 7807 |
| 5. Tarefas | [`_tasks.md`](.compozy/tasks/pet-registration/_tasks.md) | Decomposição executável e ordem de build |
| 6. Código | [`Program.cs`](fluxoEspecificacao/Program.cs) | Implementação que satisfaz a especificação |

A leitura na ordem acima reproduz o raciocínio do artigo: **a especificação é o produto; o código é a consequência.**

---

## A API

Endpoint mínimo para cadastro de pets em clínicas veterinárias — captura nome, raça, data de nascimento e tutor, com validação de integridade.

### Stack

- **.NET 10** — ASP.NET Core Minimal API (arquivo único em `Program.cs`)
- **Armazenamento in-memory** — `ConcurrentDictionary<Guid, Pet>` como singleton (sem banco; dados se perdem no restart — escolha consciente para o MVP, ver [ADR-002](.compozy/tasks/pet-registration/adrs/adr-002.md))
- **Erros padronizados** — RFC 7807 _Problem Details_ via `Results.ValidationProblem`
- **Documentação** — OpenAPI (`Microsoft.AspNetCore.OpenApi`) + Swagger UI (`Swashbuckle.AspNetCore.SwaggerUI`)

### Como executar

Pré-requisito: **.NET 10 SDK**.

```bash
cd fluxoEspecificacao
dotnet run
```

A API sobe em `http://localhost:5010` (perfil `http`). Em ambiente de desenvolvimento ficam disponíveis:

| Recurso | URL |
|---|---|
| Swagger UI | http://localhost:5010/swagger |
| Documento OpenAPI | http://localhost:5010/openapi/v1.json |

### Endpoint

#### `POST /pets`

| Campo | Tipo | Regra |
|---|---|---|
| `name` | string | Obrigatório, não vazio |
| `breed` | string | Obrigatório, texto livre |
| `birthDate` | date | Obrigatório, deve estar no passado |
| `tutorName` | string | Obrigatório, não vazio |

**Requisição**

```bash
curl -X POST http://localhost:5010/pets \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Bidu",
    "breed": "SRD",
    "birthDate": "2020-05-15",
    "tutorName": "Maria Silva"
  }'
```

**Sucesso — `201 Created`** (com header `Location: /pets/{id}`)

```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "name": "Bidu",
  "breed": "SRD",
  "birthDate": "2020-05-15T00:00:00Z",
  "tutorName": "Maria Silva"
}
```

**Erro de validação — `400 Bad Request`** (RFC 7807)

```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "BirthDate": ["Birth date must be in the past."]
  }
}
```

---

## Estrutura do repositório

```
.
├── .compozy/tasks/pet-registration/   # Artefatos do fluxo de especificação
│   ├── _prd.md                        # Product Requirements Document
│   ├── _techspec.md                   # Especificação técnica
│   ├── _tasks.md                      # Tarefas decompostas
│   └── adrs/                          # Architecture Decision Records
├── fluxoEspecificacao/                # Projeto da API
│   ├── Program.cs                     # Endpoint, modelos e validação
│   └── fluxoEspecificacao.csproj
├── fluxoEspecificacao.slnx
├── LICENSE
└── README.md
```

---

## Licença

[MIT](LICENSE) © 2026 Luciano Castro

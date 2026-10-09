# Skolehjemsamtale

Et samtaleværktøj til skole-hjem-samarbejde, bygget i ASP.NET Core (.NET 8).

Værktøjet følger op på de **observationer af elevens adfærd**, som læreren har nedfældet i undervisningen,
og støtter en **ærlig og vækstorienteret samtale** der:

- fremmer forældrenes oplevelse af **samarbejde** med skolen,
- giver **eleven en reel stemme** og en oplevelse af at blive hørt,
- indsamler **ny viden fra hjemmet**, som ikke er tilgængelig i skolekonteksten.

Projektet er bygget API-first og med et eksplicit **anti-corruption-lag**, så det senere kan kobles
sammen med det andet projekt **Techtree** – uanset hvilken teknologi Techtree er bygget i.

## Kom i gang

```bash
# 1) Installer .NET 8 SDK (én gang)
curl -sSL https://dot.net/v1/dotnet-install.sh | bash -s -- --channel 8.0

# 2) Byg og test
dotnet build
dotnet test

# 3) Kør webappen
dotnet run --project src/Skolehjemsamtale.Web
```

Åbn derefter:

| URL | Indhold |
| --- | --- |
| `/` | Overblik (dansk UI) |
| `/Elever` | Elever, observationer og hjemmets bidrag |
| `/Samtaler` | Samtaler og planlægning |
| `/Metode` | Den researchbaserede samtalestruktur |
| `/swagger` | REST-API (OpenAPI) |
| `/health` | Health check |

Ved første opstart oprettes SQLite-databasen og seedes med demodata.
Læg en eksportfil fra det gamle værktøj i `seed/samtale_vaerktoej_data.json` for at importere rigtige data
(format: se `seed/samtale_vaerktoej_data.example.json`).

## Projektstruktur

```
src/Skolehjemsamtale.Domain          Entiteter, enums og den researchbaserede samtalestruktur
src/Skolehjemsamtale.Application     Use cases, DTO'er (stabil kontrakt) og integrationskontrakter
src/Skolehjemsamtale.Infrastructure  EF Core/SQLite, outbox, Techtree-gateway, import
src/Skolehjemsamtale.Web             Razor Pages-UI + REST-API (api/v1) – kompositionsrod
tests/Skolehjemsamtale.Tests         Domæne-, kvalitets-, import- og outbox-tests
docs/                                Research, arkitektur og integrationsguide
```

## Dokumentation

- [`docs/research.md`](docs/research.md) – forskning i skolehjemsamarbejde, skolevægring og feedback-/samtalekultur.
- [`docs/arkitektur.md`](docs/arkitektur.md) – arkitektur, domænemodel og udvidelsespunkter.
- [`docs/integration-techtree.md`](docs/integration-techtree.md) – sådan kobles projektet til Techtree.

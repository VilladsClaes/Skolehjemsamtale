# Arkitektur

## Overblik

Løsningen er delt i fire projekter med en klassisk afhængighedsretning indad. Web er den eneste
kompositionsrod; domænet kender ingen infrastruktur.

```
Skolehjemsamtale.Web  ──►  Infrastructure  ──►  Application  ──►  Domain
        │                        │
        └────────────────────────┴──────────────►  Application  ──►  Domain
```

| Projekt | Ansvar | Må ikke kende |
| --- | --- | --- |
| `Domain` | Entiteter, enums, forretningsregler og samtalestrukturen | EF, HTTP, JSON |
| `Application` | Use cases (services), DTO'er og integrationskontrakter + port-interfaces | EF, web |
| `Infrastructure` | EF Core/SQLite, repositories, outbox, Techtree-gateway, import | Web |
| `Web` | Razor Pages-UI, REST-API (`api/v1`), DI, seed ved opstart | – |

## Domænemodel

```
Elev ──1:N── Observation            (observerbar adfærd nedfældet i undervisningen)
 │
 ├──1:N── Samtale  (aggregat)
 │          ├── SamtaleDeltager       (Elev/Forælder/Lærer/…)
 │          ├── SamtaleFaseNote       (note pr. trin + Perspektiv: Skole/Hjem/Elev)
 │          ├── Handlingsaftale       (Beskrivelse, Ansvarlig, Frist, Evalueringsdato)
 │          ├── SamtaleObservation    (kobling til de observationer samtalen følger op på)
 │          └── HjemmeIndsigt         (viden fra hjemmet)
 └──1:N── HjemmeIndsigt
```

**Invarianser i domænet** (håndhæves i entiteterne, ikke i UI):

- `Samtale.Afslut()` kræver mindst én `Handlingsaftale` **og** en note i fasen `StemmeTjek`.
  En samtale kan altså ikke lukkes uden et fælles næste skridt og et tjek af, om parterne blev hørt.
- `Observation` kræver beskrivelse og observatør (adskiller observeret adfærd fra tolkning).
- En aflyst samtale kan ikke markeres afholdt eller afsluttes.

## Samtalestrukturen som data

Den researchbaserede struktur ligger i `Domain/Framework` (`DefaultSamtaleSkabeloner`,
`HjemmeSpoergeramme`) – ikke som løs tekst i UI. Det betyder, at samme vejledning kan vises i UI,
eksponeres over API'et (`/api/v1/metode/...`) og genbruges af Techtree.

Tre skabeloner: `skole-hjem-grund`, `skolevaegring-opfoelgning` og `elevsamtale`, hver med 7–8 trin,
vejledende spørgsmål og "vær opmærksom"-punkter.

## Kvalitetsvurdering

`SamtaleKvalitetEvaluator` (Application) vurderer en samtale mod ni principper fra forskningen
(observation, styrke, hjem, elev, fælles forståelse, aftale, delt ansvar, opfølgning, stemmetjek) og
giver en score. Den bruges i UI, over API'et og sendes med i `samtale.afsluttet`-eventet til Techtree.

## Persistens

- EF Core 8 + SQLite (fil `skolehjemsamtale.db`, oprettes og seedes ved opstart).
- Alle id'er er klient-genererede `Guid` (konfigureret `ValueGeneratedNever`), så poster kan udveksles
  mellem systemer uden lokale identity-kolonner.
- `QuerySplittingBehavior.SplitQuery` for aggregater med flere kollektioner.
- Aggregatet `Samtale` hentes med `GetMedDetaljerAsync()` inkl. børn.

Skift til en anden database ved at ændre `UseSqlite(...)` i `Infrastructure/DependencyInjection.cs`;
domæne og applikation er uafhængige af valget. Til produktionsbrug med PostgreSQL/SQL Server bør
`EnsureCreated` erstattes af EF-migrationer.

## Udvidelsespunkter

| Behov | Sted |
| --- | --- |
| Ny samtalestruktur | `DefaultSamtaleSkabeloner` (tilføj en `SamtaleSkabelon`) |
| Ny observationstype | `ObservationKategori` |
| Ny aktør | `SamtaleRolle` |
| Anden database | `Infrastructure/DependencyInjection.cs` |
| Andet integrationsmål end Techtree | ny `ITechtreeGateway`-implementering |
| Flere events | tilføj kontrakt i `Application/Integration/Contracts.cs` + publicering i en service |

## Test

`tests/Skolehjemsamtale.Tests` dækker domæneinvarianser, kvalitetsvurdering, import fra det gamle
format og outbox-integration mod en isoleret SQLite-database.

```bash
dotnet test
```

# Integration med Techtree

Værktøjet er bygget, så det **kan kobles til Techtree senere – uanset hvordan Techtree er bygget**.
Integrationen er ikke antaget at være "endnu en .NET-app": kontrakterne er versionsstyrede,
teknologineutrale og afsendes asynkront.

## Principper

1. **API-first, ikke delt database.** Skolehjemsamtale eksponerer et versioneret REST-API (`/api/v1`)
   og udsender events – Techtree læser ikke direkte i vores database.
2. **Anti-corruption-lag.** Alt der vedrører modtagerens protokol og format ligger i
   `Infrastructure/Integration` bag interfacet `ITechtreeGateway`. Resten af systemet kender ikke Techtree.
3. **Transaktionel outbox.** Et event skrives i `OutboxMessages` i samme database som ændringen og
   leveres af en baggrundsprocessor med retry. En utilgængelig Techtree blokerer aldrig lærerens arbejde.
4. **Idempotens.** Hvert event har et stabilt `Id` (sendt som `X-Event-Id`) og et versionsnummer.
   Modtageren kan deduplisere på `X-Event-Id`.
5. **Løs kobling i data.** Alle id'er er `Guid` (som tekst), tidsstempler ISO-8601 UTC, og enums
   som tekst. Ingen delte databasetabeller og ingen afhængighed af vores identity-kolonner.

## Kontrakter (versiones 1)

Defineret i `Application/Integration/Contracts.cs`:

| EventType | Payload | Udløses af |
| --- | --- | --- |
| `observation.registreret` | `ObservationRegistreretV1` | ny observation i undervisningen |
| `samtale.planlagt` | `SamtalePlanlagtV1` | samtale oprettet |
| `samtale.afsluttet` | `SamtaleAfsluttetV1` (inkl. kvalitetsscore, inddragelse) | samtale afsluttet |
| `aftale.oprettet` | `AftaleOprettetV1` | handlingsaftale tilføjet |
| `hjemmeindsigt.registreret` | `HjemmeIndsigtRegistreretV1` | viden registreret fra hjemmet |

Den rå `Payload` er JSON og leveres i en kuvert:

```json
{
  "eventType": "samtale.afsluttet",
  "version": 1,
  "aggregateType": "Samtale",
  "aggregateId": "…",
  "elevId": "…",
  "occurredAt": "2026-10-09T11:00:00+00:00",
  "payload": "{ \"samtaleId\": \"…\", \"kvalitetsscore\": 89, … }"
}
```

HTTP-headere: `X-Source: Skolehjemsamtale`, `X-Event-Id: <outbox-id>` og – hvis en delt hemmelighed er
konfigureret – `X-Signature` (HMAC-SHA256 over body med `TECHTREE_SHARED_SECRET`).

## Konfiguration

`appsettings.json`:

```json
"Techtree": {
  "BaseUrl": "https://techtree.example.dk",
  "EventsPath": "/api/integration/v1/events",
  "SharedSecretName": "TECHTREE_SHARED_SECRET",
  "Enabled": true,
  "BatchSize": 25,
  "MaksForsoeg": 8
}
```

- Er `BaseUrl` tom, er integrationen **deaktiveret**: events bliver liggende i outbox (intet går tabt),
  og HTTP-gatewayen erstattes af `NoopTechtreeGateway`.
- Selve hemmeligheden læses fra en miljøvariabel/secret med navnet i `SharedSecretName` – den ligger
  aldrig i repo eller konfigurationsfil.

## Drift og overvågning

| Endpoint | Formål |
| --- | --- |
| `GET /api/v1/integration/status` | antal ventende / behandlede / fejlede events |
| `GET /api/v1/integration/outbox?take=50` | de seneste outbox-poster inkl. fejl |

Fejlede events bevares (markeres behandlet efter `MaksForsoeg`) med fejlbesked, så de kan genafspilles
manuelt i stedet for at gå tabt.

## Sådan tilsluttes Techtree – trin for trin

1. **Bliv enige om kuverten.** Bekræft at Techtree kan modtage `POST` med JSON-kuverten ovenfor (eller
   tilpas `HttpTechtreeGateway` til modtagerens format – det er netop dette lags formål).
2. **Opsæt endpoint i Techtree** til at modtage events, verificere `X-Signature`, og deduplisere på `X-Event-Id`.
3. **Sæt `Techtree:BaseUrl`** (og hemmeligheden i miljøet) og genstart appen.
4. **Rækkefølge og gentagelser.** Outbox-processoren leverer i FIFO-rækkefølge og forsøger igen.
   Sørg for, at modtageren er idempotent.
5. **Brug `Elev.EksterntId`** til at koble elever til Techtrees id'er, og sæt `Entity.EksterntId`, når
   en post spejles tilbage. Feltet findes på alle entiteter netop til dette.
6. **Vælg retning pr. datatype.** Observationer og samtaler ejes af Skolehjemsamtale. Hvis Techtree
   også opretter elever, så aftal én ejer pr. felt og synkronisér via begivenheder – ikke via delt database.
7. **Versionering.** Ved ændringer i en payload: opret `...V2` og hæv `Versionsnummer`; fjern ikke felter
   i V1, før Techtree er migreret.

## Hvis Techtree er bygget anderledes

Teknologivalg i Techtree er ligegyldigt for integrationen, fordi koblingen kun er:

- **JSON over HTTP** (kan erstattes af fx en beskedkø ved at udskifte `ITechtreeGateway`), og
- **Guid + ISO-8601 + tekst-enums** i data.

Er Techtree fx bygget på en anden datamodel, løses det i en mapping i Techtree (eller i en ny gateway),
ikke ved at ændre domænet her. Er Techtree selv en ASP.NET Core-app, kan den i stedet konsumere vores
REST-API (`/swagger` viser hele kontrakten) og lytte på webhook-events.

## Åbne punkter før produktionsdrift

- **Autentificering af de indgående `/api/v1`-endpoints** (i dag åbne i demo). Sæt et token/API-nøgle-lag
  foran `api/v1`.
- **EF-migrationer** i stedet for `EnsureCreated` (se `docs/arkitektur.md`).
- **Valg af transport** til Techtree (HTTP-webhook eller kø) og aftale om retry/backoff.
- **Databehandling og fortrolighed**: elevdata er personoplysninger – afklar hvad der må deles med
  Techtree, logning og sletteprocedurer.

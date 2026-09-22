# Applied

Personlig jobbsøk-verktøy: legg inn stillinger du finner (Finn.no, LinkedIn, andre), få AI-matchet score mot CV-en din og et utkast til søknadsbrev, og godkjenn før noe sendes.

Bygget videre fra en tidligere prototype ("Jobbradar") som kjørte helt inne i Claude. Dette er den "ordentlige" versjonen — egen database, egen backend, flere brukere.

## Stack

- **Backend:** ASP.NET Core Web API (.NET 8), EF Core, PostgreSQL, ASP.NET Identity + JWT
- **Frontend:** Angular 18 (standalone components)

## Hvorfor ingen automatisk skanning av Finn.no/LinkedIn

Verken Finn.no eller LinkedIn tilbyr et offentlig API for å lese eller sende jobbsøknader programmatisk. Automatisert skanning/utfylling av skjemaene deres bryter begges brukervilkår og kan føre til at kontoen din blir sperret.

Applied er derfor bevisst bygget rundt **"lim inn en lenke → vi henter og analyserer den"**, ikke bakgrunnsskanning. Det samme gjelder innsending: en søknad kan aldri gå fra `New` rett til `Sent` i API-et — den må innom `Approved` først (se `JobsController.SetStatus`). Det er en arkitektonisk grense, ikke bare en regel noen følger.

## Kom i gang

### Forutsetninger
- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Node.js 20+](https://nodejs.org) og npm
- Docker (for lokal Postgres) — eller egen Postgres-instans

### 1. Database

```bash
docker compose up -d
```

Starter Postgres på `localhost:5432` (bruker/passord: `applied` / `applied_dev_password`, kun for lokal utvikling).

### 2. Backend

```bash
cd src/Applied.Api
dotnet user-secrets init
dotnet user-secrets set "Jwt:Key" "<en-lang-tilfeldig-hemmelig-streng-minst-32-tegn>"
dotnet ef database update   # krever: dotnet tool install --global dotnet-ef
dotnet run
```

API-et kjører på `https://localhost:5001` (Swagger på `/swagger` i development).

**Viktig:** `appsettings.Development.json` har en placeholder-nøkkel (`CHANGE_ME`) kun så prosjektet skal bygge — bruk alltid `dotnet user-secrets` (eller miljøvariabler i produksjon) for den ekte JWT-nøkkelen. Aldri commit en ekte hemmelighet til git.

### 3. Frontend

```bash
cd applied-web
npm install
npm start
```

Kjører på `http://localhost:4200`, med `/api`-kall proxyet til backend (se `proxy.conf.json`).

## Prosjektstruktur

```
src/
  Applied.Domain/          Entiteter (User, CvProfile, JobListing, JobApplication)
  Applied.Infrastructure/  EF Core DbContext
  Applied.Api/             Kontrollere, auth, Program.cs
applied-web/                Angular-frontend
docker-compose.yml          Lokal Postgres
```

## Status / veikart

Dette er et skjelett — det bygger og kjører, med innlogging, datamodeller og et grunnleggende dashboard, men mangler blant annet:

- [ ] EF Core-migrasjoner (kjør `dotnet ef migrations add Init` i `Applied.Api` etter første `dotnet restore`)
- [ ] "Legg til stilling"-skjema i UI (i dag: `POST /api/jobs` direkte)
- [ ] AI-matching og generering av søknadsutkast (flyttes hit fra Claude-chatten)
- [ ] E-postutsending for stillinger som tar imot søknad på e-post
- [ ] Deploy (Azure App Service / Railway / annet)

## Push til GitHub

```bash
cd applied   # denne mappen
git init
git add .
git commit -m "Initial Applied skeleton: .NET API + Angular"
git branch -M main
git remote add origin https://github.com/thanhibanani/applied.git
git push -u origin main
```

(Opprett det tomme repoet `applied` på github.com/thanhibanani først.)

### 👋 Hi, I'm Ajay Shrestha

I'm a passionate developer exploring full-stack technologies and building modern solutions across **Web**, **Android**, and **Desktop platforms** using **.NET MAUI**, **C#**, and **JavaScript**.

---

## Neon Database + Netlify Functions Setup

This repo now includes a backend-ready setup so your Netlify-hosted portfolio can persist **skills**, **experience**, and other structured data in **Neon Postgres**.

### 1) Install dependencies

```bash
npm install
```

### 2) Create a Neon database

1. Create a project in Neon.
2. Copy the connection string (pooled connection is recommended).

### 3) Add environment variable in Netlify

In Netlify site settings, add:

- `DATABASE_URL=<your-neon-connection-string>`

### 4) Deploy to Netlify

`netlify.toml` is configured so API routes map to Netlify Functions:

- `/api/init-schema` → creates tables
- `/api/seed-portfolio` → inserts starter data
- `/api/get-portfolio-data` → reads all portfolio data

### 5) Initialize schema and seed data

After first deploy, call:

- `/.netlify/functions/init-schema`
- `/.netlify/functions/seed-portfolio`

Or use the redirected routes:

- `/api/init-schema`
- `/api/seed-portfolio`

---

## Database Tables Created

The schema is available in `sql/schema.sql` and is created by `netlify/functions/init-schema.js`.

### Core tables

- `profile` - owner details
- `skills` - skills list grouped by category
- `experiences` - job timeline entries
- `experience_highlights` - bullet points linked to each experience
- `projects` - project portfolio entries
- `other_data` - flexible JSON data (metrics, contact blocks, etc.)

---

## API Endpoints

### `GET /api/init-schema`
Creates all required tables in Neon.

### `GET /api/seed-portfolio`
Seeds sample profile, skills, experiences, and metadata.

### `GET /api/get-portfolio-data`
Returns a combined JSON payload:

- profile
- skills
- experiences (with highlights)
- otherData

---

## Notes

- Keep `DATABASE_URL` only in Netlify environment variables (not in source control).
- If you add new sections to the portfolio UI, store them in `other_data` or add dedicated tables.
- `skills` and `experiences` are upsert-safe for repeated seeding.

# Run Neon + Netlify functions from GitHub Actions

This repository includes a manual GitHub Actions workflow that can call your deployed Netlify function endpoints to:

1. initialize Neon tables (`/api/init-schema`)
2. optionally seed data (`/api/seed-portfolio`)

## 1) Configure repository secret

In GitHub → **Settings** → **Secrets and variables** → **Actions**, add:

- `NETLIFY_SITE_URL` = your deployed site URL (for example `https://your-site.netlify.app`)

## 2) Trigger workflow

1. Open **Actions** tab
2. Select **Neon + Netlify Functions**
3. Click **Run workflow**
4. Choose whether to run seed (`true`/`false`)

The workflow will call:

- `${NETLIFY_SITE_URL}/api/init-schema`
- `${NETLIFY_SITE_URL}/api/seed-portfolio` (when enabled)

## 3) Continuous checks

`Node Lint` workflow runs on push and pull request and executes:

- `npm install`
- `npm run lint`

This validates Netlify function JavaScript syntax in CI.

const { getPool } = require('./db');

const ddl = `
CREATE TABLE IF NOT EXISTS profile (
  id SERIAL PRIMARY KEY,
  full_name TEXT NOT NULL,
  headline TEXT,
  bio TEXT,
  email TEXT,
  linkedin_url TEXT,
  created_at TIMESTAMPTZ DEFAULT NOW(),
  updated_at TIMESTAMPTZ DEFAULT NOW()
);

CREATE TABLE IF NOT EXISTS skills (
  id SERIAL PRIMARY KEY,
  category TEXT NOT NULL,
  title TEXT NOT NULL,
  details TEXT,
  sort_order INT DEFAULT 0,
  created_at TIMESTAMPTZ DEFAULT NOW(),
  UNIQUE (category, title)
);

CREATE TABLE IF NOT EXISTS experiences (
  id SERIAL PRIMARY KEY,
  company TEXT NOT NULL,
  role TEXT NOT NULL,
  start_date DATE NOT NULL,
  end_date DATE,
  is_current BOOLEAN DEFAULT FALSE,
  summary TEXT,
  sort_order INT DEFAULT 0,
  created_at TIMESTAMPTZ DEFAULT NOW(),
  UNIQUE (company, role, start_date)
);

CREATE TABLE IF NOT EXISTS experience_highlights (
  id SERIAL PRIMARY KEY,
  experience_id INT NOT NULL REFERENCES experiences(id) ON DELETE CASCADE,
  highlight TEXT NOT NULL,
  sort_order INT DEFAULT 0
);

CREATE TABLE IF NOT EXISTS projects (
  id SERIAL PRIMARY KEY,
  name TEXT NOT NULL,
  description TEXT,
  tech_stack TEXT,
  project_url TEXT,
  sort_order INT DEFAULT 0,
  created_at TIMESTAMPTZ DEFAULT NOW(),
  UNIQUE (name)
);

CREATE TABLE IF NOT EXISTS other_data (
  id SERIAL PRIMARY KEY,
  data_key TEXT UNIQUE NOT NULL,
  data_value JSONB NOT NULL,
  created_at TIMESTAMPTZ DEFAULT NOW(),
  updated_at TIMESTAMPTZ DEFAULT NOW()
);
`;

exports.handler = async () => {
  try {
    const pool = getPool();
    await pool.query(ddl);

    return {
      statusCode: 200,
      body: JSON.stringify({ message: 'Schema initialized successfully.' })
    };
  } catch (error) {
    return {
      statusCode: 500,
      body: JSON.stringify({ error: error.message })
    };
  }
};

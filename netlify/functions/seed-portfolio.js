const { getPool } = require('./db');

exports.handler = async () => {
  const pool = getPool();
  const client = await pool.connect();

  try {
    await client.query('BEGIN');

    await client.query(`
      INSERT INTO profile (full_name, headline, bio, email, linkedin_url)
      VALUES ($1, $2, $3, $4, $5)
      ON CONFLICT DO NOTHING
    `, [
      'Ajay Shrestha',
      'Associate Software Engineer',
      'Building polished apps and robust backend workflows.',
      'ajay.shrestha54@gmail.com',
      'https://www.linkedin.com/in/ajay-stha/'
    ]);

    await client.query(`
      INSERT INTO skills (category, title, details, sort_order)
      VALUES
        ('App Development', '.NET MAUI', 'WPF (MVVM), XAML, Xamarin migration', 1),
        ('Languages', 'C#, Python, JavaScript', 'TypeScript basics', 2),
        ('Data & Storage', 'SQL Server, SQLite, Firebase', 'Firestore, LINQ', 3),
        ('Web & APIs', 'ASP.NET MVC, REST APIs', 'jQuery, Vue.js, AJAX', 4)
      ON CONFLICT DO NOTHING
    `);

    const exp1 = await client.query(`
      INSERT INTO experiences (company, role, start_date, end_date, is_current, summary, sort_order)
      VALUES ($1, $2, $3, $4, $5, $6, $7)
      ON CONFLICT (company, role, start_date) DO UPDATE SET
        end_date = EXCLUDED.end_date,
        is_current = EXCLUDED.is_current,
        summary = EXCLUDED.summary,
        sort_order = EXCLUDED.sort_order
      RETURNING id
    `, [
      'Bajra Technologies',
      'Associate Software Engineer',
      '2024-06-01',
      null,
      true,
      'Led dashboard integration and .NET MAUI app development.',
      1
    ]);

    const exp2 = await client.query(`
      INSERT INTO experiences (company, role, start_date, end_date, is_current, summary, sort_order)
      VALUES ($1, $2, $3, $4, $5, $6, $7)
      ON CONFLICT (company, role, start_date) DO UPDATE SET
        end_date = EXCLUDED.end_date,
        is_current = EXCLUDED.is_current,
        summary = EXCLUDED.summary,
        sort_order = EXCLUDED.sort_order
      RETURNING id
    `, [
      'BISage Pvt. Ltd.',
      'Associate Software Engineer',
      '2022-01-01',
      '2024-06-01',
      false,
      'Developed HR, POS, and CRM modules with MVC and jQuery.',
      2
    ]);

    await client.query('DELETE FROM experience_highlights WHERE experience_id = ANY($1::int[])', [[exp1.rows[0].id, exp2.rows[0].id]]);

    await client.query(`
      INSERT INTO experience_highlights (experience_id, highlight, sort_order)
      VALUES
        ($1, 'Led dashboard integration with REST APIs.', 1),
        ($1, 'Mentored junior developers and trainees.', 2),
        ($1, 'Developed .NET MAUI apps with Firebase backend.', 3),
        ($2, 'Created .NET libraries for large-scale data ingestion.', 1),
        ($2, 'Developed HR, POS, CRM modules using MVC and jQuery.', 2),
        ($2, 'Improved UI/UX with custom JavaScript and reporting tools.', 3)
    `, [exp1.rows[0].id, exp2.rows[0].id]);

    await client.query(`
      INSERT INTO other_data (data_key, data_value)
      VALUES
        ('metrics', '{"yearsExperience":"3+","productionModules":"10+","coreTechStacks":"6+"}'::jsonb),
        ('contact', '{"email":"ajay.shrestha54@gmail.com","cta":"Contact Me"}'::jsonb)
      ON CONFLICT (data_key) DO UPDATE SET
        data_value = EXCLUDED.data_value,
        updated_at = NOW()
    `);

    await client.query('COMMIT');

    return {
      statusCode: 200,
      body: JSON.stringify({ message: 'Portfolio data seeded.' })
    };
  } catch (error) {
    await client.query('ROLLBACK');
    return {
      statusCode: 500,
      body: JSON.stringify({ error: error.message })
    };
  } finally {
    client.release();
  }
};

const { getPool } = require('./db');

exports.handler = async () => {
  try {
    const pool = getPool();

    const [profileRes, skillsRes, expRes, highlightsRes, otherDataRes] = await Promise.all([
      pool.query('SELECT * FROM profile ORDER BY id DESC LIMIT 1'),
      pool.query('SELECT * FROM skills ORDER BY sort_order, id'),
      pool.query('SELECT * FROM experiences ORDER BY sort_order, start_date DESC'),
      pool.query('SELECT * FROM experience_highlights ORDER BY experience_id, sort_order, id'),
      pool.query('SELECT data_key, data_value FROM other_data')
    ]);

    const highlightsByExperience = highlightsRes.rows.reduce((acc, item) => {
      if (!acc[item.experience_id]) {
        acc[item.experience_id] = [];
      }
      acc[item.experience_id].push(item.highlight);
      return acc;
    }, {});

    const experiences = expRes.rows.map((experience) => ({
      ...experience,
      highlights: highlightsByExperience[experience.id] || []
    }));

    const otherData = otherDataRes.rows.reduce((acc, row) => {
      acc[row.data_key] = row.data_value;
      return acc;
    }, {});

    return {
      statusCode: 200,
      body: JSON.stringify({
        profile: profileRes.rows[0] || null,
        skills: skillsRes.rows,
        experiences,
        otherData
      })
    };
  } catch (error) {
    return {
      statusCode: 500,
      body: JSON.stringify({ error: error.message })
    };
  }
};

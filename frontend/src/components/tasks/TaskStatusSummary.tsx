import { useEffect, useState, useContext } from 'react';
import { TokenContext } from '../../App';
import { fetchTasksSummary } from './api';
import type { Summary } from './api';

const statusColours: Record<string, string> = {
  Pending: '#f59e0b',
  InProgress: '#3b82f6',
  Completed: '#10b981',
  Archived: '#6b7280',
};

export default function TaskStatusSummary() {
  const token = useContext(TokenContext);
  const [summary, setSummary] = useState<Summary | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!token) return;
    let mounted = true;
    setLoading(true);
    setError(null);
    fetchTasksSummary(token)
      .then(data => {
        if (mounted) setSummary(data as Summary);
      })
      .catch(e => {
        if (mounted) setError(String(e));
      })
      .finally(() => {
        if (mounted) setLoading(false);
      });
    return () => {
      mounted = false;
    };
  }, [token]);

  if (!token) return null;
  if (loading) return <div style={{ textAlign: 'center', color: '#666' }}>Loading status summary...</div>;
  if (error) return <div style={{ textAlign: 'center', color: '#c53030' }}>Unable to load summary: {error}</div>;
  if (!summary || !summary.counts || summary.counts.length === 0) return <div style={{ textAlign: 'center', color: '#666' }}>No tasks to summarize.</div>;

  const total = summary.total || summary.counts.reduce((a, b) => a + b.count, 0);

  const rows = [...summary.counts].sort((a, b) => b.count - a.count);

  return (
    <div style={{ maxWidth: 420, margin: '0 auto 16px', padding: 12, borderRadius: 8 }}>
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'baseline', marginBottom: 8 }}>
        <h3 style={{ margin: 0, fontSize: 16, color: '#1f2937' }}>Task status</h3>
        <div style={{ fontSize: 13, color: '#6b7280' }}>{total} total</div>
      </div>
      <div style={{ display: 'grid', gap: 10 }}>
        {rows.map(r => {
          const percentage = total ? Math.round((r.count / total) * 100) : 0;
          const colour = statusColours[r.status] ?? '#8b5cf6';
          return (
            <div key={r.status} style={{ display: 'flex', flexDirection: 'column', gap: 6 }}>
              <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                <div style={{ fontSize: 14, color: '#374151', fontWeight: 600 }}>{r.status}</div>
                <div style={{ fontSize: 13, color: '#6b7280' }}>{r.count} ({percentage}%)</div>
              </div>
              <div style={{ background: '#e6edf7', height: 10, borderRadius: 6, overflow: 'hidden' }}>
                <div style={{ width: `${percentage}%`, height: '100%', background: colour }} />
              </div>
            </div>
          );
        })}
      </div>
    </div>
  );
}

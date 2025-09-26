import { useEffect, useState, useContext, useRef, useCallback } from 'react';
import { Link } from 'react-router-dom';
import { fetchTasks, Priority, Status } from './api';
import { selectStyle } from '../../styles/ui';
import type { TaskItem } from './api';
import { TokenContext } from '../../App';
import TaskStatusSummary from './TaskStatusSummary';

export default function TaskList() {
  const token = useContext(TokenContext);
  const [tasks, setTasks] = useState<TaskItem[]>([]);
  const [priority, setPriority] = useState<Priority | ''>('');
  const [status, setStatus] = useState<Status | ''>('');
  const [loading, setLoading] = useState(false);
  const [nextCursor, setNextCursor] = useState<string | null>(null);
  const [hasNextPage, setHasNextPage] = useState(false);
  const sentinelRef = useRef<HTMLDivElement | null>(null);

  useEffect(() => {
    if (!token) return;
    setLoading(true);
    setTasks([]);
    setNextCursor(null);
    setHasNextPage(false);
    fetchTasks(token, priority, status, null)
      .then(result => {
        setTasks(result.items);
        setNextCursor(result.nextCursor);
        setHasNextPage(result.hasNextPage);
      })
      .finally(() => setLoading(false));
  }, [token, priority, status]);

  const loadMore = useCallback(async () => {
    if (!token || !hasNextPage) return;
    setLoading(true);
    try {
      const result = await fetchTasks(token, priority, status, nextCursor);
      setTasks(prev => [...prev, ...result.items]);
      setNextCursor(result.nextCursor);
      setHasNextPage(result.hasNextPage);
    } finally {
      setLoading(false);
    }
  }, [token, hasNextPage, nextCursor, priority, status]);

  useEffect(() => {
    const sentinelElement = sentinelRef.current;
    if (!sentinelElement) return;
    const observer = new IntersectionObserver(
      entries => {
        const entry = entries[0];
        if (entry.isIntersecting) {
          loadMore();
        }
      },
      { root: null, rootMargin: '200px', threshold: 0.1 }
    );
    observer.observe(sentinelElement);
    return () => observer.disconnect();
  }, [loadMore]);

  if (!token) return <div style={{ textAlign: 'center' }}>Please log in to view tasks.</div>;

  return (
    <div style={{ maxWidth: 800, margin: '12px auto', background: '#fff', borderRadius: 12, boxShadow: '0 2px 12px #0001', padding: '8px 24px', }}>
      <h2 style={{ textAlign: 'center', marginBottom: 24, color: '#2d3748', }}>Tasks</h2>
      <TaskStatusSummary />
      <div style={{ display: 'flex', alignItems: 'center', gap: 24, marginBottom: 24, justifyContent: 'center', flexWrap: 'wrap', }}>
        <div>
          <label style={{ fontWeight: 500, marginRight: 8 }}>Priority:</label>
          <select value={priority} onChange={e => setPriority(e.target.value as Priority | '')} style={{ ...selectStyle, minWidth: 120, }}>
            <option value=''>All</option>
            <option value={Priority.Low}>Low</option>
            <option value={Priority.Medium}>Medium</option>
            <option value={Priority.High}>High</option>
          </select>
        </div>
        <div>
          <label style={{ fontWeight: 500, marginRight: 8 }}>Status:</label>
          <select value={status} onChange={e => setStatus(e.target.value as Status | '')} style={{ ...selectStyle, minWidth: 160, }}>
            <option value=''>All</option>
            <option value={Status.Pending}>Pending</option>
            <option value={Status.InProgress}>In Progress</option>
            <option value={Status.Completed}>Completed</option>
            <option value={Status.Archived}>Archived</option>
          </select>
        </div>
      </div>
      {loading && tasks.length === 0 ? (
        <div style={{ textAlign: 'center', color: '#888', margin: '32px 0' }}>Loading...</div>
      ) : (
        <ul style={{ listStyle: 'none', margin: 0, padding: 0, }}>
          {tasks.length === 0 && (
            <li style={{ textAlign: 'center', color: '#888', margin: '32px 0' }}>No tasks found.</li>
          )}
          {tasks.map(task => (
            <li key={task.id} style={{
              background: '#f7fafc',
              borderRadius: 8,
              marginBottom: 16,
              boxShadow: '0 1px 4px #0001',
              display: 'flex',
              alignItems: 'center',
              justifyContent: 'space-between',
              padding: 16,
              gap: 16,
            }}>
              <div style={{ flex: 1, minWidth: 0 }}>
                <Link to={`/tasks/${task.id}`} style={{ fontWeight: 600, color: '#2b6cb0', textDecoration: 'none', fontSize: 18 }}>
                  {task.title}
                </Link>
                <div style={{ fontSize: 14, color: '#555', marginTop: 4, }}>
                  <span style={{
                    background: '#e2e8f0',
                    borderRadius: 4,
                    marginRight: 8,
                    fontWeight: 500,
                    padding: '2px 6px',
                    fontSize: 13,
                  }}>{task.priority}</span>
                  <span style={{
                    background: '#e2e8f0',
                    borderRadius: 4,
                    fontWeight: 500,
                    padding: '2px 6px',
                    fontSize: 13,
                  }}>{task.status}</span>
                </div>
              </div>
              <Link to={`/tasks/${task.id}`} style={{
                color: '#3182ce',
                fontWeight: 500,
                fontSize: 14,
                textDecoration: 'underline',
                alignSelf: 'center',
              }}>View</Link>
            </li>
          ))}
        </ul>
      )}
      <div style={{ textAlign: 'center', marginTop: 32, marginBottom: 16 }}>
        <Link to='/tasks/create' style={{
          display: 'inline-block',
          background: '#3182ce',
          color: '#fff',
          borderRadius: 6,
          textDecoration: 'none',
          fontWeight: 600,
          fontSize: 16,
          boxShadow: '0 1px 4px #0001',
          padding: '10px 18px'
        }}>
          + Create New Task
        </Link>
      </div>

      <div ref={sentinelRef} style={{ height: 1 }} />
      {loading && tasks.length > 0 && (
        <div style={{ textAlign: 'center', color: '#888', marginTop: 12 }}>Loading more...</div>
      )}
    </div>
  );
}

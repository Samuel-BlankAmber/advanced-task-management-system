import React, { useState, useContext, useRef, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { createTask, Priority, Status } from './api';
import { TokenContext } from '../../App';
import { selectStyle, inputStyle, textareaStyle } from '../../styles/ui';

export default function CreateTaskForm() {
  const token = useContext(TokenContext);
  const navigate = useNavigate();
  const [showConfirm, setShowConfirm] = useState(false);
  const [form, setForm] = useState({
    title: '',
    description: '',
    priority: Priority.Low as typeof Priority[keyof typeof Priority],
    dueDate: '',
    status: Status.Pending as typeof Status[keyof typeof Status],
  });
  const [error, setError] = useState('');
  const confirmButtonRef = useRef<HTMLButtonElement | null>(null);
  const createButtonRef = useRef<HTMLButtonElement | null>(null);

  function handleChange(e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement | HTMLTextAreaElement>) {
    setForm(f => ({ ...f, [e.target.name]: e.target.value }));
  }

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault();
    if (!token) return;
    if (form.priority === Priority.High) {
      setShowConfirm(true);
      return;
    }
    await doCreate();
  }

  useEffect(() => {
    if (showConfirm) {
      setTimeout(() => confirmButtonRef.current?.focus(), 0);
    } else {
      createButtonRef.current?.focus();
    }
  }, [showConfirm]);

  async function doCreate() {
    if (!token) return;
    try {
      // Ensure dueDate is sent as an ISO datetime at midnight UTC.
      const payload = { ...form } as Omit<import('./api').TaskItem, 'id'>;
      if (form.dueDate) {
        const [y, m, d] = form.dueDate.split('-').map(s => Number(s));
        const dt = new Date(Date.UTC(y, (m || 1) - 1, d || 1, 0, 0, 0));
        payload.dueDate = dt.toISOString();
      }
      await createTask(token, payload);
      navigate('/tasks');
    } catch (err) {
      setError('Failed to create task');
    }
  }

  if (!token) return <div>Please log in to create a task.</div>;

  return (
    <form onSubmit={handleSubmit} style={{ padding: 24, background: '#fff', borderRadius: 8, display: 'flex', flexDirection: 'column', gap: 12, }}>
      <h3 style={{ marginTop: 0 }}>Create Task</h3>
      {error && <div style={{ color: 'red' }}>{error}</div>}

      <label style={{ fontSize: 14, fontWeight: 600, textAlign: 'left' }}>Title</label>
      <input name='title' value={form.title} onChange={handleChange} placeholder='Title' required minLength={3} maxLength={200} style={{ ...inputStyle }} />

      <label style={{ fontSize: 14, fontWeight: 600, textAlign: 'left' }}>Description</label>
      <textarea name='description' value={form.description} onChange={handleChange} placeholder='Description' maxLength={1000} style={{ ...textareaStyle }} />

      <div style={{ display: 'flex', gap: 12, alignItems: 'flex-end', flexWrap: 'wrap', }}>
        <div style={{ display: 'flex', flexDirection: 'column', minWidth: 140, }}>
          <label style={{ fontSize: 13, fontWeight: 600, marginBottom: 6 }}>Priority</label>
          <select name='priority' value={form.priority} onChange={handleChange} required style={{ ...selectStyle }}>
            <option value={Priority.Low}>Low</option>
            <option value={Priority.Medium}>Medium</option>
            <option value={Priority.High}>High</option>
          </select>
        </div>

        <div style={{ display: 'flex', flexDirection: 'column', minWidth: 180, }}>
          <label style={{ fontSize: 13, fontWeight: 600, marginBottom: 6 }}>Status</label>
          <select name='status' value={form.status} onChange={handleChange} style={{ ...selectStyle }}>
            <option value={Status.Pending}>Pending</option>
            <option value={Status.InProgress}>In Progress</option>
            <option value={Status.Completed}>Completed</option>
            <option value={Status.Archived}>Archived</option>
          </select>
        </div>

        <div style={{ display: 'flex', flexDirection: 'column', }}>
          <label style={{ fontSize: 13, fontWeight: 600, marginBottom: 6 }}>Due Date</label>
          <input name='dueDate' type='date' value={form.dueDate} onChange={handleChange} required style={{ ...inputStyle }} />
        </div>
      </div>

      <div style={{ display: 'flex', gap: 12, marginTop: 8, }}>
        <button ref={createButtonRef} type='submit' style={{ padding: '10px 14px', borderRadius: 6, background: '#3182ce', color: '#fff', border: 'none', fontWeight: 600 }}>Create</button>
        <button type='button' onClick={() => (window.history.back())} style={{ padding: '10px 14px', borderRadius: 6 }}>Cancel</button>
      </div>
      {showConfirm && (
        <div role='dialog' aria-modal='true' style={{ position: 'fixed', inset: 0, display: 'flex', alignItems: 'center', justifyContent: 'center', background: 'rgba(0,0,0,0.4)' }}>
          <div style={{ background: '#fff', padding: 20, borderRadius: 8, width: 420, maxWidth: '90%' }}>
            <h4 style={{ marginTop: 0 }}>Confirm High Priority</h4>
            <p>You've selected <b>High</b> priority. Are you sure you want to mark this task as high priority?</p>
            <div style={{ display: 'flex', gap: 12, justifyContent: 'flex-end', marginTop: 12 }}>
              <button onClick={() => { setShowConfirm(false); }} style={{ padding: '8px 12px', borderRadius: 6 }}>Cancel</button>
              <button ref={confirmButtonRef} onClick={async () => { setShowConfirm(false); await doCreate(); }} style={{ padding: '8px 12px', borderRadius: 6, background: '#2ea44f', color: '#fff', border: 'none' }}>Confirm</button>
            </div>
          </div>
        </div>
      )}
    </form>
  );
}

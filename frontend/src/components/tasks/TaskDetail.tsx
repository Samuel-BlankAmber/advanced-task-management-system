import React, { useEffect, useState, useContext } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { fetchTask, updateTask, deleteTask, Priority, Status } from './api';
import { selectStyle, inputStyle, textareaStyle } from '../../styles/ui';
import type { TaskItem } from './api';
import { TokenContext } from '../../App';

export default function TaskDetail() {
  const token = useContext(TokenContext);
  const { id } = useParams();
  const navigate = useNavigate();
  const [task, setTask] = useState<TaskItem | null>(null);
  const [edit, setEdit] = useState(false);
  type FormState = {
    title: string;
    description: string;
    priority: Priority;
    dueDate: string;
    status: Status;
  };

  const [form, setForm] = useState<FormState>({ title: '', description: '', priority: Priority.Low, dueDate: '', status: Status.Pending, });

  useEffect(() => {
    if (id && token) fetchTask(token, id).then(t => { setTask(t); setForm({ ...t, dueDate: t.dueDate.split('T')[0] }); });
  }, [id, token]);

  function handleChange(e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement | HTMLTextAreaElement>) {
    const name = e.target.name as keyof FormState;
    const value = e.target.value;
    setForm(prev => ({
      ...prev,
      [name]: (name === 'priority' ? (value as Priority) : name === 'status' ? (value as Status) : value) as FormState[typeof name]
    }));
  }

  async function handleUpdate() {
    if (!id || !token) return;
    // Ensure dueDate is an ISO datetime at midnight UTC.
    const payload: Partial<Omit<TaskItem, 'id'>> = { ...form };
    if (form.dueDate) {
      const [y, m, d] = form.dueDate.split('-').map(s => Number(s));
      const dt = new Date(Date.UTC(y, (m || 1) - 1, d || 1, 0, 0, 0));
      payload.dueDate = dt.toISOString();
    }
    await updateTask(token, id, payload);
    setEdit(false);
    fetchTask(token, id).then(setTask);
  }

  async function handleDelete() {
    if (!id || !token) return;
    await deleteTask(token, id);
    navigate('/tasks');
  }

  if (!token) return <div>Please log in to view task details.</div>;
  if (!task) return <div>Loading...</div>;

  return (
    <div style={{ padding: 24, background: '#fff', borderRadius: 8, }}>
      <h3 style={{ marginTop: 0 }}>Task Details</h3>
      {edit ? (
        <div style={{ display: 'flex', flexDirection: 'column', gap: 12, }}>
          <label style={{ fontSize: 14, fontWeight: 600 }}>Title</label>
          <input name='title' value={form.title} onChange={handleChange} placeholder='Title' style={{ ...inputStyle }} />
          <label style={{ fontSize: 14, fontWeight: 600 }}>Description</label>
          <textarea name='description' value={form.description} onChange={handleChange} placeholder='Description' style={{ ...textareaStyle }} />
          <div style={{ display: 'flex', gap: 12, flexWrap: 'wrap', alignItems: 'flex-end', }}>
            <div style={{ display: 'flex', flexDirection: 'column', minWidth: 140, }}>
              <label style={{ fontSize: 13, fontWeight: 600, marginBottom: 6 }}>Priority</label>
              <select name='priority' value={form.priority} onChange={handleChange} style={{ ...selectStyle }}>
                <option value='Low'>Low</option>
                <option value='Medium'>Medium</option>
                <option value='High'>High</option>
              </select>
            </div>
            <div style={{ display: 'flex', flexDirection: 'column', minWidth: 180, }}>
              <label style={{ fontSize: 13, fontWeight: 600, marginBottom: 6 }}>Status</label>
              <select name='status' value={form.status} onChange={handleChange} style={{ ...selectStyle }}>
                <option value='Pending'>Pending</option>
                <option value='InProgress'>In Progress</option>
                <option value='Completed'>Completed</option>
                <option value='Archived'>Archived</option>
              </select>
            </div>
            <div style={{ display: 'flex', flexDirection: 'column', }}>
              <label style={{ fontSize: 13, fontWeight: 600, marginBottom: 6 }}>Due Date</label>
              <input name='dueDate' type='date' value={form.dueDate} onChange={handleChange} style={{ ...inputStyle }} />
            </div>
          </div>
          <div style={{ display: 'flex', gap: 12, }}>
            <button onClick={handleUpdate} style={{ padding: '8px 12px', borderRadius: 6, background: '#2ea44f', color: '#fff', border: 'none' }}>Save</button>
            <button onClick={() => setEdit(false)} style={{ padding: '8px 12px', borderRadius: 6 }}>Cancel</button>
          </div>
        </div>
      ) : (
        <div style={{ display: 'flex', flexDirection: 'column', gap: 8 }}>
          <div><b>Title:</b> {task.title}</div>
          <div><b>Description:</b> {task.description}</div>
          <div><b>Priority:</b> {task.priority}</div>
          <div><b>Due Date:</b> {task.dueDate.split('T')[0]}</div>
          <div><b>Status:</b> {task.status}</div>
          <div style={{ display: 'flex', gap: 12, marginTop: 12 }}>
            <button onClick={() => setEdit(true)} style={{ padding: '8px 12px', borderRadius: 6 }}>Edit</button>
            <button onClick={handleDelete} style={{ padding: '8px 12px', borderRadius: 6, background: '#d73a49', color: '#fff', border: 'none' }}>Delete</button>
          </div>
        </div>
      )}
      <div style={{ marginTop: 16 }}>
        <button onClick={() => navigate('/tasks')} style={{ padding: '8px 12px', borderRadius: 6 }}>Back to List</button>
      </div>
    </div>
  );
}

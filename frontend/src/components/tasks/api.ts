export const Priority = {
  Low: 'Low',
  Medium: 'Medium',
  High: 'High',
} as const;
export type Priority = typeof Priority[keyof typeof Priority];

export const Status = {
  Pending: 'Pending',
  InProgress: 'InProgress',
  Completed: 'Completed',
  Archived: 'Archived',
} as const;
export type Status = typeof Status[keyof typeof Status];

export type TaskItem = {
  id: string;
  title: string;
  description: string;
  priority: Priority;
  dueDate: string;
  status: Status;
}

export type CursorPaginatedResult<T> = {
  items: T[];
  pageSize: number;
  hasNextPage: boolean;
  nextCursor: string | null;
};

const API_BASE = import.meta.env.VITE_API_URL + '/tasks';

export async function fetchTasks(
  token: string,
  priority?: Priority | '',
  status?: Status | '',
  cursor: string | null = null,
  pageSize: number = 10,
): Promise<CursorPaginatedResult<TaskItem>> {
  const params = new URLSearchParams();
  if (priority) params.append('priority', priority);
  if (status) params.append('status', status);
  if (cursor) params.append('cursor', cursor);
  if (pageSize) params.append('pageSize', String(pageSize));

  const res = await fetch(`${API_BASE}?${params}`, {
    headers: {
      Authorization: `Bearer ${token}`,
    },
  });
  if (!res.ok) throw new Error('Failed to fetch tasks');

  const data = await res.json();
  return {
    items: data.items || [],
    pageSize: data.pageSize ?? pageSize,
    hasNextPage: data.hasNextPage ?? false,
    nextCursor: data.nextCursor ?? null,
  } as CursorPaginatedResult<TaskItem>;
}

export async function fetchTask(token: string, id: string): Promise<TaskItem> {
  const res = await fetch(`${API_BASE}/${id}`, {
    headers: {
      Authorization: `Bearer ${token}`,
    },
  });
  if (!res.ok) throw new Error('Failed to fetch task');
  return await res.json();
}

export async function createTask(token: string, task: Omit<TaskItem, 'id'>): Promise<TaskItem> {
  const res = await fetch(API_BASE, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
      Authorization: `Bearer ${token}`,
    },
    body: JSON.stringify(task),
  });
  if (!res.ok) throw new Error('Failed to create task');
  return await res.json();
}

export async function updateTask(token: string, id: string, task: Partial<Omit<TaskItem, 'id'>>): Promise<TaskItem> {
  const res = await fetch(`${API_BASE}/${id}`, {
    method: 'PUT',
    headers: {
      'Content-Type': 'application/json',
      Authorization: `Bearer ${token}`,
    },
    body: JSON.stringify(task),
  });
  if (!res.ok) throw new Error('Failed to update task');
  return await res.json();
}

export async function deleteTask(token: string, id: string): Promise<void> {
  const res = await fetch(`${API_BASE}/${id}`, {
    method: 'DELETE',
    headers: {
      Authorization: `Bearer ${token}`,
    },
  });
  if (!res.ok) throw new Error('Failed to delete task');
}

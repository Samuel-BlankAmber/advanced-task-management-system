import type { CSSProperties } from 'react';

export const selectStyle: CSSProperties = {
  padding: '8px 36px 8px 12px',
  borderRadius: 6,
  border: '1px solid #cbd5e0',
  backgroundColor: '#fff',
  boxShadow: 'inset 0 1px 2px rgba(16,24,40,0.03)',
  // Hide native arrow and use custom SVG chevron as background image.
  appearance: 'none',
  WebkitAppearance: 'none',
  MozAppearance: 'none',
  backgroundImage: `url("data:image/svg+xml;utf8,<svg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 24 24' fill='none' stroke='%23666' stroke-width='2' stroke-linecap='round' stroke-linejoin='round'><polyline points='6 9 12 15 18 9'/></svg>")`,
  backgroundRepeat: 'no-repeat',
  backgroundPosition: 'right 10px center',
  fontSize: 14,
  color: '#111827',
};

export const inputStyle: CSSProperties = {
  padding: '8px 12px',
  borderRadius: 6,
  border: '1px solid #cbd5e0',
  backgroundColor: '#fff',
  boxShadow: 'inset 0 1px 2px rgba(16,24,40,0.02)',
  fontSize: 14,
  color: '#111827',
  outline: 'none',
};

export const textareaStyle: CSSProperties = {
  padding: '8px 12px',
  borderRadius: 6,
  border: '1px solid #cbd5e0',
  backgroundColor: '#fff',
  boxShadow: 'inset 0 1px 2px rgba(16,24,40,0.02)',
  fontSize: 14,
  color: '#111827',
  minHeight: 100,
  resize: 'vertical',
};

export default {};

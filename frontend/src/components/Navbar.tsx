import React from 'react';

interface NavbarProps {
  isAuthenticated: boolean;
  user: { name?: string } | undefined;
  loginWithRedirect: () => void;
  logout: () => void;
}

const Navbar: React.FC<NavbarProps> = ({ isAuthenticated, user, loginWithRedirect, logout }) => (
  <nav style={{
    width: '100%',
    display: 'flex',
    alignItems: 'center',
    justifyContent: 'space-between',
    background: '#24292f',
    color: '#fff',
    padding: '0.75rem 2rem',
    boxSizing: 'border-box',
    boxShadow: '0 2px 8px rgba(0,0,0,0.04)'
  }}>
    <div style={{ fontWeight: 700, fontSize: '1.25rem', letterSpacing: '0.5px' }}>
      Advanced Task Management
    </div>
    <div style={{ display: 'flex', alignItems: 'center', gap: '1rem' }}>
      {isAuthenticated ? (
        <>
          <span style={{ fontWeight: 500, fontSize: '1rem' }}>Logged in as <span style={{ color: '#79b8ff' }}>{user?.name}</span></span>
          <button
            onClick={logout}
            style={{
              background: '#d73a49',
              color: '#fff',
              border: 'none',
              borderRadius: '4px',
              padding: '0.5rem 1rem',
              cursor: 'pointer',
              fontWeight: 500,
              transition: 'background 0.2s',
            }}
            onMouseOver={e => (e.currentTarget.style.background = '#b31d28')}
            onMouseOut={e => (e.currentTarget.style.background = '#d73a49')}
          >
            Log Out
          </button>
        </>
      ) : (
        <button
          onClick={loginWithRedirect}
          style={{
            background: '#2ea44f',
            color: '#fff',
            border: 'none',
            borderRadius: '4px',
            padding: '0.5rem 1rem',
            cursor: 'pointer',
            fontWeight: 500,
            transition: 'background 0.2s',
          }}
          onMouseOver={e => (e.currentTarget.style.background = '#22863a')}
          onMouseOut={e => (e.currentTarget.style.background = '#2ea44f')}
        >
          Log In
        </button>
      )}
    </div>
  </nav>
);

export default Navbar;

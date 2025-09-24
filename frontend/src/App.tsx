
import { useAuth0 } from "@auth0/auth0-react";
import Navbar from "./components/Navbar";

function App() {
  const { loginWithRedirect, logout, isAuthenticated, user } = useAuth0();

  return (
    <div style={{ fontFamily: 'Inter, Arial, sans-serif', background: '#f6f8fa', minHeight: '100vh', width: '100vw', margin: 0, padding: 0 }}>
      <Navbar
        isAuthenticated={isAuthenticated}
        user={user}
        loginWithRedirect={loginWithRedirect}
        logout={logout}
      />
      <div style={{ display: 'flex', justifyContent: 'center', alignItems: 'flex-start', width: '100%', minHeight: 'calc(100vh - 64px)', boxSizing: 'border-box', padding: '2.5rem 0' }}>
        <main style={{ width: '100%', maxWidth: 600, background: '#fff', borderRadius: 12, boxShadow: '0 2px 16px rgba(36,41,47,0.07)', padding: '2rem', boxSizing: 'border-box' }}>
          <h2 style={{ fontWeight: 600, fontSize: '1.5rem', marginBottom: '1rem', color: '#24292f' }}>Advanced Task Management System</h2>
          <p style={{ fontSize: '1.1rem', color: '#586069', marginBottom: '2rem' }}>
            Manage your tasks efficiently.
          </p>
        </main>
      </div>
    </div>
  );
}

export default App;

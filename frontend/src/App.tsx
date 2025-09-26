

import { useAuth0 } from '@auth0/auth0-react';
import Navbar from './components/Navbar';
import { BrowserRouter, Routes, Route } from 'react-router-dom';
import TaskList from './components/tasks/TaskList';
import TaskDetail from './components/tasks/TaskDetail';
import CreateTaskForm from './components/tasks/CreateTaskForm';
import { createContext, useEffect, useState } from 'react';

export const TokenContext = createContext<string | null>(null);

function App() {
  const { loginWithRedirect, logout, isAuthenticated, user, getAccessTokenSilently } = useAuth0();
  const [token, setToken] = useState<string | null>(null);

  useEffect(() => {
    if (isAuthenticated) {
      getAccessTokenSilently().then(setToken);
    } else {
      setToken(null);
    }
  }, [isAuthenticated, getAccessTokenSilently]);

  return (
    <BrowserRouter>
      <TokenContext.Provider value={token}>
  <div style={{ fontFamily: 'Inter, Arial, sans-serif', background: '#f6f8fa', minHeight: '100vh', width: '100%', }}>
          <Navbar
            isAuthenticated={isAuthenticated}
            user={user}
            loginWithRedirect={loginWithRedirect}
            logout={logout}
          />
          <div style={{ display: 'flex', justifyContent: 'center', alignItems: 'flex-start', minHeight: 'calc(100vh - 64px)', boxSizing: 'border-box', padding: '12px 24px', }}>
            <main style={{ width: '100%', maxWidth: 1000, margin: '0 auto', background: '#fff', borderRadius: 12, boxShadow: '0 2px 16px rgba(36,41,47,0.07)', boxSizing: 'border-box', padding: 24, }}>
              <Routes>
                <Route path='/' element={<TaskList />} />
                <Route path='/tasks' element={<TaskList />} />
                <Route path='/tasks/create' element={<CreateTaskForm />} />
                <Route path='/tasks/:id' element={<TaskDetail />} />
              </Routes>
            </main>
          </div>
        </div>
      </TokenContext.Provider>
    </BrowserRouter>
  );
}

export default App;

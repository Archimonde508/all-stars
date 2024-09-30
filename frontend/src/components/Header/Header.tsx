import { useState } from 'react';
import styled from 'styled-components';
import { login } from '../../api/api';
import AddGameModal from '../AddGameModal/AddGameModal';

export const Header = () => {
  const [isLoginModalOpen, setLoginModalOpen] = useState(false);
  const [isAddGameModalOpen, setAddGameModalOpen] = useState(false);
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const [isLoggedIn, setIsLoggedIn] = useState(true);

  const openModal = () => {
    setAddGameModalOpen(true);
  };

  const handleLogin = async () => {
    const token = await login(username, password);
    if (token) {
      setLoginModalOpen(false);
      setIsLoggedIn(true);
    } else {
      setError("Nie udało się zalogować.");
    }
  };

  const onLoginLogout = () => {
    setIsLoggedIn(false);
    // remove token
  };

  return (
    <HeaderContainer>
      <AppName>
        <span>Aplikacja Gwiazd</span>
        {isLoggedIn && (
          <GreenButton onClick={openModal}>
            Dodaj Grę
          </GreenButton>
        )}
        {isAddGameModalOpen && (
          <AddGameModal closeModal={() => setAddGameModalOpen(false)} />
        )}
      </AppName>
      <div>
        {isLoggedIn ? (
          <AuthButton onClick={onLoginLogout}>Logout</AuthButton>
        ) : (
          <AuthButton onClick={() => setLoginModalOpen(true)}>Login</AuthButton>
        )}
      </div>
      {isLoginModalOpen && (
        <ModalOverlay>
          <ModalContent>
            <h2>Login</h2>
            <ModalInput
              type="text"
              placeholder="Username"
              value={username}
              onChange={(e) => setUsername(e.target.value)}
            />
            <ModalInput
              type="password"
              placeholder="Password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
            />
            {error && <p>{error}</p>}
            <ModalButton onClick={handleLogin}>Login</ModalButton>
            <ModalButton onClick={() => setLoginModalOpen(false)}>Close</ModalButton>
          </ModalContent>
        </ModalOverlay>
      )}
    </HeaderContainer>
  );
};
const HeaderContainer = styled.header`
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 0.9375rem 1.875rem;
  background-color: #1e1e1e;
  color: #f5f5f5;
  box-shadow: 0 0.125rem 0.25rem rgba(0, 0, 0, 0.2);
`;

const AppName = styled.div`
  font-size: 1.375rem;
  font-weight: bold;
  color: #ffd700;
  display: flex;
  gap: 1.5625rem;
`;

const AuthButton = styled.button`
  background-color: #3a3a3a;
  color: #f5f5f5;
  padding: 0.625rem 1.25rem;
  border: none;
  border-radius: 0.3125rem;
  cursor: pointer;
  font-weight: 500;
  transition: background-color 0.3s ease;

  &:hover {
    background-color: #575757;
  }
`;

const GreenButton = styled(AuthButton)`
  background-color: #085519;

  &:hover {
    background-color: #0e8327;
  }
`;

const ModalOverlay = styled.div`
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background: rgba(0, 0, 0, 0.8);
  display: flex;
  justify-content: center;
  align-items: center;
`;

const ModalContent = styled.div`
  background: #2d3748;
  color: #e2e8f0;
  padding: 2rem;
  border-radius: 0.5rem;
  width: 18.75rem;
  text-align: center;
`;

const ModalInput = styled.input`
  display: block;
  width: 100%;
  padding: 0.5rem;
  margin-bottom: 1rem;
  border: 1px solid #4a5568;
  border-radius: 0.25rem;
  background: #1a202c;
  color: #e2e8f0;
`;

const ModalButton = styled.button`
  background-color: #2c3036;
  color: #e2e8f0;
  border: none;
  padding: 0.5rem 1rem;
  margin: 0.5rem;
  cursor: pointer;
  border-radius: 0.25rem;

  &:hover {
    background-color: #2d3748;
  }
`;


export default Header;

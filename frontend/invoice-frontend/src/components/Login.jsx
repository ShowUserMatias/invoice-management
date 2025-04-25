import React, { useState } from 'react';
import api from '../services/api';
import { useNavigate } from 'react-router-dom';

const Login = ({ onLoginSuccess }) => {
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const navigate = useNavigate();

  const handleLogin = async () => {
    try {
      const response = await api.post('Auth/login', {
        username,
        password,
      });
      const token = response.data.token;
      localStorage.setItem('token', response.data.token); 
      setError('');
      if (onLoginSuccess) onLoginSuccess();
      navigate('/')
    } catch (err) {
      setError('Credenciales incorrectas. Intente nuevamente.');
    }
  };

  return (
    <div className="max-w-md mx-auto mt-10 bg-white p-8 rounded-2xl shadow-md">
      <h2 className="text-xl font-semibold mb-6 text-center">Inicio de Sesión</h2>
      <div className="mb-4">
        <label className="block text-sm font-medium text-gray-700 mb-1">Usuario:</label>
        <input
          type="text"
          className="w-full border border-gray-300 rounded-lg px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-500"
          value={username}
          onChange={(e) => setUsername(e.target.value)}
        />
      </div>
      <div className="mb-6">
        <label className="block text-sm font-medium text-gray-700 mb-1">Contraseña:</label>
        <input
          type="password"
          className="w-full border border-gray-300 rounded-lg px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-500"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
        />
      </div>
      {error && <p className="text-red-500 text-sm mb-4">{error}</p>}
      <button
        onClick={handleLogin}
        className="w-full bg-blue-600 text-white py-2 px-4 rounded-lg hover:bg-blue-700 transition-colors"
      >
        Iniciar Sesión
      </button>
    </div>
  );
};

export default Login;

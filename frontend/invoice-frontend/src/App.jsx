import React  from 'react';
import SearchInvoice from './components/SearchInvoice';
import InvoiceDatails from './components/InvoiceDetails';
import { Routes, Route } from 'react-router-dom';
import Login from './components/Login';
import { isAuthenticated, logout } from './services/auth';
import { useNavigate } from 'react-router-dom';

const App = () => {
    const navigate = useNavigate();
  
    const handleLogout = () => {
      logout();
      navigate('/login');
    };

  return (
    <div className="App">
        <header className="flex justify-between items-center p-4 bg-gray-800 text-white">
            <h1 className="text-xl">Gestión de Facturas</h1>
      {!isAuthenticated() && (
        <button
        onClick={handleLogout}
            className="bg-red-500 py-1 px-3 rounded hover:bg-red-600 transition-colors"
          >
            Cerrar Sesión
          </button>
      )}
      </header>

      <main className="p-6">      
      <Routes>
        <Route path="/login" element={<Login />} />
        {isAuthenticated() ? (
            <>
        <Route path="/" element={<SearchInvoice />} />
        <Route path='/factura/:invoiceId' element={<InvoiceDatails />} />
        </>
        ) : (
      <Route path="*" element={<Login />} />  
      )} 
      </Routes>
      </main>
    </div>
  );
};

export default App;

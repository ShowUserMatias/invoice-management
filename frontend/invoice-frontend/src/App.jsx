import React from 'react';
import SearchInvoice from './components/SearchInvoice';
import InvoiceDatails from './components/InvoiceDetails';
import { Routes, Route } from 'react-router-dom';

function App() {
  return (
    <div className="min-h-screen bg-gray-100 p-4">
      <h1 className="text-3xl font-bold text-center mb-8 text-gray-800">Gestión de Facturas</h1>
      <Routes>
        <Route path="/" element={<SearchInvoice />} />
        <Route path='/factura/:invoiceId' element={<InvoiceDatails />} />
      </Routes>      
    </div>
  );
}

export default App;

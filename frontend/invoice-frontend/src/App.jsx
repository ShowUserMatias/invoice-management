import React from 'react';
import SearchInvoice from './components/SearchInvoice';
import InvoiceDatails from './components/InvoiceDetails';
import { Routes, Route } from 'react-router-dom';

function App() {
  return (
    <div className="App">
      <h1>Gestión de Facturas</h1>
      <Routes>
        <Route path="/" element={<SearchInvoice />} />
        <Route path='/factura/:invoiceId' element={<InvoiceDatails />} />
      </Routes>      
    </div>
  );
}

export default App;

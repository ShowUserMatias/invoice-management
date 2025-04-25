import React, { useState } from 'react';
import axios from 'axios';
import { Link } from 'react-router-dom';

const SearchInvoice = () => {
  const [invoiceNumber, setInvoiceNumber] = useState('');
  const [invoiceStatus, setInvoiceStatus] = useState('');
  const [paymentStatus, setPaymentStatus] = useState('');
  const [results, setResults] = useState([]);
  const [error, setError] = useState('');
  const [validationError, setValidationError] = useState('');

  const handleSearch = async () => {
    if (!invoiceNumber && !invoiceStatus && !paymentStatus) {
        setValidationError('Debe ingresar al menos un filtro para realizar la búsqueda.');
        return;
      }
      if (invoiceNumber && invoiceNumber <= 0) {
        setValidationError('El número de factura debe ser mayor a cero.');
        return;
      }
      setValidationError('');
    try {
      const params = {};
      if (invoiceNumber) params.invoiceNumber = invoiceNumber;
      if (invoiceStatus) params.invoiceStatus = invoiceStatus;
      if (paymentStatus) params.paymentStatus = paymentStatus;

      const response = await axios.get('http://localhost:5192/api/Factura/buscar', { params });
      setResults(response.data);
      setError('');
    } catch (err) {        
        setError('No se pudo conectar con el servidor o hubo un error en la búsqueda.');
      setResults([]);
    }
  };

  return (
    <div className="max-w-3xl mx-auto bg-white shadow-md rounded-lg p-8 space-y-6">
      <h2 className="text-2xl font-semibold text-gray-800 text-center">Buscar Facturas</h2>
      <div className="space-y-4">
        <label className="block text-gray-700 mb-1">Número de Factura:</label>
        <input
          type="number"
          value={invoiceNumber}
          onChange={(e) => setInvoiceNumber(e.target.value)}
          className="w-full border border-gray-300 rounded px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-400"
        />
      </div>
      <div>
        <label className="block text-gray-700 mb-1">Estado de Factura:</label>
        <input
          type="text"
          placeholder="Issued, Cancelled, Partial"
          value={invoiceStatus}
          onChange={(e) => setInvoiceStatus(e.target.value)}
          className="w-full border border-gray-300 rounded px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-400"
        />
      </div>
      <div>
        <label className="block text-gray-700 mb-1">Estado de Pago:</label>
        <input
          type="text"
          placeholder="Pending, Overdue, Paid"
          value={paymentStatus}
          onChange={(e) => setPaymentStatus(e.target.value)}
          className="w-full border border-gray-300 rounded px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-400"
        />
      </div>
      <button onClick={handleSearch}
      className="w-full bg-blue-600 text-white py-2 rounded hover:bg-blue-700 transition-colors"
      >Buscar</button>

      {validationError && <p className="text-yellow-500 text-sm">{validationError}</p>}
      {error && <p className="text-red-600 text-sm">{error}</p>}      

      <div className="pt-6">
        {results.length > 0 ? (
          <ul className="space-y-4">
            {results.map((invoice) => (
              <li key={invoice.invoiceId}
              className="border border-gray-200 rounded p-4 hover:shadow-md transition-shadow"
              >
                <p className="text-gray-800">
                    <span className="font-medium"> Factura #{invoice.invoiceNumber}</span> - Estado:{' '} 
                    <span className="text-blue-700"> {invoice.invoiceStatus}</span> - Pago:{' '} 
                    <span className="text-green-700"> {invoice.paymentStatus} {' '} </span>
                </p>
                <Link to={`/factura/${invoice.invoiceId}`}
                className="text-sm text-blue-600 hover:underline mt-2 inline-block"
                >Ver Detalle</Link>

              </li>
            ))}
          </ul>     
        ) : (
            <p className="text-gray-500 text-center">No se encontraron facturas.</p>   
        )}
      </div>
    </div>
  );
};

export default SearchInvoice;

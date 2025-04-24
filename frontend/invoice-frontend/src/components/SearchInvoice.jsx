import React, { useState } from 'react';
import axios from 'axios';
import { Link } from 'react-router-dom';

const SearchInvoice = () => {
  const [invoiceNumber, setInvoiceNumber] = useState('');
  const [invoiceStatus, setInvoiceStatus] = useState('');
  const [paymentStatus, setPaymentStatus] = useState('');
  const [results, setResults] = useState([]);
  const [error, setError] = useState('');

  const handleSearch = async () => {
    try {
      const params = {};
      if (invoiceNumber) params.invoiceNumber = invoiceNumber;
      if (invoiceStatus) params.invoiceStatus = invoiceStatus;
      if (paymentStatus) params.paymentStatus = paymentStatus;

      const response = await axios.get('http://localhost:5192/api/Factura/buscar', { params });
      setResults(response.data);
      setError('');
    } catch (err) {
        console.error(err);
        setError('No se pudo conectar con el servidor o hubo un error en la búsqueda.');
      setResults([]);
    }
  };

  return (
    <div>
      <h2>Buscar Facturas</h2>
      <div>
        <label>Número de Factura:</label>
        <input
          type="number"
          value={invoiceNumber}
          onChange={(e) => setInvoiceNumber(e.target.value)}
        />
      </div>
      <div>
        <label>Estado de Factura:</label>
        <input
          type="text"
          placeholder="Issued, Cancelled, Partial"
          value={invoiceStatus}
          onChange={(e) => setInvoiceStatus(e.target.value)}
        />
      </div>
      <div>
        <label>Estado de Pago:</label>
        <input
          type="text"
          placeholder="Pending, Overdue, Paid"
          value={paymentStatus}
          onChange={(e) => setPaymentStatus(e.target.value)}
        />
      </div>
      <button onClick={handleSearch}>Buscar</button>

      {error && <p style={{ color: 'red' }}>{error}</p>}

      {!error && results.length === 0 && <p>No se encontraron facturas.</p>}

      <div>
        {results.length > 0 && (
          <ul>
            {results.map((invoice) => (
              <li key={invoice.invoiceId}>
                Factura #{invoice.invoiceNumber} - Estado: {invoice.invoiceStatus} - Pago: {invoice.paymentStatus} {' '}
                <Link to={`/factura/${invoice.invoiceId}`}>Ver Detalle</Link>
              </li>
            ))}
          </ul>        
        )}
      </div>
    </div>
  );
};

export default SearchInvoice;

import React, { useEffect, useState, useCallback } from 'react';
import { useParams } from 'react-router-dom';
import api from '../services/api';
import AddCreditNote from './AddCreditNote';

const InvoiceDetails = () => {
  const { invoiceId } = useParams();
  console.log('ID recibido por useParams:', invoiceId);
  const [invoice, setInvoice] = useState(null);
  const [error, setError] = useState('');

  
    const fetchInvoiceDetail = useCallback(async () => {
      try {
        const response = await api.get(`http://localhost:5192/api/Factura/${invoiceId}`);        
          setInvoice(response.data);                  
          setError('');        
      } catch (err) {
        setError('Error al cargar los detalles de la factura.');
      }
    }, [invoiceId]);

    useEffect(() => {  
    fetchInvoiceDetail();
  }, [fetchInvoiceDetail]);

  if (error) return <p className="text-red-600 text-center mt-6">{error}</p>;
  if (!invoice) return <p className="text-gray-500 text-center mt-6">Cargando detalles de la factura...</p>;

  return (
    <div className="max-w-3xl mx-auto bg-white shadow-md rounded-lg p-8 space-y-6">
      <h2 className="text-2xl font-semibold text-gray-800 text-center">Detalle de Factura #{invoice.invoiceNumber}</h2>
      <div className="space-y-2">
      <p><strong>Fecha:</strong> {invoice.invoiceDate}</p>
      <p><strong>Monto Total:</strong> {invoice.totalAmount}</p>
      <p><strong>Estado de Factura:</strong> <span className="text-blue-700">{invoice.invoiceStatus}</span></p>
      <p><strong>Estado de Pago:</strong> <span className="text-green-700">{invoice.paymentStatus}</span></p>
      </div>
      <div>
      <h3 className="text-lg font-semibold text-gray-700 mb-2">Cliente</h3>
      <p>{invoice.customer.customerName} ({invoice.customer.customerRun}) - {invoice.customer.customerEmail}</p>
      </div>
      <div>
      <h3 className="text-lg font-semibold text-gray-700 mb-2">Productos</h3>
      <ul className="space-y-2">
        {invoice.invoiceDetails.map((detail, index) => (
          <li key={index} className="border border-gray-200 rounded p-3">
            {detail.productName} - Cantidad: {detail.quantity} - Precio Unitario: {detail.unitPrice} - Subtotal: {detail.subtotal}
          </li>
        ))}
      </ul>
      </div>
      <div>
      <h3 className="text-lg font-semibold text-gray-700 mb-2">Notas de Crédito</h3>
      {invoice.invoiceCreditNotes.length > 0 ? (
        <ul className="space-y-2">
          {invoice.invoiceCreditNotes.map((note, index) => (
            <li key={index} className="border border-gray-200 rounded p-3">
              NC #{note.creditNoteNumber} - {note.creditNoteAmount} (Fecha: {note.creditNoteDate})
            </li>
          ))}
        </ul>
      ) : (
        <p className="text-gray-500">No hay notas de crédito.</p>
      )}
    </div>
        <AddCreditNote invoiceId={invoice.invoiceId} onCreditNoteAdded={fetchInvoiceDetail} />
      <div>
      <h3 className="text-lg font-semibold text-gray-700 mb-2">Pago</h3>
      {invoice.invoicePayment.paymentDate ? (
        <p>Método: {invoice.invoicePayment.paymentMethod} - Fecha: {invoice.invoicePayment.paymentDate}</p>
      ) : (
        <p className="text-gray-500">Pago pendiente.</p>
      )}
      </div>
    </div>
  );
};

export default InvoiceDetails;

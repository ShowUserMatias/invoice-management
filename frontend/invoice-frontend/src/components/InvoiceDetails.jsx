import React, { useEffect, useState } from 'react';
import { useParams } from 'react-router-dom';
import axios from 'axios';

const InvoiceDetails = () => {
  const { id } = useParams();
  const [invoice, setInvoice] = useState(null);
  const [error, setError] = useState('');

  useEffect(() => {
    const fetchInvoice = async () => {
      try {
        const response = await axios.get('http://localhost:5192/api/Factura/buscar', {
            params: { invoiceNumber: id }});
        if (response.data.length > 0) {
          setInvoice(response.data[0]);          
        } else {
          setError('Factura no encontrada.');
        }
      } catch (err) {
        setError('Error al cargar los detalles de la factura.');
      }
    };

    fetchInvoice();
  }, [id]);

  if (error) return <p style={{ color: 'red' }}>{error}</p>;
  if (!invoice) return <p>Cargando detalles de la factura...</p>;

  return (
    <div>
      <h2>Detalle de Factura #{invoice.invoiceNumber}</h2>
      <p><strong>Fecha:</strong> {invoice.invoiceDate}</p>
      <p><strong>Monto Total:</strong> {invoice.totalAmount}</p>
      <p><strong>Estado de Factura:</strong> {invoice.invoiceStatus}</p>
      <p><strong>Estado de Pago:</strong> {invoice.paymentStatus}</p>

      <h3>Cliente</h3>
      <p>{invoice.customer.customerName} ({invoice.customer.customerRun}) - {invoice.customer.customerEmail}</p>

      <h3>Productos</h3>
      <ul>
        {invoice.invoiceDetails.map((detail, index) => (
          <li key={index}>
            {detail.productName} - Cantidad: {detail.quantity} - Precio Unitario: {detail.unitPrice} - Subtotal: {detail.subtotal}
          </li>
        ))}
      </ul>

      <h3>Notas de Crédito</h3>
      {invoice.invoiceCreditNotes.length > 0 ? (
        <ul>
          {invoice.invoiceCreditNotes.map((note, index) => (
            <li key={index}>
              NC #{note.creditNoteNumber} - {note.creditNoteAmount} (Fecha: {note.creditNoteDate})
            </li>
          ))}
        </ul>
      ) : (
        <p>No hay notas de crédito.</p>
      )}

      <h3>Pago</h3>
      {invoice.invoicePayment.paymentDate ? (
        <p>Método: {invoice.invoicePayment.paymentMethod} - Fecha: {invoice.invoicePayment.paymentDate}</p>
      ) : (
        <p>Pago pendiente.</p>
      )}
    </div>
  );
};

export default InvoiceDetails;

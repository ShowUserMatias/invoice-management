import React, { useState } from 'react';
import axios from 'axios';

const AddCreditNote = ({ invoiceId, onCreditNoteAdded }) => {
  const [creditNoteNumber, setCreditNoteNumber] = useState('');
  const [creditNoteAmount, setCreditNoteAmount] = useState('');
  const [message, setMessage] = useState('');

  const handleAddCreditNote = async () => {
    try {
      const response = await axios.post('http://localhost:5192/api/Factura/agregar-nc', {
        invoiceId: invoiceId,
        creditNoteNumber: parseInt(creditNoteNumber),
        creditNoteAmount: parseFloat(creditNoteAmount)
      });
      setMessage(response.data.message);
      setCreditNoteNumber('');
      setCreditNoteAmount('');
      if (onCreditNoteAdded) onCreditNoteAdded(); 
    } catch (error) {
        const backendMessage = error.response?.data?.message || 'Error al agregar la nota de crédito.';
        setMessage(backendMessage);
    }
  };

  return (
    <div>
      <h3>Agregar Nota de Crédito</h3>
      <div>
        <label>Número de Nota de Crédito:</label>
        <input
          type="number"
          value={creditNoteNumber}
          onChange={(e) => setCreditNoteNumber(e.target.value)}
        />
      </div>
      <div>
        <label>Monto de Nota de Crédito:</label>
        <input
          type="number"
          value={creditNoteAmount}
          onChange={(e) => setCreditNoteAmount(e.target.value)}
        />
      </div>
      <button onClick={handleAddCreditNote}>Agregar Nota de Crédito</button>
      {message && <p>{message}</p>}
    </div>
  );
};

export default AddCreditNote;

import React, { useEffect, useState } from 'react';
import axios from 'axios';

const AddCreditNote = ({ invoiceId, onCreditNoteAdded }) => {
  const [creditNoteNumber, setCreditNoteNumber] = useState('');
  const [creditNoteAmount, setCreditNoteAmount] = useState('');
  const [message, setMessage] = useState('');
  const [errors, setErrors] = useState({});

  useEffect(() => {
    setMessage('');
  }, [creditNoteNumber, creditNoteAmount]);

  const validate = () => {
    const newErrors = {};
    if (!creditNoteNumber || creditNoteNumber <= 0) {
      newErrors.creditNoteNumber = 'Debe ingresar un número válido para la nota de crédito.';
    }
    if (!creditNoteAmount || creditNoteAmount <= 0) {
      newErrors.creditNoteAmount = 'El monto debe ser mayor a cero.';
    }
    return newErrors;
  };

  const handleAddCreditNote = async () => {
    const validationErrors = validate();
    if (Object.keys(validationErrors).length > 0) {
      setErrors(validationErrors);
      return;
    }
    try {
      const response = await axios.post('http://localhost:5192/api/Factura/agregar-nc', {
        invoiceId: invoiceId,
        creditNoteNumber: parseInt(creditNoteNumber),
        creditNoteAmount: parseFloat(creditNoteAmount)
      });
      setMessage(response.data.message);
      setErrors({});
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
        {errors.creditNoteNumber && <p style={{ color: 'red' }}>{errors.creditNoteNumber}</p>}
      </div>
      <div>
        <label>Monto de Nota de Crédito:</label>
        <input
          type="number"
          value={creditNoteAmount}
          onChange={(e) => setCreditNoteAmount(e.target.value)}
        />
        {errors.creditNoteAmount && <p style={{ color: 'red' }}>{errors.creditNoteAmount}</p>}
      </div>
      <button onClick={handleAddCreditNote}>Agregar Nota de Crédito</button>
      {message && <p>{message}</p>}
    </div>
  );
};

export default AddCreditNote;

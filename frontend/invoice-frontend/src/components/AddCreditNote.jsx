import React, { useEffect, useState } from 'react';
import api from '../services/api';

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
      const response = await api.post('http://localhost:5192/api/Factura/agregar-nc', {
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
    <div className="mt-8 p-6 bg-white rounded-2xl shadow-md border border-gray-200">
      <h3 className="text-lg font-semibold mb-4">Agregar Nota de Crédito</h3>
      <div className="space-y-4">
        <label className="block text-sm font-medium text-gray-700 mb-1">Número de Nota de Crédito:</label>
        <input
          type="number"
          className="w-full border border-gray-300 rounded-lg px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-500"
          value={creditNoteNumber}
          onChange={(e) => setCreditNoteNumber(e.target.value)}
        />
        {errors.creditNoteNumber && <p className="text-red-500 text-sm mt-1">{errors.creditNoteNumber}</p>}
      </div>
      <div>
        <label className="block text-sm font-medium text-gray-700 mb-1">Monto de Nota de Crédito:</label>
        <input
          type="number"
          className="w-full border border-gray-300 rounded-lg px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-500"
          value={creditNoteAmount}
          onChange={(e) => setCreditNoteAmount(e.target.value)}
        />
        {errors.creditNoteAmount && <p className="text-red-500 text-sm mt-1">{errors.creditNoteAmount}</p>}
      </div>
      <button onClick={handleAddCreditNote}
      className="w-full bg-blue-600 text-white py-2 px-4 rounded-lg hover:bg-blue-700 transition-colors mt-4"
      >Agregar Nota de Crédito
      </button>
      {message && ( <p className={`text-sm mt-2 ${message.includes('exitosamente') ? 'text-green-600' : 'text-red-500'}`}>
       {message}
      </p>)}
    </div>    
  );
};

export default AddCreditNote;

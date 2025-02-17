import React, { useState } from 'react';

function App() {
    const [from, setFrom] = useState('');
    const [to, setTo] = useState('');
    const [amount, setAmount] = useState('');
    const [date, setDate] = useState('');
    const [result, setResult] = useState(null);
    const [error, setError] = useState(null);

    const handleSubmit = async (e) => {
        e.preventDefault();
        setError(null);
        setResult(null);

        try {
            // Construct query parameters; if date is empty, it can be omitted
            const params = new URLSearchParams({ from, to, amount });
            if (date) params.append("date", date);

            // Adjust the URL if needed (e.g., your API might run on a different port)
            const response = await fetch(`https://localhost:5001/conversion?${params.toString()}`);

            if (!response.ok) {
                throw new Error("API error: " + response.statusText);
            }

            const data = await response.json();
            setResult(data);
        } catch (err) {
            setError(err.message);
        }
    };

    return (
        <div style={{ padding: '20px' }}>
            <h1>Currency Converter</h1>
            <form onSubmit={handleSubmit}>
                <div>
                    <label>From Currency:</label>
                    <input type="text" value={from} onChange={(e) => setFrom(e.target.value)} placeholder="USD" />
                </div>
                <div>
                    <label>To Currency:</label>
                    <input type="text" value={to} onChange={(e) => setTo(e.target.value)} placeholder="EUR" />
                </div>
                <div>
                    <label>Amount:</label>
                    <input type="number" value={amount} onChange={(e) => setAmount(e.target.value)} placeholder="100" />
                </div>
                <div>
                    <label>Date (optional):</label>
                    <input type="date" value={date} onChange={(e) => setDate(e.target.value)} />
                </div>
                <button type="submit">Convert</button>
            </form>
            {error && <p style={{ color: 'red' }}>Error: {error}</p>}
            {result && (
                <div>
                    <h2>Conversion Result</h2>
                    <p>
                        {result.Amount} {result.From} = {result.ConvertedAmount} {result.To} on {result.Date}
                    </p>
                </div>
            )}
        </div>
    );
}

export default App;

import React, { useState, FormEvent } from 'react';

interface ConversionResult {
    amount: number;
    from: string;
    to: string;
    convertedAmount: number;
    date: string;
}

function App() {
    const [from, setFrom] = useState<string>('');
    const [to, setTo] = useState<string>('');
    const [amount, setAmount] = useState<string>('');
    const [date, setDate] = useState<string>('');
    const [result, setResult] = useState<ConversionResult | null>(null);
    const [error, setError] = useState<string | null>(null);

    const handleSubmit = async (e: FormEvent) => {
        e.preventDefault();
        setError(null);
        setResult(null);

        try {
            const params = new URLSearchParams({ from, to, amount });
            if (date) params.append("date", date);

            const response = await fetch(`https://localhost:7279/conversion?${params.toString()}`);

            if (!response.ok) {
                throw new Error("API error: " + response.statusText);
            }

            const data: ConversionResult = await response.json();
            console.log(data);
            setResult(data);
        } catch (err: any) {
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
                        {result.amount} {result.from} = {result.convertedAmount} {result.to} on {result.date}
                    </p>
                </div>
            )}
        </div>
    );
}

export default App;

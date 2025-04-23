import React, { useState } from 'react';
import bcrypt from 'bcryptjs';

const PasswordHash = () => {
    const [password, setPassword] = useState('');
    const [hash, setHash] = useState('');
    const [error, setError] = useState('');

    const handleSubmit = (e) => {
        e.preventDefault();
        if (!password) {
            setError('Vui lòng nhập mật khẩu!');
            setHash('');
            return;
        }

        const saltRounds = 11;
        bcrypt.hash(password, saltRounds, function (err, hash) {
            if (err) {
                setError('Lỗi khi mã hóa: ' + err.message);
                setHash('');
            } else {
                setHash(hash);
                setError('');
                console.log('Hash của mật khẩu:', hash); // Vẫn giữ console để debug
            }
        });
    };

    return (
        <div style={{ padding: '20px', maxWidth: '400px', margin: '0 auto' }}>
            <h2>Mã hóa mật khẩu</h2>
            <form onSubmit={handleSubmit}>
                <div style={{ marginBottom: '10px' }}>
                    <label>
                        Nhập mật khẩu:
                        <input
                            type="password"
                            value={password}
                            onChange={(e) => setPassword(e.target.value)}
                            style={{
                                width: '100%',
                                padding: '8px',
                                marginTop: '5px',
                                borderRadius: '4px',
                                border: '1px solid #ccc'
                            }}
                        />
                    </label>
                </div>
                <button
                    type="submit"
                    style={{
                        padding: '10px 20px',
                        backgroundColor: '#4CAF50',
                        color: 'white',
                        border: 'none',
                        borderRadius: '4px',
                        cursor: 'pointer'
                    }}
                >
                    Mã hóa
                </button>
            </form>
            {error && (
                <div style={{ color: 'red', marginTop: '10px' }}>
                    {error}
                </div>
            )}
            {hash && (
                <div style={{ marginTop: '20px', wordBreak: 'break-all' }}>
                    <strong>Mật khẩu đã mã hóa:</strong>
                    <p>{hash}</p>
                </div>
            )}
        </div>
    );
};

export default PasswordHash;
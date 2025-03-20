import React from 'react';

const SendMessagePHHS = () => {
    // Dữ liệu mẫu cho bảng
    const classData = [
        { id: 1, className: '6A', teacher: 'Phạm Thị Hải', signed: 0, total: 0, process: 0, remaining: 0 },
        { id: 2, className: '6B', teacher: 'Vũ Thị Huệ', signed: 0, total: 0, process: 0, remaining: 0 },
        { id: 3, className: '7A', teacher: 'Nguyễn Ngọc Trang', signed: 0, total: 0, process: 0, remaining: 0 },
        { id: 4, className: '7B', teacher: 'Nguyễn Hữu Luân', signed: 0, total: 0, process: 0, remaining: 0 },
        { id: 5, className: '7C', teacher: 'Bùi Văn Trang', signed: 0, total: 0, process: 0, remaining: 0 },
        { id: 6, className: '8A', teacher: 'Trần Thị Tuyết Nhung', signed: 0, total: 0, process: 0, remaining: 0 },
        { id: 7, className: '8B', teacher: 'Nguyễn Thị Huyền', signed: 0, total: 0, process: 0, remaining: 0 },
        { id: 8, className: '9A', teacher: 'Đỗ Thị Thu', signed: 0, total: 0, process: 0, remaining: 0 },
    ];

    return (
        <div style={styles.container}>
            {/* Header */}
            <div style={styles.header}>
                <h2 style={styles.headerTitle}>Lớp học tính: 0 (0 PHHS)</h2>
                <button style={styles.headerButton}>Tất cả</button>
            </div>

            {/* Table */}
            <table style={styles.table}>
                <thead>
                    <tr>
                        <th style={{ ...styles.th, width: '20px' }}>
                            <input type="checkbox" />
                        </th>
                        <th style={styles.th}>TT</th>
                        <th style={styles.th}>Lớp</th>
                        <th style={styles.th}>Giáo viên chủ nhiệm</th>
                        <th style={styles.th}>Số PHHS đã ký</th>
                        <th style={styles.th}>Tổng số</th>
                        <th style={styles.th}>Quy trình nhận (SMS/ĐÃ)</th>
                        <th style={styles.th}>Còn lại</th>
                    </tr>
                </thead>
                <tbody>
                    {classData.map((item) => (
                        <tr key={item.id} style={styles.tr}>
                            <td style={{ ...styles.td, width: '20px', textAlign: 'center' }}>
                                <input type="checkbox" />
                            </td>
                            <td style={styles.td}>{item.id}</td>
                            <td style={styles.td}>{item.className}</td>
                            <td style={styles.td}>{item.teacher}</td>
                            <td style={styles.td}>{item.signed}</td>
                            <td style={styles.td}>{item.total}</td>
                            <td style={styles.td}>{item.process}</td>
                            <td style={styles.td}>{item.remaining}</td>
                        </tr>
                    ))}
                </tbody>
            </table>

            {/* Footer */}
            <div style={styles.footer}>
                <label style={styles.footerLabel}>Đúng tính THCS Hùng Giang:</label>
                <input type="text" style={styles.footerInput} placeholder="" />
            </div>

            {/* Message Content Section */}
            <div style={styles.messageSection}>
                <h3 style={styles.messageTitle}>Nội dung tin nhắn</h3>
                <label style={styles.messageLabel}>Trường THCS Hải Giang:</label>
                <input type="text" style={styles.messageInput} placeholder="" />
                <div style={styles.noteContainer}>
                    <p style={styles.noteText}>
                        Lưu ý: Nội dung tin nhắn có chứa [Tên học sinh]/[Tên lớp] nên để lại tin nhắn ở mỗi học sinh sẽ khác nhau.
                    </p>
                    <span style={styles.noteStats}>231/1</span>
                </div>
            </div>
        </div>
    );
};

// CSS styles (inline)
const styles = {
    container: {
        maxWidth: '1200px',
        margin: '20px auto',
        fontFamily: 'Arial, sans-serif',
        backgroundColor: '#f5f5f5',
    },
    header: {
        display: 'flex',
        justifyContent: 'space-between',
        alignItems: 'center',
        backgroundColor: '#e8ecef',
        padding: '10px 20px',
        borderRadius: '5px',
    },
    headerTitle: {
        margin: 0,
        fontSize: '18px',
        color: '#333',
    },
    headerButton: {
        backgroundColor: '#17a2b8',
        color: 'white',
        border: 'none',
        padding: '8px 15px',
        borderRadius: '5px',
        cursor: 'pointer',
    },
    table: {
        width: '100%',
        borderCollapse: 'collapse',
        marginTop: '20px',
        backgroundColor: 'white',
        boxShadow: '0 1px 3px rgba(0, 0, 0, 0.1)',
    },
    th: {
        padding: '10px',
        textAlign: 'left',
        borderBottom: '1px solid #ddd',
        backgroundColor: '#e8ecef',
        fontWeight: 'bold',
        color: '#333',
    },
    td: {
        padding: '10px',
        textAlign: 'left',
        borderBottom: '1px solid #ddd',
        color: '#555',
    },
    tr: {
        transition: 'background-color 0.2s',
    },
    footer: {
        marginTop: '20px',
        backgroundColor: '#e8ecef',
        padding: '10px',
        borderRadius: '5px',
    },
    footerLabel: {
        fontWeight: 'bold',
        color: '#333',
    },
    footerInput: {
        width: '100%',
        padding: '8px',
        marginTop: '5px',
        border: '1px solid #ddd',
        borderRadius: '5px',
        boxSizing: 'border-box',
    },
    messageSection: {
        marginTop: '20px',
        padding: '10px',
        backgroundColor: '#e8ecef',
        borderRadius: '5px',
    },
    messageTitle: {
        fontSize: '16px',
        fontWeight: 'bold',
        color: '#333',
        marginBottom: '10px',
    },
    messageLabel: {
        fontWeight: 'bold',
        color: '#333',
    },
    messageInput: {
        width: '100%',
        padding: '8px',
        marginTop: '5px',
        border: '1px solid #ddd',
        borderRadius: '5px',
        boxSizing: 'border-box',
    },
    noteContainer: {
        display: 'flex',
        justifyContent: 'space-between',
        alignItems: 'center',
        marginTop: '10px',
    },
    noteText: {
        color: '#28a745', // Màu xanh lá
        fontSize: '14px',
        margin: 0,
    },
    noteStats: {
        fontSize: '14px',
        color: '#333',
    },
};

export default SendMessagePHHS;
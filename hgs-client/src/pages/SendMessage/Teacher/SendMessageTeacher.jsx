import React from 'react';

const SendMessageTeacher = () => {
    // Dữ liệu mẫu cho bảng
    const teacherData = [
        { id: 1, name: 'Tạ Thuấn Anh', code: '36366512-07-1-1', department: 'Khối học xã hội', phone: '0985836971', messagesViettel: 0, messagesOther: 0 },
        { id: 2, name: 'Vương Thị Ngọc Anh', code: '36366512-07-1-22', department: 'Khối học xã hội', phone: '0373450650', messagesViettel: 0, messagesOther: 0 },
        { id: 3, name: 'Nguyễn Thị Chiếm', code: '36366512-07-1-2', department: 'Tổ trưởng', phone: '094412500', messagesViettel: 0, messagesOther: 0 },
        { id: 4, name: 'Phạm Duy Duyên', code: '36366512-07-1-14', department: 'Khối học tự nhiên', phone: '083462358', messagesViettel: 0, messagesOther: 0 },
        { id: 5, name: 'Nguyễn Thị Duyên', code: '36366512-07-1-6', department: 'Tổ trưởng', phone: '097740375', messagesViettel: 0, messagesOther: 0 },
        { id: 6, name: 'Phạm Thị Hải', code: '36366512-07-1-4', department: 'Khối học xã hội', phone: '0857124936', messagesViettel: 0, messagesOther: 0 },
    ];

    return (
        <div style={styles.container}>
            {/* Sidebar */}
            <div style={styles.sidebar}>
                <h3 style={styles.sidebarTitle}>Danh sách thống kê</h3>
                <ul style={styles.sidebarList}>
                    <li style={styles.sidebarItem}>Tổng quan</li>
                    <li style={styles.sidebarItem}>Lớp học bộ môn</li>
                    <li style={styles.sidebarItem}>Cán bộ năm</li>
                    <li style={styles.sidebarItem}>Cán bộ lớp</li>
                    <li style={styles.sidebarItem}>Danh sách SĐT VIETTEL</li>
                    <li style={styles.sidebarItem}>Bản tin hằng tuần</li>
                    <li style={styles.sidebarItem}>Chi bộ</li>
                    <li style={styles.sidebarItem}>Công đoàn trường</li>
                    <li style={styles.sidebarItem}>Đoàn thanh niên</li>
                    <li style={styles.sidebarItemActive}>Giáo viên chủ nhiệm</li>
                </ul>
            </div>

            {/* Main Content */}
            <div style={styles.mainContent}>
                {/* Header */}
                <div style={styles.header}>
                    <h2 style={styles.headerTitle}>Toàn trường: 23 Giáo viên</h2>
                    <h3 style={styles.subHeader}>Nguời nhận: 0 giáo viên</h3>
                </div>

                {/* Teacher List */}
                <div style={styles.tableContainer}>
                    <h3 style={styles.tableTitle}>Danh sách giáo viên</h3>
                    <table style={styles.table}>
                        <thead>
                            <tr>
                                <th style={{ ...styles.th, width: '20px' }}>
                                    <input type="checkbox" />
                                </th>
                                <th style={styles.th}>Họ và tên giáo viên</th>
                                <th style={styles.th}>Mã cán bộ</th>
                                <th style={styles.th}>Tổ bộ môn</th>
                                <th style={styles.th}>Số ĐT ĐD</th>
                                <th style={styles.th}>Số tin nhắn đã nhận Mạng Viettel</th>
                                <th style={styles.th}>Số tin nhắn đã nhận Mạng khác</th>
                            </tr>
                        </thead>
                        <tbody>
                            {teacherData.map((teacher) => (
                                <tr key={teacher.id} style={styles.tr}>
                                    <td style={{ ...styles.td, width: '20px', textAlign: 'center' }}>
                                        <input type="checkbox" />
                                    </td>
                                    <td style={styles.td}>{teacher.name}</td>
                                    <td style={styles.td}>{teacher.code}</td>
                                    <td style={styles.td}>{teacher.department}</td>
                                    <td style={styles.td}>{teacher.phone}</td>
                                    <td style={styles.td}>{teacher.messagesViettel}</td>
                                    <td style={styles.td}>{teacher.messagesOther}</td>
                                </tr>
                            ))}
                        </tbody>
                    </table>

                    {/* Pagination */}
                    <div style={styles.pagination}>
                        <span style={styles.paginationText}>Tổng số: 23 - Hiển thị 1 - 10</span>
                        <div style={styles.paginationButtons}>
                            <button style={styles.paginationButton}>◄</button>
                            <button style={{ ...styles.paginationButton, backgroundColor: '#17a2b8', color: 'white' }}>1</button>
                            <button style={styles.paginationButton}>2</button>
                            <button style={styles.paginationButton}>3</button>
                            <button style={styles.paginationButton}>...</button>
                            <button style={styles.paginationButton}>►</button>
                        </div>
                    </div>
                </div>

                {/* Message Section */}
                <div style={styles.messageSection}>
                    <h3 style={styles.messageTitle}>Nguời nhận: 0 giáo viên</h3>
                    <input type="text" style={styles.messageInput} placeholder="Toàn trường" />
                    <textarea style={styles.messageTextarea} rows="4" placeholder=""></textarea>
                    <div style={styles.messageFooter}>
                        <label style={styles.checkboxLabel}>
                            <input type="checkbox" /> Gửi tin nhắn có dấu
                        </label>
                        <span style={styles.charCount}>0/0</span>
                        <button style={styles.sendButton}>
                            Gửi <span style={styles.sendIcon}>➤</span>
                        </button>
                    </div>
                </div>
            </div>
        </div>
    );
};

// CSS styles (inline)
const styles = {
    container: {
        display: 'flex',
        minHeight: '100vh',
        fontFamily: 'Arial, sans-serif',
        backgroundColor: '#f5f5f5',
    },
    sidebar: {
        width: '200px',
        backgroundColor: '#e8ecef',
        padding: '20px',
        borderRight: '1px solid #ddd',
    },
    sidebarTitle: {
        fontSize: '16px',
        fontWeight: 'bold',
        color: '#333',
        marginBottom: '20px',
    },
    sidebarList: {
        listStyle: 'none',
        padding: 0,
        margin: 0,
    },
    sidebarItem: {
        padding: '10px 0',
        color: '#555',
        cursor: 'pointer',
    },
    sidebarItemActive: {
        padding: '10px 0',
        color: '#17a2b8',
        fontWeight: 'bold',
        cursor: 'pointer',
    },
    mainContent: {
        flex: 1,
        padding: '20px',
    },
    header: {
        marginBottom: '20px',
    },
    headerTitle: {
        fontSize: '18px',
        fontWeight: 'bold',
        color: '#333',
        margin: 0,
    },
    subHeader: {
        fontSize: '16px',
        color: '#555',
        margin: '5px 0 0 0',
    },
    tableContainer: {
        backgroundColor: 'white',
        padding: '10px',
        borderRadius: '5px',
        boxShadow: '0 1px 3px rgba(0, 0, 0, 0.1)',
    },
    tableTitle: {
        fontSize: '16px',
        fontWeight: 'bold',
        color: '#333',
        marginBottom: '10px',
    },
    table: {
        width: '100%',
        borderCollapse: 'collapse',
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
    pagination: {
        display: 'flex',
        justifyContent: 'space-between',
        alignItems: 'center',
        marginTop: '10px',
    },
    paginationText: {
        fontSize: '14px',
        color: '#555',
    },
    paginationButtons: {
        display: 'flex',
        gap: '5px',
    },
    paginationButton: {
        padding: '5px 10px',
        border: '1px solid #ddd',
        backgroundColor: 'white',
        color: '#333',
        cursor: 'pointer',
        borderRadius: '3px',
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
    messageInput: {
        width: '100%',
        padding: '8px',
        marginBottom: '10px',
        border: '1px solid #ddd',
        borderRadius: '5px',
        boxSizing: 'border-box',
    },
    messageTextarea: {
        width: '100%',
        padding: '8px',
        marginBottom: '10px',
        border: '1px solid #ddd',
        borderRadius: '5px',
        boxSizing: 'border-box',
        resize: 'none',
    },
    messageFooter: {
        display: 'flex',
        justifyContent: 'space-between',
        alignItems: 'center',
    },
    checkboxLabel: {
        display: 'flex',
        alignItems: 'center',
        color: '#333',
    },
    charCount: {
        fontSize: '14px',
        color: '#555',
    },
    sendButton: {
        backgroundColor: '#17a2b8',
        color: 'white',
        border: 'none',
        padding: '8px 15px',
        borderRadius: '5px',
        cursor: 'pointer',
        display: 'flex',
        alignItems: 'center',
        gap: '5px',
    },
    sendIcon: {
        fontSize: '14px',
    },
};

export default SendMessageTeacher;